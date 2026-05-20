using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TTG_Tools.ClassesStructs.Text;
using TTG_Tools.Texts;

namespace TTG_Tools
{
    /// <summary>
    /// Directory-level Find & Replace dialog for .landb files.
    /// Searches across all .landb files under a directory tree.
    /// </summary>
    public partial class FindInFilesDialog : Form
    {
        private BackgroundWorker _worker;
        private List<FindInFilesMatch> _allMatches;
        private string _currentRootDir;

        // Callbacks for refreshing editor state after replace
        internal Func<string, char, bool> OnFileNeedsRefresh;
        internal Action<string> OnLogMessage;

        public FindInFilesDialog()
        {
            InitializeComponent();
            InitWorker();
            _chkSpeechTranslation.Checked = true;
        }

        /// <summary>
        /// Opens/activates the dialog. Owner must be the parent LandbEditor form.
        /// </summary>
        public void Open(string findText, string rootDir, char side, IWin32Window owner)
        {
            if (!string.IsNullOrEmpty(findText))
                _txtFind.Text = findText;

            if (!string.IsNullOrEmpty(rootDir))
            {
                _txtDirectory.Text = rootDir;
                _currentRootDir = rootDir;
                _lblSideHint.Text = $"Side {side}";
            }

            if (!Visible)
                Show(owner);
            else
                Activate();

            _txtFind.Focus();
            _txtFind.SelectAll();
        }

        private void InitWorker()
        {
            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        // ===== Properties =====

        private bool MatchCase => _chkMatchCase.Checked;
        private bool WholeWord => _chkWholeWord.Checked;
        private bool IncludeSubdirs => _chkSubdirs.Checked;

        private string[] SearchFields
        {
            get
            {
                var fields = new List<string>();
                if (_chkSpeechTranslation.Checked) fields.Add("speechTranslation");
                if (_chkSpeechOriginal.Checked) fields.Add("speechOriginal");
                if (_chkActorName.Checked) fields.Add("actor");
                if (_chkFlags.Checked) fields.Add("flags");
                return fields.ToArray();
            }
        }

        private bool HasSearchFields =>
            _chkSpeechTranslation.Checked || _chkSpeechOriginal.Checked ||
            _chkActorName.Checked || _chkFlags.Checked;

        // ===== Button handlers =====

        private void OnFindAll(object sender, EventArgs e) => StartSearch(replaceMode: false);
        private void OnReplacePreview(object sender, EventArgs e) => StartSearch(replaceMode: true);
        private void OnApplyReplace(object sender, EventArgs e) => ExecuteReplace();
        private void OnClose(object sender, EventArgs e) => Hide();

        private void OnBrowseDir(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog { Description = "Select root directory containing .landb files" })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _txtDirectory.Text = dlg.SelectedPath;
                    _currentRootDir = dlg.SelectedPath;
                    _lblSideHint.Text = "manual";
                }
            }
        }

        private void OnResultDoubleClick(object sender, EventArgs e)
        {
            if (_listResults.SelectedItems.Count == 0) return;
            var match = _listResults.SelectedItems[0].Tag as FindInFilesMatch;
            if (match == null) return;
            NavigateToMatch(match);
        }

        private void OnResultKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _listResults.SelectedItems.Count > 0)
            {
                var match = _listResults.SelectedItems[0].Tag as FindInFilesMatch;
                if (match != null) NavigateToMatch(match);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                if (_worker.IsBusy) _worker.CancelAsync();
                else Hide();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (_worker.IsBusy)
                {
                    _worker.CancelAsync();
                    e.Cancel = true;
                    return;
                }
                e.Cancel = true;
                Hide();
            }
        }

        private void OnSearchOptionChanged(object sender, EventArgs e)
        {
            _btnApplyReplace.Enabled = false;
        }

        private void OnFindTextChanged(object sender, EventArgs e)
        {
            _btnApplyReplace.Enabled = false;
        }

        // ===== Search =====

        private void StartSearch(bool replaceMode)
        {
            string findText = _txtFind.Text;
            if (string.IsNullOrEmpty(findText)) { SetStatus("Enter text to find.", true); return; }
            if (!HasSearchFields) { SetStatus("Select at least one search field.", true); return; }

            string rootDir = _txtDirectory.Text.Trim();
            if (string.IsNullOrEmpty(rootDir) || !Directory.Exists(rootDir))
            {
                SetStatus("Directory not found.", true);
                return;
            }

            _currentRootDir = rootDir;

            _btnFindAll.Enabled = _btnReplacePreview.Enabled = _btnBrowseDir.Enabled = false;
            _btnApplyReplace.Enabled = false;
            SetStatus("Searching...");

            var args = new SearchArgs
            {
                RootDir = rootDir,
                FindText = findText,
                ReplaceText = replaceMode ? _txtReplace.Text : null,
                MatchCase = MatchCase,
                WholeWord = WholeWord,
                IncludeSubdirs = IncludeSubdirs,
                Fields = SearchFields,
                ReplaceMode = replaceMode
            };

            _allMatches = null;
            _listResults.Items.Clear();
            _worker.RunWorkerAsync(args);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (SearchArgs)e.Argument;
            var matches = new List<FindInFilesMatch>();

            string[] patterns = args.IncludeSubdirs
                ? new[] { "*.landb" }
                : null;

            var files = args.IncludeSubdirs
                ? Directory.GetFiles(args.RootDir, "*.landb", SearchOption.AllDirectories)
                : Directory.GetFiles(args.RootDir, "*.landb", SearchOption.TopDirectoryOnly);

            int total = files.Length;
            for (int i = 0; i < total; i++)
            {
                if (_worker.CancellationPending) { e.Cancel = true; return; }

                string file = files[i];
                string relPath = MakeRelativePath(args.RootDir, file);
                _worker.ReportProgress((i + 1) * 100 / total, $"{i + 1}/{total}  {relPath}");

                try
                {
                    bool isUnicode, mapCredits; string errorMsg;
                    var landb = LandbWorker.LoadLandbFromFile(file, out isUnicode, out mapCredits, out errorMsg);
                    if (landb == null) continue;

                    var texts = LandbWorker.LandbToCommonTextList(landb, mapCredits);
                    if (texts == null) continue;

                    for (int ei = 0; ei < texts.Count; ei++)
                    {
                        var t = texts[ei];
                        foreach (var field in args.Fields)
                        {
                            string value = GetFieldValue(t, field);
                            if (string.IsNullOrEmpty(value)) continue;

                            var positions = FindAllPositions(value, args.FindText, args.MatchCase, args.WholeWord);
                            foreach (int pos in positions)
                            {
                                matches.Add(new FindInFilesMatch
                                {
                                    FilePath = file,
                                    RelativePath = relPath,
                                    EntryIndex = ei,
                                    LangId = (int)t.strNumber,
                                    FieldName = field,
                                    FullValue = value,
                                    MatchPosition = pos,
                                    MatchLength = args.FindText.Length
                                });
                            }
                        }
                    }
                }
                catch
                {
                    // Skip files that can't be parsed
                }
            }

            e.Result = new SearchResult { Matches = matches, Args = args };
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var msg = e.UserState as string;
            SetStatus($"{msg ?? ""}  ({e.ProgressPercentage}%)");
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _btnFindAll.Enabled = _btnReplacePreview.Enabled = _btnBrowseDir.Enabled = true;

            if (e.Cancelled)
            {
                SetStatus("Cancelled.", true);
                return;
            }
            if (e.Error != null)
            {
                SetStatus($"Error: {e.Error.Message}", true);
                return;
            }

            var result = (SearchResult)e.Result;
            _allMatches = result.Matches;
            bool isReplace = result.Args.ReplaceMode;

            PopulateResults(_allMatches, isReplace);

            int fileCount = _allMatches.Select(m => m.FilePath).Distinct().Count();
            string modeLabel = isReplace ? "replace preview" : "found";
            SetStatus($"{modeLabel}: {_allMatches.Count} matches in {fileCount} file(s)");

            if (isReplace && _allMatches.Count > 0)
                _btnApplyReplace.Enabled = true;
        }

        // ===== Results display =====

        private void PopulateResults(List<FindInFilesMatch> matches, bool replaceMode)
        {
            _listResults.BeginUpdate();
            _listResults.Items.Clear();

            foreach (var m in matches)
            {
                string preview;
                if (replaceMode && !string.IsNullOrEmpty(_txtReplace.Text))
                {
                    string before = Truncate(m.FullValue, 60);
                    string after = Truncate(
                        m.FullValue.Substring(0, m.MatchPosition) +
                        _txtReplace.Text +
                        m.FullValue.Substring(m.MatchPosition + m.MatchLength), 60);
                    preview = $"{before}  →  {after}";
                }
                else
                {
                    preview = Truncate(m.FullValue, 80);
                }

                var item = new ListViewItem(m.RelativePath);
                item.SubItems.Add((m.EntryIndex + 1).ToString());
                item.SubItems.Add(m.FieldName);
                item.SubItems.Add(preview);
                item.Tag = m;
                _listResults.Items.Add(item);
            }

            _listResults.EndUpdate();
        }

        private static string Truncate(string s, int maxLen)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length <= maxLen ? s : s.Substring(0, maxLen - 3) + "...";
        }

        // ===== Replace execution =====

        private void ExecuteReplace()
        {
            if (_allMatches == null || _allMatches.Count == 0) return;
            string replaceText = _txtReplace.Text;

            var filesToModify = _allMatches.GroupBy(m => m.FilePath).ToList();
            string confirmMsg = $"Replace {_allMatches.Count} occurrence(s) in {filesToModify.Count} file(s)?\n\nThis will overwrite .landb files.";
            if (MessageBox.Show(this, confirmMsg, "Confirm Replace All",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                return;

            _btnApplyReplace.Enabled = _btnFindAll.Enabled = _btnReplacePreview.Enabled = false;

            int successFiles = 0, failFiles = 0;
            int totalReplaced = 0;

            foreach (var group in filesToModify)
            {
                string filePath = group.Key;
                try
                {
                    bool isUnicode, mapCredits; string errorMsg;
                    var landb = LandbWorker.LoadLandbFromFile(filePath, out isUnicode, out mapCredits, out errorMsg);
                    if (landb == null) { failFiles++; continue; }

                    var texts = LandbWorker.LandbToCommonTextList(landb, mapCredits);
                    if (texts == null) { failFiles++; continue; }

                    int fileReplaceCount = 0;
                    foreach (var match in group)
                    {
                        if (match.EntryIndex >= texts.Count) continue;
                        var t = texts[match.EntryIndex];
                        SetFieldValue(t, match.FieldName, match.FullValue, match.MatchPosition,
                            match.MatchLength, replaceText);
                        fileReplaceCount++;
                    }

                    if (fileReplaceCount > 0)
                    {
                        string saveResult = LandbWorker.SaveLandbToFile(filePath, filePath, landb, texts, mapCredits);
                        if (saveResult.Contains("error") || saveResult.Contains("Error") || saveResult.Contains("don't know"))
                        {
                            failFiles++;
                            OnLogMessage?.Invoke($"Replace ERROR: {Path.GetFileName(filePath)} - {saveResult}");
                        }
                        else
                        {
                            successFiles++;
                            totalReplaced += fileReplaceCount;

                            // Refresh editor if this file is open
                            char? side = OnFileNeedsRefresh?.Invoke(filePath, 'A') == true ? 'A' :
                                         OnFileNeedsRefresh?.Invoke(filePath, 'B') == true ? 'B' : (char?)null;
                            if (side.HasValue)
                                OnLogMessage?.Invoke($"Refreshed {Path.GetFileName(filePath)} (Side {side.Value})");
                        }
                    }
                }
                catch (Exception ex)
                {
                    failFiles++;
                    OnLogMessage?.Invoke($"Replace ERROR: {Path.GetFileName(filePath)} - {ex.Message}");
                }
            }

            SetStatus($"Replaced: {totalReplaced} in {successFiles} file(s)" +
                      (failFiles > 0 ? $", {failFiles} failed" : ""), failFiles > 0);

            _btnFindAll.Enabled = _btnReplacePreview.Enabled = true;
            _btnApplyReplace.Enabled = false;
            _allMatches = null;

            OnLogMessage?.Invoke($"FindInFiles: replaced {totalReplaced} occurrence(s) in {successFiles} file(s)" +
                                 (failFiles > 0 ? $", {failFiles} failed" : ""));
        }

        // ===== Navigation =====

        private void NavigateToMatch(FindInFilesMatch match)
        {
            var editor = Owner as LandbEditor;
            if (editor == null) return;

            // Determine which side has this file's parent directory
            editor.NavigateToFileAndEntry(match.FilePath, match.EntryIndex, match.FieldName);
        }

        // ===== Helpers =====

        private static string GetFieldValue(CommonText t, string field)
        {
            switch (field)
            {
                case "speechTranslation": return t.actorSpeechTranslation ?? "";
                case "speechOriginal": return t.actorSpeechOriginal ?? "";
                case "actor": return t.actorName ?? "";
                case "flags": return t.flags ?? "";
                default: return "";
            }
        }

        private static void SetFieldValue(CommonText t, string field, string fullValue,
            int matchPos, int matchLen, string replaceText)
        {
            string newValue = fullValue.Substring(0, matchPos) + replaceText +
                              fullValue.Substring(matchPos + matchLen);

            switch (field)
            {
                case "speechTranslation": t.actorSpeechTranslation = newValue; break;
                case "speechOriginal": t.actorSpeechOriginal = newValue; break;
                case "actor": t.actorName = newValue; break;
                case "flags": t.flags = newValue; break;
            }
        }

        private static List<int> FindAllPositions(string text, string search, bool matchCase, bool wholeWord)
        {
            var positions = new List<int>();
            if (string.IsNullOrEmpty(search)) return positions;

            StringComparison cmp = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            int idx = 0;
            while (idx < text.Length)
            {
                int found = text.IndexOf(search, idx, cmp);
                if (found < 0) break;

                if (wholeWord)
                {
                    bool leftBoundary = found == 0 || !char.IsLetterOrDigit(text[found - 1]);
                    bool rightBoundary = found + search.Length >= text.Length ||
                                         !char.IsLetterOrDigit(text[found + search.Length]);
                    if (leftBoundary && rightBoundary)
                        positions.Add(found);
                }
                else
                {
                    positions.Add(found);
                }
                idx = found + 1;
            }
            return positions;
        }

        private static string MakeRelativePath(string basePath, string fullPath)
        {
            if (string.IsNullOrEmpty(basePath)) return Path.GetFileName(fullPath);
            if (fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                string rel = fullPath.Substring(basePath.Length).TrimStart('\\', '/');
                return string.IsNullOrEmpty(rel) ? Path.GetFileName(fullPath) : rel;
            }
            return Path.GetFileName(fullPath);
        }

        private void SetStatus(string message, bool isError = false)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetStatus(message, isError)));
                return;
            }
            _lblStatus.Text = message;
            _lblStatus.ForeColor = isError ? Color.Red : SystemColors.ControlText;
        }

        // ===== Internal types =====

        private class SearchArgs
        {
            public string RootDir;
            public string FindText;
            public string ReplaceText;
            public bool MatchCase;
            public bool WholeWord;
            public bool IncludeSubdirs;
            public string[] Fields;
            public bool ReplaceMode; // true = replace preview, false = find only
        }

        private class SearchResult
        {
            public List<FindInFilesMatch> Matches;
            public SearchArgs Args;
        }
    }

    /// <summary>
    /// Represents a single match found during directory-level search.
    /// </summary>
    public class FindInFilesMatch
    {
        public string FilePath;
        public string RelativePath;
        public int EntryIndex;
        public int LangId;
        public string FieldName;
        public string FullValue;
        public int MatchPosition;
        public int MatchLength;
    }
}
