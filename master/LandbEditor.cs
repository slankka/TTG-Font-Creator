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

        // Find/Replace state
        private FindReplaceDialog _findReplaceDlg;
        private FindInFilesDialog _findInFilesDlg;
        private int _lastSearchRowA = -1;
        private int _lastSearchRowB = -1;
        private string _lastSearchTextA = "";
        private string _lastSearchTextB = "";

        /// <summary>Static reference for child dialogs (e.g. FindReplaceDialog).</summary>
        internal static LandbEditor ActiveInstance { get; private set; }

        public LandbEditor()
        {
            InitializeComponent();
            ActiveInstance = this;
            InitFindReplace();
            HookSyncScroll();
            HookEditingControls();
            RestoreLastDirectories();
        }

        private void InitFindReplace()
        {
            _findReplaceDlg = new FindReplaceDialog();
            _findReplaceDlg.FindNextClicked += OnFindNextClicked;
            _findReplaceDlg.ReplaceClicked += OnReplaceClicked;
            _findReplaceDlg.ReplaceAllClicked += OnReplaceAllClicked;

            _findInFilesDlg = new FindInFilesDialog();
            _findInFilesDlg.OnFileNeedsRefresh = IsFileOpenInEditor;
            _findInFilesDlg.OnLogMessage = Log;
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
            else if ((e.Control && e.KeyCode == Keys.F) || (e.Control && e.KeyCode == Keys.H))
            {
                e.SuppressKeyPress = true;
                if (e.Shift) OpenFindInFiles();
                else OpenFindReplace();
            }
            else if (e.KeyCode == Keys.F3)
            {
                e.SuppressKeyPress = true;
                FindNext();
            }
        }

        // ========== Find / Replace ==========

        /// <summary>Which side is currently active (has focus in its grid).</summary>
        private char ActiveSide
        {
            get
            {
                if (_gridViewA.ContainsFocus) return 'A';
                if (_gridViewB.ContainsFocus) return 'B';
                return 'A'; // default
            }
        }

        /// <summary>Gets the grid and last-search state for a side.</summary>
        private void GetSearchState(char side, out DataGridView grid, out int lastRow, out string lastText)
        {
            if (side == 'A')
            {
                grid = _gridViewA; lastRow = _lastSearchRowA; lastText = _lastSearchTextA;
            }
            else
            {
                grid = _gridViewB; lastRow = _lastSearchRowB; lastText = _lastSearchTextB;
            }
        }

        private void SetSearchState(char side, int lastRow, string lastText)
        {
            if (side == 'A') { _lastSearchRowA = lastRow; _lastSearchTextA = lastText; }
            else { _lastSearchRowB = lastRow; _lastSearchTextB = lastText; }
        }

        private void OpenFindReplace()
        {
            char activeSide = ActiveSide;
            string selectedText = GetSelectedText(activeSide);
            _findReplaceDlg.Open(selectedText ?? "", activeSide);

            // Position the dialog to the right of the parent form
            if (!_findReplaceDlg.Visible || _findReplaceDlg.Location.IsEmpty)
            {
                _findReplaceDlg.StartPosition = FormStartPosition.Manual;
                _findReplaceDlg.Location = new System.Drawing.Point(
                    this.Location.X + this.Width - _findReplaceDlg.Width - 50,
                    this.Location.Y + 80);
            }
        }

        private string GetSelectedText(char side)
        {
            var grid = side == 'A' ? _gridViewA : _gridViewB;
            if (grid?.CurrentCell != null)
            {
                string value = grid.CurrentCell.Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(value))
                {
                    // If a portion is selected, return that; otherwise return whole cell
                    if (grid.EditingControl is TextBox tb && !string.IsNullOrEmpty(tb.SelectedText))
                        return tb.SelectedText;
                }
            }
            return "";
        }

        // ---- Menu / button handlers ----

        private void OnFindOpen(object sender, EventArgs e) => OpenFindReplace();
        private void OnFindNextMenu(object sender, EventArgs e) => FindNext();

        private void OnFindInFiles(object sender, EventArgs e) => OpenFindInFiles();

        // ---- Dialog event handlers ----

        private void OnFindNextClicked(object sender, EventArgs e)
        {
            char side = _findReplaceDlg.Side;
            GetSearchState(side, out var grid, out int lastRow, out _);
            int startRow = (lastRow >= 0 && lastRow < grid.Rows.Count) ? lastRow + 1 : 0;
            int found = FindInGrid(grid, _findReplaceDlg.FindText, startRow, _findReplaceDlg.MatchCase);
            if (found >= 0)
            {
                SetSearchState(side, found, _findReplaceDlg.FindText);
                SelectCell(grid, found);
                _findReplaceDlg.ShowStatus($"Found at row {found}");
            }
            else
            {
                _findReplaceDlg.ShowStatus("Not found", isError: true);
            }
        }

        private void OnReplaceClicked(object sender, EventArgs e)
        {
            char side = _findReplaceDlg.Side;
            GetSearchState(side, out var grid, out int lastRow, out _);
            string findText = _findReplaceDlg.FindText;
            string replaceText = _findReplaceDlg.ReplaceText;
            bool matchCase = _findReplaceDlg.MatchCase;

            if (string.IsNullOrEmpty(findText)) return;

            // If we have a current match, replace it first
            if (lastRow >= 0 && lastRow < grid.Rows.Count)
            {
                string currentVal = grid.Rows[lastRow].Cells[1].Value?.ToString() ?? "";
                StringComparison cmp = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                if (currentVal.IndexOf(findText, cmp) >= 0)
                {
                    string newVal = ReplaceFirst(currentVal, findText, replaceText, cmp);
                    grid.Rows[lastRow].Cells[1].Value = newVal;
                    OnCellChanged(side);
                }
            }

            // Then find next
            int startRow = (lastRow >= 0 && lastRow < grid.Rows.Count) ? lastRow + 1 : 0;
            int found = FindInGrid(grid, findText, startRow, matchCase);
            if (found >= 0)
            {
                SetSearchState(side, found, findText);
                SelectCell(grid, found);
                _findReplaceDlg.ShowStatus($"Replaced, next at row {found}");
            }
            else
            {
                SetSearchState(side, -1, findText);
                _findReplaceDlg.ShowStatus("Replaced, no more matches", isError: true);
            }
        }

        private void OnReplaceAllClicked(object sender, EventArgs e)
        {
            char side = _findReplaceDlg.Side;
            GetSearchState(side, out var grid, out _, out _);
            string findText = _findReplaceDlg.FindText;
            string replaceText = _findReplaceDlg.ReplaceText;
            bool matchCase = _findReplaceDlg.MatchCase;

            if (string.IsNullOrEmpty(findText))
            {
                _findReplaceDlg.ShowStatus("Nothing to find", isError: true);
                return;
            }

            StringComparison cmp = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            int count = 0;
            for (int r = 0; r < grid.Rows.Count; r++)
            {
                string value = grid.Rows[r].Cells[1].Value?.ToString() ?? "";
                if (value.IndexOf(findText, cmp) >= 0)
                {
                    string newVal = value.Replace(findText, replaceText);
                    // Respect match case for replacement
                    if (!matchCase)
                    {
                        // Use case-insensitive replace
                        newVal = ReplaceAllIgnoreCase(value, findText, replaceText);
                    }
                    grid.Rows[r].Cells[1].Value = newVal;
                    count++;
                }
            }

            if (count > 0)
            {
                OnCellChanged(side);
                SetSearchState(side, -1, findText);
                _findReplaceDlg.ShowStatus($"Replaced {count} occurrence(s)");
                Log($"Side {side}: Replaced All - {count} occurrence(s) of \"{findText}\"");
            }
            else
            {
                _findReplaceDlg.ShowStatus("No matches found", isError: true);
            }
        }

        // ========== Core find logic ==========

        /// <summary>Performs Find Next using the last search text from the dialog (for F3).</summary>
        private void FindNext()
        {
            if (_findReplaceDlg == null || string.IsNullOrEmpty(_findReplaceDlg.FindText))
            {
                // No previous search; open Find dialog
                OpenFindReplace();
                return;
            }

            char side = _findReplaceDlg.Side;
            GetSearchState(side, out var grid, out int lastRow, out string lastText);

            // If search text changed, reset
            if (lastText != _findReplaceDlg.FindText)
            {
                lastRow = -1;
            }

            int startRow = (lastRow >= 0 && lastRow < grid.Rows.Count) ? lastRow + 1 : 0;
            int found = FindInGrid(grid, _findReplaceDlg.FindText, startRow, _findReplaceDlg.MatchCase);
            if (found >= 0)
            {
                SetSearchState(side, found, _findReplaceDlg.FindText);
                SelectCell(grid, found);
                _findReplaceDlg.ShowStatus($"Found at row {found}");
            }
            else
            {
                // Wrap-around: try from beginning
                if (startRow > 0)
                {
                    found = FindInGrid(grid, _findReplaceDlg.FindText, 0, _findReplaceDlg.MatchCase);
                    if (found >= 0)
                    {
                        SetSearchState(side, found, _findReplaceDlg.FindText);
                        SelectCell(grid, found);
                        _findReplaceDlg.ShowStatus($"Wrapped - found at row {found}");
                        return;
                    }
                }
                _findReplaceDlg.ShowStatus("Not found", isError: true);
            }
        }

        /// <summary>Searches the Value column (index 1) of a DataGridView for text.</summary>
        /// <returns>The row index of the first match, or -1 if not found.</returns>
        private static int FindInGrid(DataGridView grid, string searchText, int startRow, bool matchCase)
        {
            if (grid == null || grid.Rows.Count == 0 || string.IsNullOrEmpty(searchText))
                return -1;

            StringComparison cmp = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            int totalRows = grid.Rows.Count;

            // Clamp start row
            if (startRow < 0) startRow = 0;
            if (startRow >= totalRows) startRow = 0;

            // Search from startRow to end
            for (int r = startRow; r < totalRows; r++)
            {
                string value = grid.Rows[r].Cells[1].Value?.ToString() ?? "";
                if (value.IndexOf(searchText, cmp) >= 0)
                    return r;
            }

            // Wrap around: search from 0 to startRow - 1
            for (int r = 0; r < startRow; r++)
            {
                string value = grid.Rows[r].Cells[1].Value?.ToString() ?? "";
                if (value.IndexOf(searchText, cmp) >= 0)
                    return r;
            }

            return -1;
        }

        /// <summary>Selects a cell in the grid and scrolls to it.</summary>
        private static void SelectCell(DataGridView grid, int rowIndex)
        {
            if (grid == null || rowIndex < 0 || rowIndex >= grid.Rows.Count) return;

            grid.ClearSelection();
            grid.CurrentCell = grid.Rows[rowIndex].Cells[1];

            // Ensure the row is visible
            if (rowIndex < grid.FirstDisplayedScrollingRowIndex ||
                rowIndex >= grid.FirstDisplayedScrollingRowIndex + grid.DisplayedRowCount(false))
            {
                grid.FirstDisplayedScrollingRowIndex = Math.Max(0, rowIndex - 5);
            }
        }

        /// <summary>Replaces the first occurrence of oldValue with newValue in text.</summary>
        private static string ReplaceFirst(string text, string oldValue, string newValue, StringComparison cmp)
        {
            int idx = text.IndexOf(oldValue, cmp);
            if (idx < 0) return text;
            return text.Substring(0, idx) + newValue + text.Substring(idx + oldValue.Length);
        }

        /// <summary>Case-insensitive replace all.</summary>
        private static string ReplaceAllIgnoreCase(string text, string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(oldValue)) return text;

            var sb = new System.Text.StringBuilder();
            int pos = 0;
            while (pos < text.Length)
            {
                int idx = text.IndexOf(oldValue, pos, StringComparison.OrdinalIgnoreCase);
                if (idx < 0)
                {
                    sb.Append(text.Substring(pos));
                    break;
                }
                sb.Append(text.Substring(pos, idx - pos));
                sb.Append(newValue);
                pos = idx + oldValue.Length;
            }
            return sb.ToString();
        }

        // ========== Find in Files ==========

        private void OpenFindInFiles()
        {
            char side = ActiveSide;
            string dir = GetCurrentDirectory(side);
            string selectedText = GetSelectedText(side);

            _findInFilesDlg.Open(selectedText, dir, side, this);

            // Position dialog relative to parent
            if (!_findInFilesDlg.Visible)
            {
                _findInFilesDlg.StartPosition = FormStartPosition.Manual;
                _findInFilesDlg.Location = new System.Drawing.Point(
                    this.Location.X + (this.Width - _findInFilesDlg.Width) / 2,
                    this.Location.Y + (this.Height - _findInFilesDlg.Height) / 2);
            }
        }

        private string GetCurrentDirectory(char side)
        {
            string path = side == 'A' ? _txtPathA.Text : _txtPathB.Text;
            if (!string.IsNullOrEmpty(path))
            {
                if (Directory.Exists(path)) return path;
                if (File.Exists(path)) return Path.GetDirectoryName(path);
            }
            return "";
        }

        /// <summary>
        /// Called by FindInFilesDialog to check if a file is currently open in the editor.
        /// Returns true if the file is loaded on the given side.
        /// </summary>
        private bool IsFileOpenInEditor(string filePath, char side)
        {
            string currentPath = side == 'A' ? _filePathA : _filePathB;
            return !string.IsNullOrEmpty(currentPath) &&
                   string.Equals(currentPath, filePath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Navigates the editor to a specific file and entry, loading the file if needed.
        /// Called when user double-clicks a result in FindInFilesDialog.
        /// </summary>
        internal void NavigateToFileAndEntry(string filePath, int entryIndex, string fieldName)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            // Determine which side to use: try to match directory, fall back to Side A
            char side;
            string dir = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(_txtPathA.Text) &&
                dir.StartsWith(_txtPathA.Text, StringComparison.OrdinalIgnoreCase))
                side = 'A';
            else if (!string.IsNullOrEmpty(_txtPathB.Text) &&
                dir.StartsWith(_txtPathB.Text, StringComparison.OrdinalIgnoreCase))
                side = 'B';
            else
                side = 'A';

            // Show the directory in the tree
            string parentDir = Path.GetDirectoryName(filePath);
            if (side == 'A' && parentDir != _txtPathA.Text)
            {
                _txtPathA.Text = parentDir;
                RefreshTree(_treeViewA, parentDir);
            }
            else if (side == 'B' && parentDir != _txtPathB.Text)
            {
                _txtPathB.Text = parentDir;
                RefreshTree(_treeViewB, parentDir);
            }

            // Load the file
            LoadLandbToSide(side, filePath);

            // Navigate to the specific entry
            var grid = side == 'A' ? _gridViewA : _gridViewB;
            if (grid == null || grid.Rows.Count == 0) return;

            int targetRow = entryIndex * ROWS_PER_ENTRY;
            // Determine which sub-row (field) within the entry
            int fieldOffset;
            switch (fieldName)
            {
                case "actor": fieldOffset = 1; break;
                case "speechOriginal": fieldOffset = 2; break;
                case "speechTranslation": fieldOffset = 3; break;
                case "flags": fieldOffset = 4; break;
                default: fieldOffset = 0; break;
            }
            int rowIndex = targetRow + fieldOffset;
            if (rowIndex >= 0 && rowIndex < grid.Rows.Count)
            {
                SelectCell(grid, rowIndex);
                Log($"Navigated to {Path.GetFileName(filePath)} entry {entryIndex + 1} ({fieldName})");
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
                else if (r == DialogResult.Cancel) { e.Cancel = true; return; }
            }
            // Clean up modeless dialogs
            if (_findReplaceDlg != null && !_findReplaceDlg.IsDisposed)
            {
                _findReplaceDlg.Close();
                _findReplaceDlg.Dispose();
                _findReplaceDlg = null;
            }
            if (_findInFilesDlg != null && !_findInFilesDlg.IsDisposed)
            {
                _findInFilesDlg.Close();
                _findInFilesDlg.Dispose();
                _findInFilesDlg = null;
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
