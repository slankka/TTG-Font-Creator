using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TTG_Tools.ClassesStructs.Text;
using TTG_Tools.Texts;

namespace TTG_Tools
{
    public partial class LandbEditor : Form
    {
        private string _filePathA, _filePathB;
        private LandbClass _landbA, _landbB;
        private List<CommonText> _textsA, _textsB;
        private bool _isUnicodeA, _isUnicodeB;
        private bool _mapCreditsA, _mapCreditsB;
        private bool _isDirtyA, _isDirtyB;

        public LandbEditor()
        {
            InitializeComponent();
            HookSyncScroll();
            HookEditingControls();
            RestoreLastDirectories();
        }

        private void HookEditingControls()
        {
            _gridViewA.EditingControlShowing += OnEditingControlShowing;
            _gridViewB.EditingControlShowing += OnEditingControlShowing;
        }

        private void OnEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var tb = e.Control as TextBox;
            if (tb == null) return;

            tb.Multiline = true;
            tb.AcceptsReturn = true;
            tb.ScrollBars = ScrollBars.Vertical;
            tb.WordWrap = true;

            // The DGV copies the cell value to the TextBox BEFORE this event,
            // but if TextBox was single-line at that point, newlines are stripped.
            // Re-set the text from the cell's actual value.
            var grid = sender as DataGridView;
            if (grid?.CurrentCell != null)
            {
                string cellValue = grid.CurrentCell.Value?.ToString() ?? "";
                if (cellValue.Contains("\n"))
                    tb.Text = cellValue;
            }
        }

        private void RestoreLastDirectories()
        {
            string dirA = AppData.settings.landbEditorLastDirA;
            string dirB = AppData.settings.landbEditorLastDirB;
            if (!string.IsNullOrEmpty(dirA) && Directory.Exists(dirA))
            {
                _txtPathA.Text = dirA;
                RefreshTree(_treeViewA, dirA);
                Log($"Restored Dir A: {dirA}");
            }
            if (!string.IsNullOrEmpty(dirB) && Directory.Exists(dirB))
            {
                _txtPathB.Text = dirB;
                RefreshTree(_treeViewB, dirB);
                Log($"Restored Dir B: {dirB}");
            }
        }

        private void HookSyncScroll()
        {
            if (_syncScrollMenu.Checked)
            {
                _gridViewA.Scroll += OnGridAScroll;
                _gridViewB.Scroll += OnGridBScroll;
            }
            // Also sync row selection so clicking an entry highlights the counterpart
            _gridViewA.SelectionChanged += OnGridASelection;
            _gridViewB.SelectionChanged += OnGridBSelection;
        }

        private bool _suppressSelectionSync;
        private bool _suppressScrollSync;
        private int _lastEntryA = -1, _lastEntryB = -1;

        // ========== Menu / toolbar events ==========

        private void OnOpenDirA(object sender, EventArgs e) => BrowseDirectory('A');
        private void OnOpenDirB(object sender, EventArgs e) => BrowseDirectory('B');
        private void OnCloseMenu(object sender, EventArgs e) => Close();
        private void OnBrowseA(object sender, EventArgs e) => BrowseDirectory('A');
        private void OnBrowseB(object sender, EventArgs e) => BrowseDirectory('B');
        private void OnSaveA(object sender, EventArgs e) => Save('A');
        private void OnSaveB(object sender, EventArgs e) => Save('B');
        private void OnSaveAsA(object sender, EventArgs e) => SaveAs('A');
        private void OnSaveAsB(object sender, EventArgs e) => SaveAs('B');

        private void OnExportCharsA(object sender, EventArgs e) => ExportAllChars('A');
        private void OnExportCharsB(object sender, EventArgs e) => ExportAllChars('B');

        private void OnTreeSelectA(object sender, TreeViewEventArgs e) => OnTreeSelect('A', e.Node);
        private void OnTreeSelectB(object sender, TreeViewEventArgs e) => OnTreeSelect('B', e.Node);

        private void OnCellChangedA(object sender, DataGridViewCellEventArgs e) => OnCellChanged('A');
        private void OnCellChangedB(object sender, DataGridViewCellEventArgs e) => OnCellChanged('B');

        private void OnCellValidatingA(object sender, DataGridViewCellValidatingEventArgs e) => OnCellValidating('A', e);
        private void OnCellValidatingB(object sender, DataGridViewCellValidatingEventArgs e) => OnCellValidating('B', e);

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                if (_gridViewB.ContainsFocus) Save('B');
                else Save('A');
            }
        }

        // ========== Directory ==========

        private void BrowseDirectory(char side)
        {
            using (var dlg = new FolderBrowserDialog { Description = "Select directory containing .landb files" })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    if (side == 'A')
                    {
                        _txtPathA.Text = dlg.SelectedPath; RefreshTree(_treeViewA, dlg.SelectedPath);
                        AppData.settings.landbEditorLastDirA = dlg.SelectedPath;
                    }
                    else
                    {
                        _txtPathB.Text = dlg.SelectedPath; RefreshTree(_treeViewB, dlg.SelectedPath);
                        AppData.settings.landbEditorLastDirB = dlg.SelectedPath;
                    }
                    Settings.SaveConfig(AppData.settings);
                    Log($"Directory {side}: {dlg.SelectedPath}");
                }
            }
        }

        private void RefreshTree(TreeView tree, string rootDir)
        {
            tree.Nodes.Clear();
            if (!Directory.Exists(rootDir)) return;
            var rootNode = new TreeNode(Path.GetFileName(rootDir)) { Tag = rootDir, Name = rootDir };
            tree.Nodes.Add(rootNode);
            PopulateTreeNodes(rootNode, rootDir);
            rootNode.Expand();
        }

        private void PopulateTreeNodes(TreeNode parentNode, string dirPath)
        {
            try
            {
                foreach (var subDir in Directory.GetDirectories(dirPath))
                {
                    var dirNode = new TreeNode(Path.GetFileName(subDir)) { Tag = subDir, Name = subDir };
                    parentNode.Nodes.Add(dirNode);
                    PopulateTreeNodes(dirNode, subDir);
                }
                foreach (var file in Directory.GetFiles(dirPath, "*.landb"))
                {
                    var fileNode = new TreeNode(Path.GetFileName(file)) { Tag = file, Name = file, ForeColor = Color.DarkBlue };
                    parentNode.Nodes.Add(fileNode);
                }
            }
            catch (UnauthorizedAccessException) { }
        }

        // ========== File load ==========

        private void OnTreeSelect(char side, TreeNode node)
        {
            if (node?.Tag == null) return;
            string path = node.Tag.ToString();
            if (File.Exists(path)) LoadLandbToSide(side, path);
        }

        private void LoadLandbToSide(char side, string filePath)
        {
            try
            {
                bool isUnicode, mapCredits; string errorMsg;
                var landb = LandbWorker.LoadLandbFromFile(filePath, out isUnicode, out mapCredits, out errorMsg);
                if (landb == null) { Log($"ERROR ({side}): {errorMsg}"); return; }
                var texts = LandbWorker.LandbToCommonTextList(landb, mapCredits);

                if (side == 'A')
                {
                    _filePathA = filePath; _landbA = landb; _textsA = texts;
                    _isUnicodeA = isUnicode; _mapCreditsA = mapCredits; _isDirtyA = false;
                    PopulateGrid(_gridViewA, texts);
                    _lblFileInfoA.Text = $"{Path.GetFileName(filePath)} ({texts.Count} entries)" + (isUnicode ? " [U]" : "");
                    _btnSaveA.Enabled = _btnSaveAsA.Enabled = true;
                }
                else
                {
                    _filePathB = filePath; _landbB = landb; _textsB = texts;
                    _isUnicodeB = isUnicode; _mapCreditsB = mapCredits; _isDirtyB = false;
                    PopulateGrid(_gridViewB, texts);
                    _lblFileInfoB.Text = $"{Path.GetFileName(filePath)} ({texts.Count} entries)" + (isUnicode ? " [U]" : "");
                    _btnSaveB.Enabled = _btnSaveAsB.Enabled = true;
                }
                Log($"Loaded ({side}): {Path.GetFileName(filePath)} - {texts.Count} entries");
            }
            catch (Exception ex) { Log($"ERROR loading ({side}): {ex.Message}"); }
        }

        private const int ROWS_PER_ENTRY = 5;

        private static void PopulateGrid(DataGridView grid, List<CommonText> texts)
        {
            grid.Rows.Clear();
            if (texts == null) return;
            foreach (var t in texts)
            {
                // DataGridView GDI+ renderer requires \r\n for line breaks.
                // Single \n renders as space.
                string orig = (t.actorSpeechOriginal ?? "").Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
                string trans = (t.actorSpeechTranslation ?? "").Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
                grid.Rows.Add("langid", t.strNumber.ToString());
                grid.Rows.Add("actor", t.actorName ?? "");
                grid.Rows.Add("speechOriginal", orig);
                grid.Rows.Add("speechTranslation", trans);
                grid.Rows.Add("flags", t.flags ?? "00000000");
            }
            // Colour odd/even entries for visual grouping
            for (int i = 0; i < texts.Count; i++)
            {
                var back = i % 2 == 0
                    ? System.Drawing.SystemColors.Window
                    : System.Drawing.Color.FromArgb(245, 248, 252);
                for (int r = 0; r < ROWS_PER_ENTRY; r++)
                {
                    var row = grid.Rows[i * ROWS_PER_ENTRY + r];
                    row.DefaultCellStyle.BackColor = back;
                    row.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
            }
            // Force auto-size so multi-line rows get correct height
            grid.AutoResizeRows();
        }

        // ========== Editing ==========

        private void OnCellChanged(char side)
        {
            if (side == 'A') _isDirtyA = true; else _isDirtyB = true;
            UpdateTitle();
        }

        private void OnCellValidating(char side, DataGridViewCellValidatingEventArgs e)
        {
            // flags is the 5th row (index 4) in each entry block
            if (e.ColumnIndex == 1 && e.RowIndex % ROWS_PER_ENTRY == 4)
            {
                string v = e.FormattedValue?.ToString() ?? "";
                if (v.Length > 8) { e.Cancel = true; Log($"ERROR ({side}): flags max 8 chars"); return; }
                foreach (char c in v) { if (c != '0' && c != '1') { e.Cancel = true; Log($"ERROR ({side}): flags 0/1 only"); return; } }
            }
        }

        // ========== Sync scroll ==========

        private void OnSyncScrollToggled(object sender, EventArgs e)
        {
            if (_syncScrollMenu.Checked)
            {
                _gridViewA.Scroll += OnGridAScroll;
                _gridViewB.Scroll += OnGridBScroll;
                _gridViewA.SelectionChanged += OnGridASelection;
                _gridViewB.SelectionChanged += OnGridBSelection;
            }
            else
            {
                _gridViewA.Scroll -= OnGridAScroll;
                _gridViewB.Scroll -= OnGridBScroll;
                _gridViewA.SelectionChanged -= OnGridASelection;
                _gridViewB.SelectionChanged -= OnGridBSelection;
            }
        }

        private void OnGridAScroll(object sender, ScrollEventArgs e)
        {
            if (_suppressScrollSync || !_syncScrollMenu.Checked || _gridViewB.RowCount == 0) return;
            int entry = _gridViewA.FirstDisplayedScrollingRowIndex / ROWS_PER_ENTRY;
            if (entry == _lastEntryA) return;
            _lastEntryA = entry;
            int target = entry * ROWS_PER_ENTRY;
            if (target >= _gridViewB.RowCount) return;
            _suppressScrollSync = true;
            try { _gridViewB.FirstDisplayedScrollingRowIndex = target; } catch { }
            finally { _suppressScrollSync = false; }
        }

        private void OnGridBScroll(object sender, ScrollEventArgs e)
        {
            if (_suppressScrollSync || !_syncScrollMenu.Checked || _gridViewA.RowCount == 0) return;
            int entry = _gridViewB.FirstDisplayedScrollingRowIndex / ROWS_PER_ENTRY;
            if (entry == _lastEntryB) return;
            _lastEntryB = entry;
            int target = entry * ROWS_PER_ENTRY;
            if (target >= _gridViewA.RowCount) return;
            _suppressScrollSync = true;
            try { _gridViewA.FirstDisplayedScrollingRowIndex = target; } catch { }
            finally { _suppressScrollSync = false; }
        }

        private void OnGridASelection(object sender, EventArgs e)
        {
            if (_suppressSelectionSync || !_syncScrollMenu.Checked) return;
            if (_gridViewA.SelectedRows.Count == 0) return;
            int idx = _gridViewA.SelectedRows[0].Index;
            int entryStart = (idx / ROWS_PER_ENTRY) * ROWS_PER_ENTRY;
            if (entryStart < _gridViewB.RowCount)
            {
                _suppressSelectionSync = true;
                try
                {
                    _gridViewB.ClearSelection();
                    for (int r = 0; r < ROWS_PER_ENTRY && entryStart + r < _gridViewB.RowCount; r++)
                        _gridViewB.Rows[entryStart + r].Selected = true;
                }
                finally { _suppressSelectionSync = false; }
            }
        }

        private void OnGridBSelection(object sender, EventArgs e)
        {
            if (_suppressSelectionSync || !_syncScrollMenu.Checked) return;
            if (_gridViewB.SelectedRows.Count == 0) return;
            int idx = _gridViewB.SelectedRows[0].Index;
            int entryStart = (idx / ROWS_PER_ENTRY) * ROWS_PER_ENTRY;
            if (entryStart < _gridViewA.RowCount)
            {
                _suppressSelectionSync = true;
                try
                {
                    _gridViewA.ClearSelection();
                    for (int r = 0; r < ROWS_PER_ENTRY && entryStart + r < _gridViewA.RowCount; r++)
                        _gridViewA.Rows[entryStart + r].Selected = true;
                }
                finally { _suppressSelectionSync = false; }
            }
        }

        // ========== Save ==========

        private void Save(char side)
        {
            if (side == 'A')
            {
                if (!_isDirtyA) { Log("Side A: no changes."); return; }
                DoSave(_filePathA, _filePathA, _landbA, _gridViewA, ref _textsA, ref _isDirtyA, _mapCreditsA, 'A');
            }
            else
            {
                if (!_isDirtyB) { Log("Side B: no changes."); return; }
                DoSave(_filePathB, _filePathB, _landbB, _gridViewB, ref _textsB, ref _isDirtyB, _mapCreditsB, 'B');
            }
        }

        private void SaveAs(char side)
        {
            using (var dlg = new SaveFileDialog { Filter = "Landb files (*.landb)|*.landb", DefaultExt = ".landb" })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    if (side == 'A') DoSave(_filePathA, dlg.FileName, _landbA, _gridViewA, ref _textsA, ref _isDirtyA, _mapCreditsA, 'A');
                    else DoSave(_filePathB, dlg.FileName, _landbB, _gridViewB, ref _textsB, ref _isDirtyB, _mapCreditsB, 'B');
                }
            }
        }

        private void DoSave(string origPath, string outPath, LandbClass landb, DataGridView grid,
            ref List<CommonText> texts, ref bool isDirty, bool mapCredits, char side)
        {
            try
            {
                texts = ReadTextsFromGrid(grid, texts);
                string result = LandbWorker.SaveLandbToFile(origPath, outPath, landb, texts, mapCredits);
                Log(result);
                if (!result.Contains("error") && !result.Contains("Error"))
                {
                    isDirty = false;
                    if (outPath != origPath) { if (side == 'A') _filePathA = outPath; else _filePathB = outPath; }
                    UpdateTitle();
                }
            }
            catch (Exception ex) { Log($"ERROR saving ({side}): {ex.Message}"); }
        }

        private static List<CommonText> ReadTextsFromGrid(DataGridView grid, List<CommonText> existing)
        {
            var result = new List<CommonText>();
            int count = Math.Min(grid.Rows.Count / ROWS_PER_ENTRY, existing?.Count ?? 0);
            for (int i = 0; i < count; i++)
            {
                var t = existing[i];
                int baseRow = i * ROWS_PER_ENTRY;
                // Grid stores \r\n for display; convert back to \n for saving
                string trans = (grid.Rows[baseRow + 3].Cells[1].Value?.ToString() ?? "").Replace("\r\n", "\n");
                t.actorSpeechTranslation = trans;
                t.flags = (grid.Rows[baseRow + 4].Cells[1].Value?.ToString() ?? "00000000");
                var sb = new StringBuilder();
                foreach (char c in t.flags) if (c == '0' || c == '1') sb.Append(c);
                t.flags = sb.ToString().PadLeft(8, '0');
                result.Add(t);
            }
            return result;
        }

        // ========== Character extraction ==========

        private void ExportAllChars(char side)
        {
            var texts = side == 'A' ? _textsA : _textsB;
            if (texts == null || texts.Count == 0)
            {
                Log($"Side {side}: no data loaded.");
                return;
            }

            using (var dlg = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                DefaultExt = ".txt",
                FileName = $"chars_side_{char.ToLower(side)}.txt"
            })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    var charSet = new HashSet<string>();
                    foreach (var t in texts)
                    {
                        if (string.IsNullOrEmpty(t.actorSpeechOriginal)) continue;
                        foreach (char c in t.actorSpeechOriginal)
                            charSet.Add(c.ToString());
                    }

                    // Sort: common chars first, then by codepoint
                    var sorted = charSet
                        .OrderBy(s => s.Length > 0 && char.IsLetterOrDigit(s[0]) ? 0 : 1)
                        .ThenBy(s => (int)(s.Length > 0 ? s[0] : 0))
                        .ToList();

                    using (var sw = new StreamWriter(dlg.FileName, false, new UTF8Encoding(true)))
                    {
                        foreach (var ch in sorted)
                            sw.Write(ch);
                    }

                    Log($"Side {side}: exported {charSet.Count} unique chars → {Path.GetFileName(dlg.FileName)}");
                }
                catch (Exception ex)
                {
                    Log($"ERROR exporting chars ({side}): {ex.Message}");
                }
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            var dirty = new List<string>();
            if (_isDirtyA) dirty.Add("Side A");
            if (_isDirtyB) dirty.Add("Side B");
            if (dirty.Count > 0)
            {
                var r = MessageBox.Show($"Unsaved: {string.Join(", ", dirty)}.\n\nSave before closing?", "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (r == DialogResult.Yes) { if (_isDirtyA) Save('A'); if (_isDirtyB) Save('B'); }
                else if (r == DialogResult.Cancel) { e.Cancel = true; }
            }
        }

        private void UpdateTitle()
        {
            string t = "Landb Editor";
            if (_isDirtyA) t += " [A*]";
            if (_isDirtyB) t += " [B*]";
            Text = t;
        }

        private void Log(string msg)
        {
            if (_txtLog.IsDisposed) return;
            _txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\r\n");
        }
    }
}
