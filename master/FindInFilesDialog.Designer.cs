namespace TTG_Tools
{
    partial class FindInFilesDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this._lblFind = new System.Windows.Forms.Label();
            this._txtFind = new System.Windows.Forms.TextBox();
            this._lblReplace = new System.Windows.Forms.Label();
            this._txtReplace = new System.Windows.Forms.TextBox();
            this._chkMatchCase = new System.Windows.Forms.CheckBox();
            this._chkWholeWord = new System.Windows.Forms.CheckBox();
            this._grpDirectory = new System.Windows.Forms.GroupBox();
            this._lblSideHint = new System.Windows.Forms.Label();
            this._btnBrowseDir = new System.Windows.Forms.Button();
            this._txtDirectory = new System.Windows.Forms.TextBox();
            this._chkSubdirs = new System.Windows.Forms.CheckBox();
            this._grpFields = new System.Windows.Forms.GroupBox();
            this._chkFlags = new System.Windows.Forms.CheckBox();
            this._chkActorName = new System.Windows.Forms.CheckBox();
            this._chkSpeechOriginal = new System.Windows.Forms.CheckBox();
            this._chkSpeechTranslation = new System.Windows.Forms.CheckBox();
            this._btnFindAll = new System.Windows.Forms.Button();
            this._btnReplacePreview = new System.Windows.Forms.Button();
            this._btnApplyReplace = new System.Windows.Forms.Button();
            this._btnClose = new System.Windows.Forms.Button();
            this._lblStatus = new System.Windows.Forms.Label();
            this._listResults = new System.Windows.Forms.ListView();
            this._colFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colEntry = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colField = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colMatch = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));

            this._grpDirectory.SuspendLayout();
            this._grpFields.SuspendLayout();
            this.SuspendLayout();

            // _lblFind
            this._lblFind.AutoSize = true;
            this._lblFind.Location = new System.Drawing.Point(12, 15);
            this._lblFind.Name = "_lblFind";
            this._lblFind.Size = new System.Drawing.Size(56, 13);
            this._lblFind.TabIndex = 0;
            this._lblFind.Text = "Find what:";

            // _txtFind
            this._txtFind.Location = new System.Drawing.Point(110, 12);
            this._txtFind.Name = "_txtFind";
            this._txtFind.Size = new System.Drawing.Size(320, 20);
            this._txtFind.TabIndex = 1;
            this._txtFind.TextChanged += new System.EventHandler(this.OnFindTextChanged);

            // _lblReplace
            this._lblReplace.AutoSize = true;
            this._lblReplace.Location = new System.Drawing.Point(12, 41);
            this._lblReplace.Name = "_lblReplace";
            this._lblReplace.Size = new System.Drawing.Size(82, 13);
            this._lblReplace.TabIndex = 2;
            this._lblReplace.Text = "Replace with:";

            // _txtReplace
            this._txtReplace.Location = new System.Drawing.Point(110, 38);
            this._txtReplace.Name = "_txtReplace";
            this._txtReplace.Size = new System.Drawing.Size(320, 20);
            this._txtReplace.TabIndex = 3;

            // _chkMatchCase
            this._chkMatchCase.AutoSize = true;
            this._chkMatchCase.Location = new System.Drawing.Point(15, 68);
            this._chkMatchCase.Name = "_chkMatchCase";
            this._chkMatchCase.Size = new System.Drawing.Size(82, 17);
            this._chkMatchCase.TabIndex = 4;
            this._chkMatchCase.Text = "Match case";
            this._chkMatchCase.CheckedChanged += new System.EventHandler(this.OnSearchOptionChanged);

            // _chkWholeWord
            this._chkWholeWord.AutoSize = true;
            this._chkWholeWord.Location = new System.Drawing.Point(110, 68);
            this._chkWholeWord.Name = "_chkWholeWord";
            this._chkWholeWord.Size = new System.Drawing.Size(85, 17);
            this._chkWholeWord.TabIndex = 5;
            this._chkWholeWord.Text = "Whole word";
            this._chkWholeWord.CheckedChanged += new System.EventHandler(this.OnSearchOptionChanged);

            // _grpDirectory
            this._grpDirectory.Controls.Add(this._lblSideHint);
            this._grpDirectory.Controls.Add(this._btnBrowseDir);
            this._grpDirectory.Controls.Add(this._txtDirectory);
            this._grpDirectory.Controls.Add(this._chkSubdirs);
            this._grpDirectory.Location = new System.Drawing.Point(15, 92);
            this._grpDirectory.Name = "_grpDirectory";
            this._grpDirectory.Size = new System.Drawing.Size(415, 65);
            this._grpDirectory.TabIndex = 6;
            this._grpDirectory.TabStop = false;
            this._grpDirectory.Text = "Directory";

            // _lblSideHint
            this._lblSideHint.AutoSize = true;
            this._lblSideHint.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this._lblSideHint.Location = new System.Drawing.Point(308, 43);
            this._lblSideHint.Name = "_lblSideHint";
            this._lblSideHint.Size = new System.Drawing.Size(40, 13);
            this._lblSideHint.TabIndex = 3;
            this._lblSideHint.Text = "Side ?";

            // _btnBrowseDir
            this._btnBrowseDir.Location = new System.Drawing.Point(330, 16);
            this._btnBrowseDir.Name = "_btnBrowseDir";
            this._btnBrowseDir.Size = new System.Drawing.Size(75, 23);
            this._btnBrowseDir.TabIndex = 2;
            this._btnBrowseDir.Text = "Browse...";
            this._btnBrowseDir.UseVisualStyleBackColor = true;
            this._btnBrowseDir.Click += new System.EventHandler(this.OnBrowseDir);

            // _txtDirectory
            this._txtDirectory.Location = new System.Drawing.Point(10, 18);
            this._txtDirectory.Name = "_txtDirectory";
            this._txtDirectory.ReadOnly = true;
            this._txtDirectory.Size = new System.Drawing.Size(314, 20);
            this._txtDirectory.TabIndex = 1;
            this._txtDirectory.TextChanged += new System.EventHandler(this.OnSearchOptionChanged);

            // _chkSubdirs
            this._chkSubdirs.AutoSize = true;
            this._chkSubdirs.Checked = true;
            this._chkSubdirs.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chkSubdirs.Location = new System.Drawing.Point(10, 44);
            this._chkSubdirs.Name = "_chkSubdirs";
            this._chkSubdirs.Size = new System.Drawing.Size(136, 17);
            this._chkSubdirs.TabIndex = 0;
            this._chkSubdirs.Text = "Include subdirectories";
            this._chkSubdirs.UseVisualStyleBackColor = true;
            this._chkSubdirs.CheckedChanged += new System.EventHandler(this.OnSearchOptionChanged);

            // _grpFields
            this._grpFields.Controls.Add(this._chkFlags);
            this._grpFields.Controls.Add(this._chkActorName);
            this._grpFields.Controls.Add(this._chkSpeechOriginal);
            this._grpFields.Controls.Add(this._chkSpeechTranslation);
            this._grpFields.Location = new System.Drawing.Point(440, 12);
            this._grpFields.Name = "_grpFields";
            this._grpFields.Size = new System.Drawing.Size(170, 145);
            this._grpFields.TabIndex = 7;
            this._grpFields.TabStop = false;
            this._grpFields.Text = "Search in fields";

            // _chkSpeechTranslation
            this._chkSpeechTranslation.AutoSize = true;
            this._chkSpeechTranslation.Location = new System.Drawing.Point(10, 20);
            this._chkSpeechTranslation.Name = "_chkSpeechTranslation";
            this._chkSpeechTranslation.Size = new System.Drawing.Size(117, 17);
            this._chkSpeechTranslation.TabIndex = 0;
            this._chkSpeechTranslation.Text = "speechTranslation";
            this._chkSpeechTranslation.UseVisualStyleBackColor = true;
            this._chkSpeechTranslation.CheckedChanged += new System.EventHandler(this.OnSearchOptionChanged);

            // _chkSpeechOriginal
            this._chkSpeechOriginal.AutoSize = true;
            this._chkSpeechOriginal.Location = new System.Drawing.Point(10, 43);
            this._chkSpeechOriginal.Name = "_chkSpeechOriginal";
            this._chkSpeechOriginal.Size = new System.Drawing.Size(101, 17);
            this._chkSpeechOriginal.TabIndex = 1;
            this._chkSpeechOriginal.Text = "speechOriginal";
            this._chkSpeechOriginal.UseVisualStyleBackColor = true;
            this._chkSpeechOriginal.CheckedChanged += new System.EventHandler(this.OnSearchOptionChanged);

            // _chkActorName
            this._chkActorName.AutoSize = true;
            this._chkActorName.Location = new System.Drawing.Point(10, 66);
            this._chkActorName.Name = "_chkActorName";
            this._chkActorName.Size = new System.Drawing.Size(81, 17);
            this._chkActorName.TabIndex = 2;
            this._chkActorName.Text = "actorName";
            this._chkActorName.UseVisualStyleBackColor = true;
            this._chkActorName.CheckedChanged += new System.EventHandler(this.OnSearchOptionChanged);

            // _chkFlags
            this._chkFlags.AutoSize = true;
            this._chkFlags.Location = new System.Drawing.Point(10, 89);
            this._chkFlags.Name = "_chkFlags";
            this._chkFlags.Size = new System.Drawing.Size(50, 17);
            this._chkFlags.TabIndex = 3;
            this._chkFlags.Text = "flags";
            this._chkFlags.UseVisualStyleBackColor = true;
            this._chkFlags.CheckedChanged += new System.EventHandler(this.OnSearchOptionChanged);

            // _btnFindAll
            this._btnFindAll.Location = new System.Drawing.Point(15, 165);
            this._btnFindAll.Name = "_btnFindAll";
            this._btnFindAll.Size = new System.Drawing.Size(90, 23);
            this._btnFindAll.TabIndex = 8;
            this._btnFindAll.Text = "Find All";
            this._btnFindAll.UseVisualStyleBackColor = true;
            this._btnFindAll.Click += new System.EventHandler(this.OnFindAll);

            // _btnReplacePreview
            this._btnReplacePreview.Location = new System.Drawing.Point(111, 165);
            this._btnReplacePreview.Name = "_btnReplacePreview";
            this._btnReplacePreview.Size = new System.Drawing.Size(90, 23);
            this._btnReplacePreview.TabIndex = 9;
            this._btnReplacePreview.Text = "Preview";
            this._btnReplacePreview.UseVisualStyleBackColor = true;
            this._btnReplacePreview.Click += new System.EventHandler(this.OnReplacePreview);

            // _btnApplyReplace
            this._btnApplyReplace.Enabled = false;
            this._btnApplyReplace.Location = new System.Drawing.Point(207, 165);
            this._btnApplyReplace.Name = "_btnApplyReplace";
            this._btnApplyReplace.Size = new System.Drawing.Size(90, 23);
            this._btnApplyReplace.TabIndex = 10;
            this._btnApplyReplace.Text = "Apply Replace";
            this._btnApplyReplace.UseVisualStyleBackColor = true;
            this._btnApplyReplace.Click += new System.EventHandler(this.OnApplyReplace);

            // _btnClose
            this._btnClose.Location = new System.Drawing.Point(520, 163);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(90, 23);
            this._btnClose.TabIndex = 11;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = true;
            this._btnClose.Click += new System.EventHandler(this.OnClose);

            // _lblStatus
            this._lblStatus.AutoSize = true;
            this._lblStatus.Location = new System.Drawing.Point(12, 200);
            this._lblStatus.Name = "_lblStatus";
            this._lblStatus.Size = new System.Drawing.Size(85, 13);
            this._lblStatus.TabIndex = 12;
            this._lblStatus.Text = "Ready. Ctrl+F to fill from editor.";

            // _listResults
            this._listResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._listResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this._colFile, this._colEntry, this._colField, this._colMatch});
            this._listResults.FullRowSelect = true;
            this._listResults.GridLines = true;
            this._listResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this._listResults.HideSelection = false;
            this._listResults.Location = new System.Drawing.Point(12, 220);
            this._listResults.MultiSelect = false;
            this._listResults.Name = "_listResults";
            this._listResults.Size = new System.Drawing.Size(598, 330);
            this._listResults.TabIndex = 13;
            this._listResults.UseCompatibleStateImageBehavior = false;
            this._listResults.View = System.Windows.Forms.View.Details;
            this._listResults.DoubleClick += new System.EventHandler(this.OnResultDoubleClick);
            this._listResults.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnResultKeyDown);

            // _colFile
            this._colFile.Text = "File";
            this._colFile.Width = 180;

            // _colEntry
            this._colEntry.Text = "Entry";
            this._colEntry.Width = 50;

            // _colField
            this._colField.Text = "Field";
            this._colField.Width = 120;

            // _colMatch
            this._colMatch.Text = "Matched / Preview";
            this._colMatch.Width = 240;

            // FindInFilesDialog
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 562);
            this.Controls.Add(this._listResults);
            this.Controls.Add(this._lblStatus);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._btnApplyReplace);
            this.Controls.Add(this._btnReplacePreview);
            this.Controls.Add(this._btnFindAll);
            this.Controls.Add(this._grpFields);
            this.Controls.Add(this._grpDirectory);
            this.Controls.Add(this._chkWholeWord);
            this.Controls.Add(this._chkMatchCase);
            this.Controls.Add(this._txtReplace);
            this.Controls.Add(this._lblReplace);
            this.Controls.Add(this._txtFind);
            this.Controls.Add(this._lblFind);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 400);
            this.Name = "FindInFilesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Find in Files";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);

            this._grpDirectory.ResumeLayout(false);
            this._grpDirectory.PerformLayout();
            this._grpFields.ResumeLayout(false);
            this._grpFields.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label _lblFind;
        private System.Windows.Forms.TextBox _txtFind;
        private System.Windows.Forms.Label _lblReplace;
        private System.Windows.Forms.TextBox _txtReplace;
        private System.Windows.Forms.CheckBox _chkMatchCase;
        private System.Windows.Forms.CheckBox _chkWholeWord;
        private System.Windows.Forms.GroupBox _grpDirectory;
        private System.Windows.Forms.Label _lblSideHint;
        private System.Windows.Forms.Button _btnBrowseDir;
        private System.Windows.Forms.TextBox _txtDirectory;
        private System.Windows.Forms.CheckBox _chkSubdirs;
        private System.Windows.Forms.GroupBox _grpFields;
        private System.Windows.Forms.CheckBox _chkFlags;
        private System.Windows.Forms.CheckBox _chkActorName;
        private System.Windows.Forms.CheckBox _chkSpeechOriginal;
        private System.Windows.Forms.CheckBox _chkSpeechTranslation;
        private System.Windows.Forms.Button _btnFindAll;
        private System.Windows.Forms.Button _btnReplacePreview;
        private System.Windows.Forms.Button _btnApplyReplace;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.ListView _listResults;
        private System.Windows.Forms.ColumnHeader _colFile;
        private System.Windows.Forms.ColumnHeader _colEntry;
        private System.Windows.Forms.ColumnHeader _colField;
        private System.Windows.Forms.ColumnHeader _colMatch;
    }
}
