# Batman Switch 字体：`0x30`/`0x31` 标志与 MetaStream 结构报告

## 1. `0x30` / `0x31` 标志位

### 1.1 身份

纹理头中的 `OneByte` 是 `ToolProps::mbHasProps` 的序列化布尔值，编码为单字节 ASCII：

| 字节 | ASCII | 含义 |
| --- | --- | --- |
| `0x30` | `'0'` | 无 ToolProps，紧随 `mNumMipLevels` |
| `0x31` | `'1'` | 后接一个 PropertySet，解析完才到 `mNumMipLevels` |

MetaStream 的布尔实现另兼容一种四字节形式（首字节非 `0x30/0x31` 时额外读 3 字节再按非零判定），但本批字体样本全部使用单字节 ASCII 编码。

### 1.2 两种分支的实际分布

合并底包与更新包 RomFS 后的 89 个字体、746 页纹理：

| 取值 | 页数 | 主要来源 |
| --- | ---: | --- |
| `0x30` | 355 | 底包各语言归档、`BAT_nx_AR_FONTS` |
| `0x31` | 391 | `BAT_nx_CH_FONTS`(196)、`BAT_nx_TC_FONTS`(194)、底包 `default.font`(1) |

同一批更新包里 `AR_FONTS` 全为 `0x30`、`CH/TC_FONTS` 全为 `0x31`，说明取值取决于字体资源的制作方式，而不是平台或引擎版本。

### 1.3 运行时代码行为

更新版 `main`（build-id `a3a998af…`）中 ToolProps 的 `0x4A` 序列化回调位于 `0x9702C0`，行为与旧版 `0x2EA46C` 一致：

```cpp
if (stream.mode == 2)          // 写出
    toolProps.mbHasProps = false;   // 强制清除工具属性
stream.SerializeBool(toolProps.mbHasProps);
if (toolProps.mbHasProps) {
    PropertySet temporaryProps;
    SerializePropertySet(stream, temporaryProps);  // 读入后丢弃
}
```

因此：读入时必须消费这部分数据以保持后续字段对齐；写出时会被主动清除，这解释了「多数资源无属性、少数仍保留」的兼容性。

### 1.4 运行时实测（Citron GDB stub）

在 `0x970300`（`ldrb w8,[x20]` 之后）下断点，读取 `mName`（`x20-0x214`）、`mImportName`（`x20-0x1C`）、`mbHasProps`（`x20+0`）、`mPlatform`（`x20+4`）：

- 启动到标题界面、按 A 进主菜单、进入剧情并出现对话文字/对话选项期间，共采集到 34937 次命中。
- 所有 `.font` 纹理页的 `mbHasProps` 始终为 `1`；所有 `.d3dtx`/`.particle` 普通纹理始终为 `0`，无例外。
- 运行时加载的是更新包 `BAT_nx_CH_FONTS_compressed.ttarch2` 中的简体中文字体，页数与 CH_FONTS 静态样本逐项吻合。

已实测的字体页数：

| `.font` | 页数 |
| --- | ---: |
| `default.font` | 1 |
| `MenuBodyLarge` / `MenuBodyMedium` / `MenuBodySmall` | 32 / 15 / 8 |
| `MenuDisplayLarge` / `MenuDisplayMedium` / `MenuDisplaySmall` | 31 / 14 / 6 |
| `GameplayBodyLarge` / `GameplayBodyMedium` / `GameplayBodySmall` | 25 / 13 / 8 |
| `GameplayDisplayLarge` / `GameplayDisplayMedium` | 26 / 12 |

`GameplayBodySmall` 在单次运行中被加载 3 轮（每轮 0–7 页），属于重复实例加载，不是新资源。

---

## 2. MetaStream 结构

### 2.1 纹理头中 `0x31` 之后的布局

```text
u8  mbHasProps                      // 0x31
u32 mPropVersion                    // 编辑器字段：UnknownData.count
u32 mPropertyFlags                  // 编辑器字段：UnknownData.Unknown1
u32 propertyContentBlockSize        // 编辑器字段：UnknownData.len，含自身
u32 parentPropertyCount
u32 typeGroupCount
...按类型分组的属性...
u32 mNumMipLevels                   // PropertySet 结束后继续
u32 mWidth
u32 mHeight
```

块长度语义：前缀自身计入长度，块终点 = `blockStart + blockSize`；读入时按此推算，写出时回填 `end - blockStart`。

### 2.2 本批样本的实测常量

| 字段 | 值 |
| --- | --- |
| `mPropVersion` | `2` |
| `mPropertyFlags` | `0` |
| `propertyContentBlockSize` | `130` (`0x82`) |
| `parentPropertyCount` | `0` |
| `typeGroupCount` | `3` |
| 属性总数 | `5` |
| PropertySet 总长 | `138` 字节（含版本与标志） |
| 含 `0x31` 的占用 | `139` 字节 |

块长可独立相加验证：长度前缀 + 父数量 + 类型组数 = `12`；`uint32` 组 `36`；`bool` 组 `30`；`SArray<unsigned char,32>` 组 `52`；合计 `130`。

### 2.3 精确偏移示例

`GameplayBodyLarge.font`（CH_FONTS）第 0 页：

| 文件偏移 | 内容 |
| --- | --- |
| `0x207EB` | 首个 T3Texture 头 |
| `0x20875` | `mbHasProps = 0x31` |
| `0x20876` | `mPropVersion = 2` |
| `0x2087A` | `mPropertyFlags = 0` |
| `0x2087E` | 属性块长度 `0x82` |
| `0x20882` | 父属性数量 `0` |
| `0x20886` | 类型组数量 `3` |
| `0x20900` | 属性块终点；随后 `mNumMipLevels = 1` |
| `0x20904` / `0x20908` | 宽 / 高，均 `512` |

`0x30` 分支示例：`BM_nx_Chinese_txmesh/GameplayBodyLarge.font` 第 0 页，头在 `0x19117`，`0x191A1` 为 `30`，`0x191A2` 直接是 Mip `01 00 00 00`，无 138 字节跳过。

### 2.4 结构性质

- 196 个 `0x31` 块全部重新编码后与原始 138 字节逐字节一致，说明解析完整、无遗漏字段。
- 182 种不同内容：168 种出现 1 次，14 种出现 2 次；重复对全部来自 `GameplayBodySmall↔MenuBodySmall` 与 `GameplayDisplaySmall↔MenuDisplaySmall` 的同名页，其导入路径与像素数据也逐对相同。
- 130 字节块中前 98 字节在所有样本中完全一致，差异只出现在最后 32 字节（`SArray<unsigned char,32>` 属性值），范围 `+0x62`～`+0x81`（以含前缀的块首为基准）。

---

## 3. CRC64 与属性键

### 3.1 算法

键与类型哈希使用 **CRC-64/ECMA-182（大小写不敏感）**：

| 参数 | 值 |
| --- | --- |
| 多项式 | `0x42F0E1EBA9EA3693` |
| 初值 | `0` |
| 输入反射 | 否 |
| 输出异或 | 否 |
| 输入预处理 | 转小写 |

实现自检：`SArray<unsignedchar,32>` → `0x6B62F90BD5F79321`，与样本中读到的类型哈希一致。

### 3.2 五个属性

| 类型 | 类型哈希 | 键哈希 | 还原名称 | 样本值 |
| --- | --- | --- | --- | --- |
| `uint32` | `0x4B26A2CA30DE3F32` | `0x281A2846A86CC06B` | **未还原** | 全部 `0` |
| `uint32` | 同上 | `0xC720B2182BE4DD88` | `AlphaMode` | 全部 `0xFFFFFFFF` |
| `bool` | `0x9004C5587575D6C0` | `0x1F866CADECB045E2` | `AllowCompression` | 全部 `true` |
| `bool` | 同上 | `0xA7793EFE02E09464` | `GenerateMips` | 全部 `false` |
| `SArray<unsigned char,32>` | `0x6B62F90BD5F79321` | `0x087A51312327D5AA` | `ImportHash` | 196 页共 182 种取值 |

名称是通过约 56 万候选词典做 **64 位精确匹配**得到的还原结果，不是游戏数据里直接读到的字符串；碰撞概率可忽略。取值与结构自洽（`GenerateMips = false` 与 `mNumMipLevels = 1` 一致）。`AlphaMode = 0xFFFFFFFF` 看起来是「未指定」哨兵值，属推断。

### 3.3 `ImportHash` 的性质

- 与纹理像素内容身份一一对应：像素数据按 SHA-256 分组恰为 182 组，与 32 字节属性值分组完全对应，不存在交叉。
- **不等于**对文件内保存的像素数据直接计算 SHA-256（196 页无一匹配）。
- 生成算法与数据来源仍未确定，可能是导入前源图的哈希或其它中间数据。

### 3.4 这些键在运行时不参与逻辑

在更新版 `main.memory.bin`（17158920 字节）中搜索 5 个键哈希与 3 个类型哈希的 8 字节字面量，以及 4 条 `MOVZ/MOVK` 立即数序列，**全部 0 命中**。这与 `0x9702C0` 读入后丢弃 PropertySet 的行为一致：它们是 Telltale Tool 导入阶段的元数据，游戏运行时不按名字读取。

---

## 4. 剩余未还原的键

`0x281A2846A86CC06B`（类型 `uint32`）是唯一尚未确定名称的键：

- Inspector 安装目录内 `TTL_Inspector200_ALL_data.ttarch2` 的 `ToolLibrary.HashDB`（471 个页面的符号表）中未命中。
- 现有候选字典（纹理导入相关词、`Has/Is/Allow/Generate/Import/Source` 等前缀组合）未命中。
- 全部已分析样本（196 + 337 + 194 + 19 页）中取值恒为 `0`。

在获得更多工具侧源码或更大词典证据前，其语义不宜猜测。可行方向：

1. 从 TelltaleToolLib / Inspector 的纹理导入代码中枚举属性写入点，直接取字符串常量算哈希。
2. 用同一源图改变导入选项重新导出，观察该字段是否变化。
3. 扩大字典至 `Texture`/`D3DTX` 工具属性命名风格。

---

## 5. 结论

1. `0x30`/`0x31` 是 `ToolProps::mbHasProps` 的 ASCII 布尔编码，两种分支在 Batman Switch 资源中都真实存在，读入必须分支消费。
2. `0x31` 后是一个版本 2、标志 0、内容块 130 字节的 PropertySet，字段顺序与长度语义已完整验证，重编码可逐字节还原。
3. CRC64/ECMA-182 可精确还原 4 个键名，剩余 1 个 `uint32` 键（`0x281A2846A86CC06B`）仍未确定。
4. 这组属性是工具侧元数据，游戏运行时不使用；运行时实测 184 个 `.font` 纹理页全部为 `0x31` 且保留 PropertySet。
