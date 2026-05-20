namespace TTG_Tools
{
    partial class LandbEditor
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
            // === MenuStrip ===
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            this._fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._openDirAMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._openDirBMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._closeMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._exportCharsAMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._exportCharsBMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._findMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._replaceMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._findNextMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._compareMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._syncScrollMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._mainTable = new System.Windows.Forms.TableLayoutPanel();

            // === Log ===
            this._txtLog = new System.Windows.Forms.TextBox();

            // === Side A: Tree panel ===
            this._panelDirA = new System.Windows.Forms.Panel();
            this._innerTableA = new System.Windows.Forms.TableLayoutPanel();
            this._topBarA = new System.Windows.Forms.Panel();
            this._txtPathA = new System.Windows.Forms.TextBox();
            this._btnBrowseA = new System.Windows.Forms.Button();
            this._treeViewA = new System.Windows.Forms.TreeView();

            // === Side A: Grid panel ===
            this._panelGridA = new System.Windows.Forms.Panel();
            this._innerGridA = new System.Windows.Forms.TableLayoutPanel();
            this._lblFileInfoA = new System.Windows.Forms.Label();
            this._gridViewA = new System.Windows.Forms.DataGridView();
            this._colFieldA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colValueA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._flowButtonsA = new System.Windows.Forms.FlowLayoutPanel();
            this._btnSaveA = new System.Windows.Forms.Button();
            this._btnSaveAsA = new System.Windows.Forms.Button();

            // === Side B: Tree panel ===
            this._panelDirB = new System.Windows.Forms.Panel();
            this._innerTableB = new System.Windows.Forms.TableLayoutPanel();
            this._topBarB = new System.Windows.Forms.Panel();
            this._txtPathB = new System.Windows.Forms.TextBox();
            this._btnBrowseB = new System.Windows.Forms.Button();
            this._treeViewB = new System.Windows.Forms.TreeView();

            // === Side B: Grid panel ===
            this._panelGridB = new System.Windows.Forms.Panel();
            this._innerGridB = new System.Windows.Forms.TableLayoutPanel();
            this._lblFileInfoB = new System.Windows.Forms.Label();
            this._gridViewB = new System.Windows.Forms.DataGridView();
            this._colFieldB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colValueB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._flowButtonsB = new System.Windows.Forms.FlowLayoutPanel();
            this._btnSaveB = new System.Windows.Forms.Button();
            this._btnSaveAsB = new System.Windows.Forms.Button();

            // === SuspendLayout ===
            this._menuStrip.SuspendLayout();
            this._mainTable.SuspendLayout();
            this._panelDirA.SuspendLayout();
            this._innerTableA.SuspendLayout();
            this._topBarA.SuspendLayout();
            this._panelGridA.SuspendLayout();
            this._innerGridA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._gridViewA)).BeginInit();
            this._flowButtonsA.SuspendLayout();
            this._panelDirB.SuspendLayout();
            this._innerTableB.SuspendLayout();
            this._topBarB.SuspendLayout();
            this._panelGridB.SuspendLayout();
            this._innerGridB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._gridViewB)).BeginInit();
            this._flowButtonsB.SuspendLayout();
            this.SuspendLayout();

            // _menuStrip
            this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._fileMenu, this._editMenu, this._compareMenu});
            this._menuStrip.Location = new System.Drawing.Point(0, 0);
            this._menuStrip.Name = "_menuStrip";
            this._menuStrip.Size = new System.Drawing.Size(1600, 24);
            this._menuStrip.TabIndex = 0;

            // _fileMenu
            this._fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._openDirAMenu, this._openDirBMenu,
                this.toolStripSeparator1,
                this._exportCharsAMenu, this._exportCharsBMenu,
                this.toolStripSeparator2, this._closeMenu});
            this._fileMenu.Name = "_fileMenu";
            this._fileMenu.Size = new System.Drawing.Size(37, 20);
            this._fileMenu.Text = "File";
            this._openDirAMenu.Name = "_openDirAMenu";
            this._openDirAMenu.Size = new System.Drawing.Size(180, 22);
            this._openDirAMenu.Text = "Open Directory A...";
            this._openDirAMenu.Click += new System.EventHandler(this.OnOpenDirA);
            this._openDirBMenu.Name = "_openDirBMenu";
            this._openDirBMenu.Size = new System.Drawing.Size(180, 22);
            this._openDirBMenu.Text = "Open Directory B...";
            this._openDirBMenu.Click += new System.EventHandler(this.OnOpenDirB);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            this._exportCharsAMenu.Name = "_exportCharsAMenu";
            this._exportCharsAMenu.Size = new System.Drawing.Size(180, 22);
            this._exportCharsAMenu.Text = "Export All Chars (Side A)...";
            this._exportCharsAMenu.Click += new System.EventHandler(this.OnExportCharsA);
            this._exportCharsBMenu.Name = "_exportCharsBMenu";
            this._exportCharsBMenu.Size = new System.Drawing.Size(180, 22);
            this._exportCharsBMenu.Text = "Export All Chars (Side B)...";
            this._exportCharsBMenu.Click += new System.EventHandler(this.OnExportCharsB);
            this._closeMenu.Name = "_closeMenu";
            this._closeMenu.Size = new System.Drawing.Size(180, 22);
            this._closeMenu.Text = "Close";
            this._closeMenu.Click += new System.EventHandler(this.OnCloseMenu);

            // _editMenu
            this._editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._findMenu, this._replaceMenu, this.toolStripSeparator3, this._findNextMenu});
            this._editMenu.Name = "_editMenu";
            this._editMenu.Size = new System.Drawing.Size(39, 20);
            this._editMenu.Text = "Edit";

            this._findMenu.Name = "_findMenu";
            this._findMenu.Size = new System.Drawing.Size(180, 22);
            this._findMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this._findMenu.Text = "Find...";
            this._findMenu.Click += new System.EventHandler(this.OnFindOpen);

            this._replaceMenu.Name = "_replaceMenu";
            this._replaceMenu.Size = new System.Drawing.Size(180, 22);
            this._replaceMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this._replaceMenu.Text = "Replace...";
            this._replaceMenu.Click += new System.EventHandler(this.OnReplaceOpen);

            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);

            this._findNextMenu.Name = "_findNextMenu";
            this._findNextMenu.Size = new System.Drawing.Size(180, 22);
            this._findNextMenu.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this._findNextMenu.Text = "Find Next";
            this._findNextMenu.Click += new System.EventHandler(this.OnFindNextMenu);

            // _compareMenu
            this._compareMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this._syncScrollMenu});
            this._compareMenu.Name = "_compareMenu";
            this._compareMenu.Size = new System.Drawing.Size(62, 20);
            this._compareMenu.Text = "Compare";
            this._syncScrollMenu.Checked = true;
            this._syncScrollMenu.CheckOnClick = true;
            this._syncScrollMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this._syncScrollMenu.Name = "_syncScrollMenu";
            this._syncScrollMenu.Size = new System.Drawing.Size(136, 22);
            this._syncScrollMenu.Text = "Sync Scroll";
            this._syncScrollMenu.CheckedChanged += new System.EventHandler(this.OnSyncScrollToggled);

            // _txtLog
            this._txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._txtLog.Location = new System.Drawing.Point(0, 824);
            this._txtLog.Multiline = true;
            this._txtLog.Name = "_txtLog";
            this._txtLog.ReadOnly = true;
            this._txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtLog.Size = new System.Drawing.Size(1600, 76);
            this._txtLog.TabIndex = 1;

            // _mainTable (4 columns)
            this._mainTable.ColumnCount = 4;
            this._mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this._mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36F));
            this._mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36F));
            this._mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this._mainTable.Controls.Add(this._panelDirA, 0, 0);
            this._mainTable.Controls.Add(this._panelGridA, 1, 0);
            this._mainTable.Controls.Add(this._panelGridB, 2, 0);
            this._mainTable.Controls.Add(this._panelDirB, 3, 0);
            this._mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainTable.Location = new System.Drawing.Point(0, 24);
            this._mainTable.Name = "_mainTable";
            this._mainTable.RowCount = 1;
            this._mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._mainTable.Size = new System.Drawing.Size(1600, 800);
            this._mainTable.TabIndex = 2;

            // ===== SIDE A: TREE PANEL =====
            this._panelDirA.Controls.Add(this._innerTableA);
            this._panelDirA.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelDirA.Name = "_panelDirA";

            this._innerTableA.ColumnCount = 1;
            this._innerTableA.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._innerTableA.Controls.Add(this._topBarA, 0, 0);
            this._innerTableA.Controls.Add(this._treeViewA, 0, 1);
            this._innerTableA.Dock = System.Windows.Forms.DockStyle.Fill;
            this._innerTableA.Name = "_innerTableA";
            this._innerTableA.Padding = new System.Windows.Forms.Padding(2);
            this._innerTableA.RowCount = 2;
            this._innerTableA.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this._innerTableA.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this._topBarA.Controls.Add(this._txtPathA);
            this._topBarA.Controls.Add(this._btnBrowseA);
            this._topBarA.Dock = System.Windows.Forms.DockStyle.Fill;
            this._topBarA.Name = "_topBarA";

            this._txtPathA.Dock = System.Windows.Forms.DockStyle.Fill;
            this._txtPathA.Name = "_txtPathA";

            this._btnBrowseA.Dock = System.Windows.Forms.DockStyle.Right;
            this._btnBrowseA.Name = "_btnBrowseA";
            this._btnBrowseA.Size = new System.Drawing.Size(72, 26);
            this._btnBrowseA.TabIndex = 1;
            this._btnBrowseA.Text = "Browse...";
            this._btnBrowseA.UseVisualStyleBackColor = true;
            this._btnBrowseA.Click += new System.EventHandler(this.OnBrowseA);

            this._treeViewA.Dock = System.Windows.Forms.DockStyle.Fill;
            this._treeViewA.HideSelection = false;
            this._treeViewA.Name = "_treeViewA";
            this._treeViewA.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeSelectA);

            // ===== SIDE A: GRID PANEL =====
            this._panelGridA.Controls.Add(this._innerGridA);
            this._panelGridA.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelGridA.Name = "_panelGridA";

            this._innerGridA.ColumnCount = 1;
            this._innerGridA.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._innerGridA.Controls.Add(this._lblFileInfoA, 0, 0);
            this._innerGridA.Controls.Add(this._gridViewA, 0, 1);
            this._innerGridA.Controls.Add(this._flowButtonsA, 0, 2);
            this._innerGridA.Dock = System.Windows.Forms.DockStyle.Fill;
            this._innerGridA.Name = "_innerGridA";
            this._innerGridA.Padding = new System.Windows.Forms.Padding(2);
            this._innerGridA.RowCount = 3;
            this._innerGridA.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this._innerGridA.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._innerGridA.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));

            this._lblFileInfoA.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lblFileInfoA.Name = "_lblFileInfoA";
            this._lblFileInfoA.Text = "No file loaded (Side A)";
            this._lblFileInfoA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this._gridViewA.AllowUserToAddRows = false;
            this._gridViewA.AllowUserToDeleteRows = false;
            this._gridViewA.AllowUserToResizeRows = false;
            this._gridViewA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._gridViewA.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._gridViewA.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this._colFieldA.HeaderText = "Field";
            this._colFieldA.Name = "_colFieldA";
            this._colFieldA.ReadOnly = true;
            this._colFieldA.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._colFieldA.Width = 120;
            this._colFieldA.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this._colValueA.HeaderText = "Value";
            this._colValueA.Name = "_colValueA";
            this._colValueA.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._colValueA.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._colValueA.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._gridViewA.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this._colFieldA, this._colValueA});
            this._gridViewA.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridViewA.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystrokeOrF2;
            this._gridViewA.Name = "_gridViewA";
            this._gridViewA.RowHeadersWidth = 50;
            this._gridViewA.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this._gridViewA.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnCellChangedA);
            this._gridViewA.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.OnCellValidatingA);

            this._flowButtonsA.Controls.Add(this._btnSaveA);
            this._flowButtonsA.Controls.Add(this._btnSaveAsA);
            this._flowButtonsA.Dock = System.Windows.Forms.DockStyle.Fill;
            this._flowButtonsA.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this._flowButtonsA.Name = "_flowButtonsA";
            this._flowButtonsA.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._btnSaveA.Enabled = false;
            this._btnSaveA.Name = "_btnSaveA";
            this._btnSaveA.Size = new System.Drawing.Size(72, 23);
            this._btnSaveA.TabIndex = 0;
            this._btnSaveA.Text = "Save A";
            this._btnSaveA.UseVisualStyleBackColor = true;
            this._btnSaveA.Click += new System.EventHandler(this.OnSaveA);
            this._btnSaveAsA.Enabled = false;
            this._btnSaveAsA.Name = "_btnSaveAsA";
            this._btnSaveAsA.Size = new System.Drawing.Size(86, 23);
            this._btnSaveAsA.TabIndex = 1;
            this._btnSaveAsA.Text = "Save As A...";
            this._btnSaveAsA.UseVisualStyleBackColor = true;
            this._btnSaveAsA.Click += new System.EventHandler(this.OnSaveAsA);

            // ===== SIDE B: GRID PANEL =====
            this._panelGridB.Controls.Add(this._innerGridB);
            this._panelGridB.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelGridB.Name = "_panelGridB";

            this._innerGridB.ColumnCount = 1;
            this._innerGridB.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._innerGridB.Controls.Add(this._lblFileInfoB, 0, 0);
            this._innerGridB.Controls.Add(this._gridViewB, 0, 1);
            this._innerGridB.Controls.Add(this._flowButtonsB, 0, 2);
            this._innerGridB.Dock = System.Windows.Forms.DockStyle.Fill;
            this._innerGridB.Name = "_innerGridB";
            this._innerGridB.Padding = new System.Windows.Forms.Padding(2);
            this._innerGridB.RowCount = 3;
            this._innerGridB.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this._innerGridB.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._innerGridB.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));

            this._lblFileInfoB.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lblFileInfoB.Name = "_lblFileInfoB";
            this._lblFileInfoB.Text = "No file loaded (Side B)";
            this._lblFileInfoB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this._gridViewB.AllowUserToAddRows = false;
            this._gridViewB.AllowUserToDeleteRows = false;
            this._gridViewB.AllowUserToResizeRows = false;
            this._gridViewB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._gridViewB.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._gridViewB.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this._colFieldB.HeaderText = "Field";
            this._colFieldB.Name = "_colFieldB";
            this._colFieldB.ReadOnly = true;
            this._colFieldB.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._colFieldB.Width = 120;
            this._colFieldB.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this._colValueB.HeaderText = "Value";
            this._colValueB.Name = "_colValueB";
            this._colValueB.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._colValueB.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._colValueB.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._gridViewB.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this._colFieldB, this._colValueB});
            this._gridViewB.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridViewB.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystrokeOrF2;
            this._gridViewB.Name = "_gridViewB";
            this._gridViewB.RowHeadersWidth = 50;
            this._gridViewB.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this._gridViewB.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnCellChangedB);
            this._gridViewB.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.OnCellValidatingB);

            this._flowButtonsB.Controls.Add(this._btnSaveB);
            this._flowButtonsB.Controls.Add(this._btnSaveAsB);
            this._flowButtonsB.Dock = System.Windows.Forms.DockStyle.Fill;
            this._flowButtonsB.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this._flowButtonsB.Name = "_flowButtonsB";
            this._flowButtonsB.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._btnSaveB.Enabled = false;
            this._btnSaveB.Name = "_btnSaveB";
            this._btnSaveB.Size = new System.Drawing.Size(72, 23);
            this._btnSaveB.TabIndex = 0;
            this._btnSaveB.Text = "Save B";
            this._btnSaveB.UseVisualStyleBackColor = true;
            this._btnSaveB.Click += new System.EventHandler(this.OnSaveB);
            this._btnSaveAsB.Enabled = false;
            this._btnSaveAsB.Name = "_btnSaveAsB";
            this._btnSaveAsB.Size = new System.Drawing.Size(86, 23);
            this._btnSaveAsB.TabIndex = 1;
            this._btnSaveAsB.Text = "Save As B...";
            this._btnSaveAsB.UseVisualStyleBackColor = true;
            this._btnSaveAsB.Click += new System.EventHandler(this.OnSaveAsB);

            // ===== SIDE B: TREE PANEL =====
            this._panelDirB.Controls.Add(this._innerTableB);
            this._panelDirB.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelDirB.Name = "_panelDirB";

            this._innerTableB.ColumnCount = 1;
            this._innerTableB.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._innerTableB.Controls.Add(this._topBarB, 0, 0);
            this._innerTableB.Controls.Add(this._treeViewB, 0, 1);
            this._innerTableB.Dock = System.Windows.Forms.DockStyle.Fill;
            this._innerTableB.Name = "_innerTableB";
            this._innerTableB.Padding = new System.Windows.Forms.Padding(2);
            this._innerTableB.RowCount = 2;
            this._innerTableB.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this._innerTableB.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this._topBarB.Controls.Add(this._txtPathB);
            this._topBarB.Controls.Add(this._btnBrowseB);
            this._topBarB.Dock = System.Windows.Forms.DockStyle.Fill;
            this._topBarB.Name = "_topBarB";

            this._txtPathB.Dock = System.Windows.Forms.DockStyle.Fill;
            this._txtPathB.Name = "_txtPathB";

            this._btnBrowseB.Dock = System.Windows.Forms.DockStyle.Right;
            this._btnBrowseB.Name = "_btnBrowseB";
            this._btnBrowseB.Size = new System.Drawing.Size(72, 26);
            this._btnBrowseB.TabIndex = 1;
            this._btnBrowseB.Text = "Browse...";
            this._btnBrowseB.UseVisualStyleBackColor = true;
            this._btnBrowseB.Click += new System.EventHandler(this.OnBrowseB);

            this._treeViewB.Dock = System.Windows.Forms.DockStyle.Fill;
            this._treeViewB.HideSelection = false;
            this._treeViewB.Name = "_treeViewB";
            this._treeViewB.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeSelectB);

            // === LandbEditor ===
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1600, 900);
            this.Controls.Add(this._mainTable);
            this.Controls.Add(this._txtLog);
            this.Controls.Add(this._menuStrip);
            this.KeyPreview = true;
            this.MainMenuStrip = this._menuStrip;
            this.Name = "LandbEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Landb Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);

            // === ResumeLayout ===
            this._menuStrip.ResumeLayout(false);
            this._menuStrip.PerformLayout();
            this._mainTable.ResumeLayout(false);
            this._panelDirA.ResumeLayout(false);
            this._innerTableA.ResumeLayout(false);
            this._topBarA.ResumeLayout(false);
            this._topBarA.PerformLayout();
            this._panelGridA.ResumeLayout(false);
            this._innerGridA.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._gridViewA)).EndInit();
            this._flowButtonsA.ResumeLayout(false);
            this._panelDirB.ResumeLayout(false);
            this._innerTableB.ResumeLayout(false);
            this._topBarB.ResumeLayout(false);
            this._topBarB.PerformLayout();
            this._panelGridB.ResumeLayout(false);
            this._innerGridB.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._gridViewB)).EndInit();
            this._flowButtonsB.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip _menuStrip;
        private System.Windows.Forms.ToolStripMenuItem _fileMenu;
        private System.Windows.Forms.ToolStripMenuItem _openDirAMenu;
        private System.Windows.Forms.ToolStripMenuItem _openDirBMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem _exportCharsAMenu;
        private System.Windows.Forms.ToolStripMenuItem _exportCharsBMenu;
        private System.Windows.Forms.ToolStripMenuItem _closeMenu;
        private System.Windows.Forms.ToolStripMenuItem _editMenu;
        private System.Windows.Forms.ToolStripMenuItem _findMenu;
        private System.Windows.Forms.ToolStripMenuItem _replaceMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem _findNextMenu;
        private System.Windows.Forms.ToolStripMenuItem _compareMenu;
        private System.Windows.Forms.ToolStripMenuItem _syncScrollMenu;
        private System.Windows.Forms.TableLayoutPanel _mainTable;
        private System.Windows.Forms.TextBox _txtLog;

        private System.Windows.Forms.Panel _panelDirA;
        private System.Windows.Forms.TableLayoutPanel _innerTableA;
        private System.Windows.Forms.Panel _topBarA;
        private System.Windows.Forms.TextBox _txtPathA;
        private System.Windows.Forms.Button _btnBrowseA;
        private System.Windows.Forms.TreeView _treeViewA;

        private System.Windows.Forms.Panel _panelGridA;
        private System.Windows.Forms.TableLayoutPanel _innerGridA;
        private System.Windows.Forms.Label _lblFileInfoA;
        private System.Windows.Forms.DataGridView _gridViewA;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colFieldA;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colValueA;
        private System.Windows.Forms.FlowLayoutPanel _flowButtonsA;
        private System.Windows.Forms.Button _btnSaveA;
        private System.Windows.Forms.Button _btnSaveAsA;

        private System.Windows.Forms.Panel _panelDirB;
        private System.Windows.Forms.TableLayoutPanel _innerTableB;
        private System.Windows.Forms.Panel _topBarB;
        private System.Windows.Forms.TextBox _txtPathB;
        private System.Windows.Forms.Button _btnBrowseB;
        private System.Windows.Forms.TreeView _treeViewB;

        private System.Windows.Forms.Panel _panelGridB;
        private System.Windows.Forms.TableLayoutPanel _innerGridB;
        private System.Windows.Forms.Label _lblFileInfoB;
        private System.Windows.Forms.DataGridView _gridViewB;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colFieldB;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colValueB;
        private System.Windows.Forms.FlowLayoutPanel _flowButtonsB;
        private System.Windows.Forms.Button _btnSaveB;
        private System.Windows.Forms.Button _btnSaveAsB;
    }
}
