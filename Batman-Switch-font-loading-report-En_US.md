# Batman (Switch) Fonts: the `0x30`/`0x31` Flag and the MetaStream Structure

## 1. The `0x30` / `0x31` flag

### 1.1 Identity

`OneByte` in the texture header is the serialized value of `ToolProps::mbHasProps`, encoded as a single ASCII byte:

| Byte | ASCII | Meaning |
| --- | --- | --- |
| `0x30` | `'0'` | No ToolProps; `mNumMipLevels` follows immediately |
| `0x31` | `'1'` | A PropertySet follows; it must be consumed before `mNumMipLevels` |

The MetaStream boolean serializer also tolerates a four-byte form (if the first byte is neither `0x30` nor `0x31`, it reads three more bytes and tests for non-zero), but every font sample examined uses the single-byte ASCII encoding.

### 1.2 Observed distribution

Across the merged base + update RomFS: 89 fonts, 746 texture pages.

| Value | Pages | Main sources |
| --- | ---: | --- |
| `0x30` | 355 | Base-game language archives, `BAT_nx_AR_FONTS` |
| `0x31` | 391 | `BAT_nx_CH_FONTS` (196), `BAT_nx_TC_FONTS` (194), base `default.font` (1) |

Within the same update package, `AR_FONTS` is entirely `0x30` while `CH`/`TC_FONTS` are entirely `0x31`, so the value depends on how each font asset was produced, not on platform or engine version.

### 1.3 Runtime behaviour

In the updated `main` (build-id `a3a998af…`) the ToolProps `0x4A` serialize callback is at `0x9702C0` and behaves exactly like the older `0x2EA46C`:

```cpp
if (stream.mode == 2)          // writing
    toolProps.mbHasProps = false;   // tool props are always stripped
stream.SerializeBool(toolProps.mbHasProps);
if (toolProps.mbHasProps) {
    PropertySet temporaryProps;
    SerializePropertySet(stream, temporaryProps);  // read, then discarded
}
```

So on read the data must be consumed to keep later fields aligned, while on write it is cleared. This explains why most assets carry no tool props and a minority still do.

### 1.4 Runtime measurement (Citron GDB stub)

A breakpoint at `0x970300` (just after `ldrb w8,[x20]`) read `mName` (`x20-0x214`), `mImportName` (`x20-0x1C`), `mbHasProps` (`x20+0`) and `mPlatform` (`x20+4`) on every hit. Across boot to title screen, pressing A into the main menu, and entering gameplay until dialogue text and dialogue choices appeared, 34937 hits were collected.

- Every `.font` texture page reported `mbHasProps = 1`; every ordinary `.d3dtx`/`.particle` texture reported `0`. No exceptions.
- The fonts loaded at runtime come from the update package's `BAT_nx_CH_FONTS_compressed.ttarch2` (Simplified Chinese); per-font page counts match the static CH_FONTS samples exactly.

Pages observed per font:

| `.font` | Pages |
| --- | ---: |
| `default.font` | 1 |
| `MenuBodyLarge` / `MenuBodyMedium` / `MenuBodySmall` | 32 / 15 / 8 |
| `MenuDisplayLarge` / `MenuDisplayMedium` / `MenuDisplaySmall` | 31 / 14 / 6 |
| `GameplayBodyLarge` / `GameplayBodyMedium` / `GameplayBodySmall` | 25 / 13 / 8 |
| `GameplayDisplayLarge` / `GameplayDisplayMedium` | 26 / 12 |

`GameplayBodySmall` was loaded three times in a single session (pages 0–7 each time), i.e. repeated instance loading rather than new assets.

---

## 2. MetaStream structure

### 2.1 Layout after `0x31` in a texture header

```text
u8  mbHasProps                      // 0x31
u32 mPropVersion                    // editor field: UnknownData.count
u32 mPropertyFlags                  // editor field: UnknownData.Unknown1
u32 propertyContentBlockSize        // editor field: UnknownData.len, includes itself
u32 parentPropertyCount
u32 typeGroupCount
...type-grouped properties...
u32 mNumMipLevels                   // continues after the PropertySet
u32 mWidth
u32 mHeight
```

Block length semantics: the size field counts itself, and the block ends at `blockStart + blockSize`. On read the position is derived that way; on write the value is backfilled as `end - blockStart`.

### 2.2 Constants measured in this sample set

| Field | Value |
| --- | --- |
| `mPropVersion` | `2` |
| `mPropertyFlags` | `0` |
| `propertyContentBlockSize` | `130` (`0x82`) |
| `parentPropertyCount` | `0` |
| `typeGroupCount` | `3` |
| Total properties | `5` |
| PropertySet length | `138` bytes (including version and flags) |
| Including the `0x31` byte | `139` bytes |

The block length is independently verifiable by summing: length prefix + parent count + group count = `12`; `uint32` group `36`; `bool` group `30`; `SArray<unsigned char,32>` group `52`; total `130`.

### 2.3 Exact offsets

`GameplayBodyLarge.font` (CH_FONTS), page 0:

| File offset | Content |
| --- | --- |
| `0x207EB` | first T3Texture header |
| `0x20875` | `mbHasProps = 0x31` |
| `0x20876` | `mPropVersion = 2` |
| `0x2087A` | `mPropertyFlags = 0` |
| `0x2087E` | property block length `0x82` |
| `0x20882` | parent property count `0` |
| `0x20886` | type group count `3` |
| `0x20900` | property block end; `mNumMipLevels = 1` follows |
| `0x20904` / `0x20908` | width / height, both `512` |

For the `0x30` branch: `BM_nx_Chinese_txmesh/GameplayBodyLarge.font`, page 0 has its header at `0x19117`, `0x191A1` is `30`, and `0x191A2` is the mip count `01 00 00 00` with no 138-byte skip.

### 2.4 Structural properties

- All 196 `0x31` blocks re-encode to exactly the original 138 bytes, confirming the parse is complete and no field is missing.
- 182 distinct contents: 168 appear once, 14 appear twice. Every duplicate pair is between `GameplayBodySmall↔MenuBodySmall` and `GameplayDisplaySmall↔MenuDisplaySmall` on the same page index, and those pages also share identical import paths and pixel data.
- The first 98 bytes of the 130-byte block are identical across all samples; only the final 32 bytes (the `SArray<unsigned char,32>` value) vary, at `+0x62`–`+0x81` relative to the block start including the length prefix.

---

## 3. CRC64 and property keys

### 3.1 Algorithm

Keys and type hashes use **CRC-64/ECMA-182, case-insensitive**:

| Parameter | Value |
| --- | --- |
| Polynomial | `0x42F0E1EBA9EA3693` |
| Initial value | `0` |
| Input reflection | no |
| Final XOR | no |
| Input preprocessing | lower-case |

Self-check: `SArray<unsignedchar,32>` → `0x6B62F90BD5F79321`, matching the type hash read from samples.

### 3.2 The five properties

| Type | Type hash | Key hash | Recovered name | Values in samples |
| --- | --- | --- | --- | --- |
| `uint32` | `0x4B26A2CA30DE3F32` | `0x281A2846A86CC06B` | **unresolved** | always `0` |
| `uint32` | same | `0xC720B2182BE4DD88` | `AlphaMode` | always `0xFFFFFFFF` |
| `bool` | `0x9004C5587575D6C0` | `0x1F866CADECB045E2` | `AllowCompression` | always `true` |
| `bool` | same | `0xA7793EFE02E09464` | `GenerateMips` | always `false` |
| `SArray<unsigned char,32>` | `0x6B62F90BD5F79321` | `0x087A51312327D5AA` | `ImportHash` | 182 distinct values over 196 pages |

The names are recovered by exact 64-bit matching against a dictionary of roughly 560k candidates, not strings read out of game data; the collision probability is negligible. The values are self-consistent (`GenerateMips = false` matches `mNumMipLevels = 1`). `AlphaMode = 0xFFFFFFFF` looks like an "unspecified" sentinel, which is an inference rather than a verified fact.

### 3.3 Nature of `ImportHash`

- It corresponds one-to-one with pixel content identity: grouping pixel data by SHA-256 yields exactly 182 groups, matching the 32-byte value grouping with no crossovers.
- It is **not** a SHA-256 of the pixel data stored in the file (none of the 196 pages match).
- The generation algorithm and its input data remain undetermined; possibilities include a hash of the source image before import, or some other intermediate data.

### 3.4 These keys play no part in runtime logic

Searching the updated `main.memory.bin` (17,158,920 bytes) for the 8-byte literals of the 5 key hashes and 3 type hashes, plus 4 `MOVZ/MOVK` immediate sequences, produced **zero hits**. This matches the behaviour of `0x9702C0`, which discards the PropertySet after reading it. They are Telltale Tool import-time metadata and are never looked up by name at runtime.

---

## 4. The one remaining unresolved key

`0x281A2846A86CC06B` (type `uint32`) is the only key without a name:

- Not present in `ToolLibrary.HashDB` from `TTL_Inspector200_ALL_data.ttarch2` (471 symbol-table pages).
- Not matched by the existing candidate dictionaries (texture-import vocabulary plus `Has/Is/Allow/Generate/Import/Source` prefix combinations).
- Constant `0` across every sample analysed so far (196 + 337 + 194 + 19 pages).

Its meaning should not be guessed before more tool-side source or a larger dictionary is available. Viable next steps:

1. Enumerate property write sites in the TelltaleToolLib / Inspector texture import code and hash the string constants directly.
2. Re-export the same source image with changed import options and see whether this field changes.
3. Widen the dictionary toward `Texture`/`D3DTX` tool-property naming conventions.

---

## 5. Conclusions

1. `0x30`/`0x31` is the ASCII boolean encoding of `ToolProps::mbHasProps`. Both branches genuinely occur in Batman Switch assets, and the reader must branch on it.
2. `0x31` is followed by a PropertySet with version 2, flags 0 and a 130-byte content block. Field order and length semantics are fully verified and re-encode byte-for-byte.
3. CRC-64/ECMA-182 recovers 4 of the 5 key names; one `uint32` key (`0x281A2846A86CC06B`) is still unidentified.
4. These properties are tool-side metadata unused at runtime; in measurement, all 184 runtime `.font` texture pages were `0x31` and retained their PropertySet.
