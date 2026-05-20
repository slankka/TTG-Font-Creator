namespace TTG_Tools
{
    partial class FindReplaceDialog
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
            this._grpSide = new System.Windows.Forms.GroupBox();
            this._radioSideB = new System.Windows.Forms.RadioButton();
            this._radioSideA = new System.Windows.Forms.RadioButton();
            this._btnFindNext = new System.Windows.Forms.Button();
            this._btnReplace = new System.Windows.Forms.Button();
            this._btnReplaceAll = new System.Windows.Forms.Button();
            this._btnClose = new System.Windows.Forms.Button();
            this._lblStatus = new System.Windows.Forms.Label();
            this._grpSide.SuspendLayout();
            this.SuspendLayout();

            // _lblFind
            this._lblFind.AutoSize = true;
            this._lblFind.Location = new System.Drawing.Point(12, 15);
            this._lblFind.Name = "_lblFind";
            this._lblFind.Size = new System.Drawing.Size(56, 13);
            this._lblFind.TabIndex = 0;
            this._lblFind.Text = "Find what:";

            // _txtFind
            this._txtFind.Location = new System.Drawing.Point(100, 12);
            this._txtFind.Name = "_txtFind";
            this._txtFind.Size = new System.Drawing.Size(260, 20);
            this._txtFind.TabIndex = 1;
            this._txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFindKeyDown);

            // _lblReplace
            this._lblReplace.AutoSize = true;
            this._lblReplace.Location = new System.Drawing.Point(12, 41);
            this._lblReplace.Name = "_lblReplace";
            this._lblReplace.Size = new System.Drawing.Size(82, 13);
            this._lblReplace.TabIndex = 2;
            this._lblReplace.Text = "Replace with:";

            // _txtReplace
            this._txtReplace.Location = new System.Drawing.Point(100, 38);
            this._txtReplace.Name = "_txtReplace";
            this._txtReplace.Size = new System.Drawing.Size(260, 20);
            this._txtReplace.TabIndex = 3;
            this._txtReplace.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnReplaceKeyDown);

            // _chkMatchCase
            this._chkMatchCase.AutoSize = true;
            this._chkMatchCase.Location = new System.Drawing.Point(15, 70);
            this._chkMatchCase.Name = "_chkMatchCase";
            this._chkMatchCase.Size = new System.Drawing.Size(82, 17);
            this._chkMatchCase.TabIndex = 4;
            this._chkMatchCase.Text = "Match case";

            // _grpSide
            this._grpSide.Controls.Add(this._radioSideB);
            this._grpSide.Controls.Add(this._radioSideA);
            this._grpSide.Location = new System.Drawing.Point(15, 95);
            this._grpSide.Name = "_grpSide";
            this._grpSide.Size = new System.Drawing.Size(170, 42);
            this._grpSide.TabIndex = 5;
            this._grpSide.TabStop = false;
            this._grpSide.Text = "Search in";

            // _radioSideA
            this._radioSideA.AutoSize = true;
            this._radioSideA.Checked = true;
            this._radioSideA.Location = new System.Drawing.Point(10, 17);
            this._radioSideA.Name = "_radioSideA";
            this._radioSideA.Size = new System.Drawing.Size(56, 17);
            this._radioSideA.TabIndex = 0;
            this._radioSideA.TabStop = true;
            this._radioSideA.Text = "Side A";
            this._radioSideA.UseVisualStyleBackColor = true;

            // _radioSideB
            this._radioSideB.AutoSize = true;
            this._radioSideB.Location = new System.Drawing.Point(80, 17);
            this._radioSideB.Name = "_radioSideB";
            this._radioSideB.Size = new System.Drawing.Size(56, 17);
            this._radioSideB.TabIndex = 1;
            this._radioSideB.Text = "Side B";
            this._radioSideB.UseVisualStyleBackColor = true;

            // _btnFindNext
            this._btnFindNext.Location = new System.Drawing.Point(380, 10);
            this._btnFindNext.Name = "_btnFindNext";
            this._btnFindNext.Size = new System.Drawing.Size(90, 23);
            this._btnFindNext.TabIndex = 6;
            this._btnFindNext.Text = "Find Next";
            this._btnFindNext.UseVisualStyleBackColor = true;
            this._btnFindNext.Click += new System.EventHandler(this.OnFindNext);

            // _btnReplace
            this._btnReplace.Location = new System.Drawing.Point(380, 36);
            this._btnReplace.Name = "_btnReplace";
            this._btnReplace.Size = new System.Drawing.Size(90, 23);
            this._btnReplace.TabIndex = 7;
            this._btnReplace.Text = "Replace";
            this._btnReplace.UseVisualStyleBackColor = true;
            this._btnReplace.Click += new System.EventHandler(this.OnReplace);

            // _btnReplaceAll
            this._btnReplaceAll.Location = new System.Drawing.Point(380, 62);
            this._btnReplaceAll.Name = "_btnReplaceAll";
            this._btnReplaceAll.Size = new System.Drawing.Size(90, 23);
            this._btnReplaceAll.TabIndex = 8;
            this._btnReplaceAll.Text = "Replace All";
            this._btnReplaceAll.UseVisualStyleBackColor = true;
            this._btnReplaceAll.Click += new System.EventHandler(this.OnReplaceAll);

            // _btnClose
            this._btnClose.Location = new System.Drawing.Point(380, 110);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(90, 23);
            this._btnClose.TabIndex = 9;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = true;
            this._btnClose.Click += new System.EventHandler(this.OnClose);

            // _lblStatus
            this._lblStatus.AutoSize = true;
            this._lblStatus.Location = new System.Drawing.Point(12, 145);
            this._lblStatus.Name = "_lblStatus";
            this._lblStatus.Size = new System.Drawing.Size(0, 13);
            this._lblStatus.TabIndex = 10;

            // FindReplaceDialog
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 170);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Controls.Add(this._lblStatus);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._btnReplaceAll);
            this.Controls.Add(this._btnReplace);
            this.Controls.Add(this._btnFindNext);
            this.Controls.Add(this._grpSide);
            this.Controls.Add(this._chkMatchCase);
            this.Controls.Add(this._txtReplace);
            this.Controls.Add(this._lblReplace);
            this.Controls.Add(this._txtFind);
            this.Controls.Add(this._lblFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindReplaceDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Find and Replace";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);

            this._grpSide.ResumeLayout(false);
            this._grpSide.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label _lblFind;
        private System.Windows.Forms.TextBox _txtFind;
        private System.Windows.Forms.Label _lblReplace;
        private System.Windows.Forms.TextBox _txtReplace;
        private System.Windows.Forms.CheckBox _chkMatchCase;
        private System.Windows.Forms.GroupBox _grpSide;
        private System.Windows.Forms.RadioButton _radioSideA;
        private System.Windows.Forms.RadioButton _radioSideB;
        private System.Windows.Forms.Button _btnFindNext;
        private System.Windows.Forms.Button _btnReplace;
        private System.Windows.Forms.Button _btnReplaceAll;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.Label _lblStatus;
    }
}
