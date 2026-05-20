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
        private BackgroundWorker _replaceWorker;
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
            _chkSpeechOriginal.Checked = true;
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

            _replaceWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = false
            };
            _replaceWorker.DoWork += ReplaceWorker_DoWork;
            _replaceWorker.ProgressChanged += ReplaceWorker_ProgressChanged;
            _replaceWorker.RunWorkerCompleted += ReplaceWorker_RunWorkerCompleted;
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
        private void OnClose(object sender, EventArgs e)
        {
            Hide();
            (Owner as Form)?.Activate();
        }

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
                else { Hide(); (Owner as Form)?.Activate(); }
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

            // Use deduplicated entry count (speechOriginal == speechTranslation for same position)
            int uniqueMatches = _allMatches
                .GroupBy(m => new { m.FilePath, m.EntryIndex, m.MatchPosition })
                .Count();
            int fileCount = _allMatches.Select(m => m.FilePath).Distinct().Count();
            string modeLabel = isReplace ? "replace preview" : "found";
            SetStatus($"{modeLabel}: {uniqueMatches} unique matches in {fileCount} file(s)");

            if (isReplace && uniqueMatches > 0)
                _btnApplyReplace.Enabled = true;
        }

        // ===== Results display =====

        private void PopulateResults(List<FindInFilesMatch> matches, bool replaceMode)
        {
            _listResults.BeginUpdate();
            _listResults.Items.Clear();

            var entryGroups = matches.GroupBy(m => new { m.RelativePath, m.EntryIndex });
            foreach (var g in entryGroups)
            {
                var first = g.First();
                string fields = string.Join(", ", g.Select(m => m.FieldName).Distinct());

                string preview;
                if (replaceMode && !string.IsNullOrEmpty(_txtReplace.Text))
                {
                    string before = Truncate(first.FullValue, 60);
                    // Apply ALL match positions (descending) to get the complete after text
                    string after = first.FullValue;
                    foreach (var m in g.Select(m => m.MatchPosition).Distinct().OrderByDescending(p => p))
                        after = after.Substring(0, m) + _txtReplace.Text + after.Substring(m + first.MatchLength);
                    preview = $"{Truncate(before, 50)}  →  {Truncate(after, 50)}";
                }
                else
                {
                    preview = Truncate(first.FullValue, 80);
                }

                var item = new ListViewItem(first.RelativePath);
                item.SubItems.Add((first.EntryIndex + 1).ToString());
                item.SubItems.Add(fields);
                item.SubItems.Add(preview);
                item.Tag = first;
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
            int uniqueMatches = _allMatches
                .GroupBy(m => new { m.FilePath, m.EntryIndex, m.MatchPosition })
                .Count();
            string confirmMsg = $"Replace {uniqueMatches} unique occurrence(s) in {filesToModify.Count} file(s)?\n\nThis will overwrite .landb files.";
            if (MessageBox.Show(this, confirmMsg, "Confirm Replace All",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                return;

            _btnApplyReplace.Enabled = _btnFindAll.Enabled = _btnReplacePreview.Enabled = false;

            // Mark all result items as pending
            foreach (ListViewItem item in _listResults.Items)
                item.BackColor = SystemColors.Window;

            _replaceWorker.RunWorkerAsync(new ReplaceArgs
            {
                FilesToModify = filesToModify,
                ReplaceText = replaceText,
                AllMatches = _allMatches
            });
        }

        private void ReplaceWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (ReplaceArgs)e.Argument;
            var results = new ReplaceResults();

            for (int i = 0; i < args.FilesToModify.Count; i++)
            {
                var group = args.FilesToModify[i];
                string filePath = group.Key;
                string fileName = Path.GetFileName(filePath);

                _replaceWorker.ReportProgress((i + 1) * 100 / args.FilesToModify.Count,
                    new ReplaceProgress { FileName = fileName, Phase = "writing" });

                try
                {
                    bool isUnicode, mapCredits; string errorMsg;
                    var landb = LandbWorker.LoadLandbFromFile(filePath, out isUnicode, out mapCredits, out errorMsg);
                    if (landb == null) { results.FailFiles++; continue; }

                    var texts = LandbWorker.LandbToCommonTextList(landb, mapCredits);
                    if (texts == null) { results.FailFiles++; continue; }

                    int fileReplaceCount = 0;

                    foreach (var entryGroup in group.GroupBy(m => m.EntryIndex))
                    {
                        int ei = entryGroup.Key;
                        if (ei >= texts.Count) continue;

                        var t = texts[ei];
                        string currentText = GetFieldValue(t, entryGroup.First().FieldName);
                        string fieldName = entryGroup.First().FieldName;

                        var uniquePositions = entryGroup
                            .Select(m => m.MatchPosition)
                            .Distinct()
                            .OrderByDescending(p => p);

                        foreach (int pos in uniquePositions)
                        {
                            int matchLen = entryGroup.First().MatchLength;
                            if (pos < 0 || pos + matchLen > currentText.Length) continue;

                            currentText = currentText.Substring(0, pos)
                                         + args.ReplaceText
                                         + currentText.Substring(pos + matchLen);
                            fileReplaceCount++;
                        }

                        if (fieldName == "speechOriginal" || fieldName == "speechTranslation")
                        {
                            t.actorSpeechOriginal = currentText;
                            t.actorSpeechTranslation = currentText;
                        }
                        else
                        {
                            SetFieldFromText(ref t, fieldName, currentText);
                        }
                        texts[ei] = t;
                    }

                    if (fileReplaceCount > 0)
                    {
                        string saveResult = LandbWorker.SaveLandbToFile(filePath, filePath, landb, texts, mapCredits);
                        bool saveFailed = saveResult.Contains("error") || saveResult.Contains("Error")
                                       || saveResult.Contains("don't know") || saveResult.Contains("Unknown error");

                        if (saveFailed)
                        {
                            results.FailFiles++;
                            OnLogMessage?.Invoke($"Replace ERROR: {fileName} - {saveResult}");
                            _replaceWorker.ReportProgress(0, new ReplaceProgress { FileName = fileName, Phase = "fail" });
                        }
                        else
                        {
                            results.SuccessFiles++;
                            results.TotalReplaced += fileReplaceCount;

                            char? side = OnFileNeedsRefresh?.Invoke(filePath, 'A') == true ? 'A' :
                                         OnFileNeedsRefresh?.Invoke(filePath, 'B') == true ? 'B' : (char?)null;
                            if (side.HasValue)
                                OnLogMessage?.Invoke($"Refreshed {fileName} (Side {side.Value})");
                            _replaceWorker.ReportProgress(0, new ReplaceProgress { FileName = fileName, Phase = "done" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    results.FailFiles++;
                    OnLogMessage?.Invoke($"Replace ERROR: {fileName} - {ex.Message}");
                    _replaceWorker.ReportProgress(0, new ReplaceProgress { FileName = fileName, Phase = "fail" });
                }
            }

            e.Result = results;
        }

        private void ReplaceWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var prog = e.UserState as ReplaceProgress;
            if (prog == null) return;

            SetStatus($"Writing: {prog.FileName}  ({e.ProgressPercentage}%)");

            // Highlight ALL items for this file
            foreach (ListViewItem item in _listResults.Items)
            {
                string itemFile = item.Text;
                if (itemFile == prog.FileName || itemFile.EndsWith("\\" + prog.FileName))
                {
                    switch (prog.Phase)
                    {
                        case "writing": item.BackColor = Color.LightGoldenrodYellow; break;
                        case "done": item.BackColor = Color.LightGreen; break;
                        case "fail": item.BackColor = Color.LightPink; break;
                    }
                }
            }
        }

        private void ReplaceWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _btnFindAll.Enabled = _btnReplacePreview.Enabled = true;
            _btnApplyReplace.Enabled = false;
            _allMatches = null;

            if (e.Error != null)
            {
                SetStatus($"Error: {e.Error.Message}", true);
                return;
            }

            var results = (ReplaceResults)e.Result;
            SetStatus($"Replaced: {results.TotalReplaced} in {results.SuccessFiles} file(s)" +
                      (results.FailFiles > 0 ? $", {results.FailFiles} failed" : ""), results.FailFiles > 0);

            OnLogMessage?.Invoke($"FindInFiles: replaced {results.TotalReplaced} occurrence(s) in {results.SuccessFiles} file(s)" +
                                 (results.FailFiles > 0 ? $", {results.FailFiles} failed" : ""));
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

        private static void SetFieldFromText(ref CommonText t, string field, string value)
        {
            switch (field)
            {
                case "speechTranslation": t.actorSpeechTranslation = value; break;
                case "speechOriginal": t.actorSpeechOriginal = value; break;
                case "actor": t.actorName = value; break;
                case "flags": t.flags = value; break;
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

        private class ReplaceArgs
        {
            public List<IGrouping<string, FindInFilesMatch>> FilesToModify;
            public string ReplaceText;
            public List<FindInFilesMatch> AllMatches;
        }

        private class ReplaceProgress
        {
            public string FileName;
            public string Phase; // "writing", "done", "fail"
        }

        private class ReplaceResults
        {
            public int SuccessFiles;
            public int FailFiles;
            public int TotalReplaced;
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
