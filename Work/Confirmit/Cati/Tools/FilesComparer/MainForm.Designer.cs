namespace FilesComparer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.textBoxFolderPath1 = new System.Windows.Forms.TextBox();
            this.labelFolder1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.textBoxFolderPath2 = new System.Windows.Forms.TextBox();
            this.labelFolder2 = new System.Windows.Forms.Label();
            this.buttonStartComparing = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.FilesList = new System.Windows.Forms.DataGridView();
            this.FilePathFolder1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CompareValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FilePathFolder2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EmptyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxCompareInSubfolders = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSelectFolder2 = new System.Windows.Forms.Button();
            this.buttonSelectFolder1 = new System.Windows.Forms.Button();
            this.labelWait = new System.Windows.Forms.Label();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.checkBoxLogWrongComparedFiles = new System.Windows.Forms.CheckBox();
            this.textBoxFileMasks = new System.Windows.Forms.TextBox();
            this.textBoxIgnoreFilesMasks = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonStopComparing = new System.Windows.Forms.Button();
            this.checkBoxShowDifferent = new System.Windows.Forms.CheckBox();
            this.checkBoxShowUnique = new System.Windows.Forms.CheckBox();
            this.checkBoxShowSkipped = new System.Windows.Forms.CheckBox();
            this.checkBoxShowEqual = new System.Windows.Forms.CheckBox();
            this.checkBoxShowNotCompared = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.FilesList)).BeginInit();
            this.groupBoxSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxFolderPath1
            // 
            this.textBoxFolderPath1.Location = new System.Drawing.Point(12, 20);
            this.textBoxFolderPath1.Name = "textBoxFolderPath1";
            this.textBoxFolderPath1.Size = new System.Drawing.Size(356, 20);
            this.textBoxFolderPath1.TabIndex = 0;
            this.textBoxFolderPath1.Text = "c:\\_Folder1\\";
            // 
            // labelFolder1
            // 
            this.labelFolder1.AutoSize = true;
            this.labelFolder1.Location = new System.Drawing.Point(148, 4);
            this.labelFolder1.Name = "labelFolder1";
            this.labelFolder1.Size = new System.Drawing.Size(69, 13);
            this.labelFolder1.TabIndex = 1;
            this.labelFolder1.Text = "Folder path 1";
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Select folder to compare";
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // textBoxFolderPath2
            // 
            this.textBoxFolderPath2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFolderPath2.Location = new System.Drawing.Point(442, 20);
            this.textBoxFolderPath2.Name = "textBoxFolderPath2";
            this.textBoxFolderPath2.Size = new System.Drawing.Size(356, 20);
            this.textBoxFolderPath2.TabIndex = 3;
            this.textBoxFolderPath2.Text = "c:\\_Folder2\\";
            // 
            // labelFolder2
            // 
            this.labelFolder2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFolder2.AutoSize = true;
            this.labelFolder2.Location = new System.Drawing.Point(592, 4);
            this.labelFolder2.Name = "labelFolder2";
            this.labelFolder2.Size = new System.Drawing.Size(69, 13);
            this.labelFolder2.TabIndex = 4;
            this.labelFolder2.Text = "Folder path 2";
            // 
            // buttonStartComparing
            // 
            this.buttonStartComparing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStartComparing.Location = new System.Drawing.Point(564, 61);
            this.buttonStartComparing.Name = "buttonStartComparing";
            this.buttonStartComparing.Size = new System.Drawing.Size(119, 25);
            this.buttonStartComparing.TabIndex = 6;
            this.buttonStartComparing.Text = "Start comparing";
            this.buttonStartComparing.UseVisualStyleBackColor = true;
            this.buttonStartComparing.Click += new System.EventHandler(this.ButtonStartComparingClick);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 503);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(818, 23);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 7;
            // 
            // FilesList
            // 
            this.FilesList.AllowUserToAddRows = false;
            this.FilesList.AllowUserToDeleteRows = false;
            this.FilesList.AllowUserToResizeRows = false;
            this.FilesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilesList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.FilesList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.FilesList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.FilesList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.FilesList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FilesList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FilePathFolder1,
            this.CompareValue,
            this.FilePathFolder2,
            this.EmptyColumn});
            this.FilesList.Location = new System.Drawing.Point(12, 133);
            this.FilesList.MultiSelect = false;
            this.FilesList.Name = "FilesList";
            this.FilesList.ReadOnly = true;
            this.FilesList.RowHeadersVisible = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.FilesList.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.FilesList.RowTemplate.Height = 17;
            this.FilesList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.FilesList.Size = new System.Drawing.Size(818, 364);
            this.FilesList.StandardTab = true;
            this.FilesList.TabIndex = 10;
            this.FilesList.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.FilesListCellMouseDoubleClick);
            // 
            // FilePathFolder1
            // 
            this.FilePathFolder1.HeaderText = "File and folder paths from folder 1";
            this.FilePathFolder1.Name = "FilePathFolder1";
            this.FilePathFolder1.ReadOnly = true;
            this.FilePathFolder1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.FilePathFolder1.Width = 350;
            // 
            // CompareValue
            // 
            this.CompareValue.HeaderText = "Compare value";
            this.CompareValue.Name = "CompareValue";
            this.CompareValue.ReadOnly = true;
            this.CompareValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FilePathFolder2
            // 
            this.FilePathFolder2.HeaderText = "File and folder paths from folder 2";
            this.FilePathFolder2.Name = "FilePathFolder2";
            this.FilePathFolder2.ReadOnly = true;
            this.FilePathFolder2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.FilePathFolder2.Width = 350;
            // 
            // EmptyColumn
            // 
            this.EmptyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.EmptyColumn.HeaderText = "";
            this.EmptyColumn.MinimumWidth = 2;
            this.EmptyColumn.Name = "EmptyColumn";
            this.EmptyColumn.ReadOnly = true;
            this.EmptyColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // checkBoxCompareInSubfolders
            // 
            this.checkBoxCompareInSubfolders.AutoSize = true;
            this.checkBoxCompareInSubfolders.Checked = true;
            this.checkBoxCompareInSubfolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCompareInSubfolders.Location = new System.Drawing.Point(6, 10);
            this.checkBoxCompareInSubfolders.Name = "checkBoxCompareInSubfolders";
            this.checkBoxCompareInSubfolders.Size = new System.Drawing.Size(97, 30);
            this.checkBoxCompareInSubfolders.TabIndex = 12;
            this.checkBoxCompareInSubfolders.Text = "Compare files\r\nfrom subfolders";
            this.checkBoxCompareInSubfolders.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(198, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 39);
            this.label1.TabIndex = 13;
            this.label1.Text = "Including\r\nfile\r\nmasks:";
            // 
            // buttonSelectFolder2
            // 
            this.buttonSelectFolder2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectFolder2.Image = global::FilesComparer.Properties.Resources.open_16;
            this.buttonSelectFolder2.Location = new System.Drawing.Point(806, 17);
            this.buttonSelectFolder2.Name = "buttonSelectFolder2";
            this.buttonSelectFolder2.Size = new System.Drawing.Size(21, 21);
            this.buttonSelectFolder2.TabIndex = 5;
            this.buttonSelectFolder2.UseVisualStyleBackColor = true;
            this.buttonSelectFolder2.Click += new System.EventHandler(this.ButtonSelectFolder2Click);
            // 
            // buttonSelectFolder1
            // 
            this.buttonSelectFolder1.Image = global::FilesComparer.Properties.Resources.open_16;
            this.buttonSelectFolder1.Location = new System.Drawing.Point(376, 17);
            this.buttonSelectFolder1.Name = "buttonSelectFolder1";
            this.buttonSelectFolder1.Size = new System.Drawing.Size(21, 21);
            this.buttonSelectFolder1.TabIndex = 2;
            this.buttonSelectFolder1.UseVisualStyleBackColor = true;
            this.buttonSelectFolder1.Click += new System.EventHandler(this.ButtonSelectFolder1Click);
            // 
            // labelWait
            // 
            this.labelWait.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelWait.AutoSize = true;
            this.labelWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWait.Location = new System.Drawing.Point(282, 506);
            this.labelWait.Name = "labelWait";
            this.labelWait.Size = new System.Drawing.Size(337, 13);
            this.labelWait.TabIndex = 15;
            this.labelWait.Text = "Wait. Program collect information about files to compare...";
            this.labelWait.Visible = false;
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSettings.Controls.Add(this.checkBoxLogWrongComparedFiles);
            this.groupBoxSettings.Controls.Add(this.textBoxFileMasks);
            this.groupBoxSettings.Controls.Add(this.textBoxIgnoreFilesMasks);
            this.groupBoxSettings.Controls.Add(this.label2);
            this.groupBoxSettings.Controls.Add(this.checkBoxCompareInSubfolders);
            this.groupBoxSettings.Controls.Add(this.label1);
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 47);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(537, 80);
            this.groupBoxSettings.TabIndex = 16;
            this.groupBoxSettings.TabStop = false;
            // 
            // checkBoxLogWrongComparedFiles
            // 
            this.checkBoxLogWrongComparedFiles.AutoSize = true;
            this.checkBoxLogWrongComparedFiles.Checked = true;
            this.checkBoxLogWrongComparedFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLogWrongComparedFiles.Location = new System.Drawing.Point(6, 43);
            this.checkBoxLogWrongComparedFiles.Name = "checkBoxLogWrongComparedFiles";
            this.checkBoxLogWrongComparedFiles.Size = new System.Drawing.Size(149, 30);
            this.checkBoxLogWrongComparedFiles.TabIndex = 18;
            this.checkBoxLogWrongComparedFiles.Text = "Log information about files\r\nthat cannot be compared";
            this.checkBoxLogWrongComparedFiles.UseVisualStyleBackColor = true;
            // 
            // textBoxFileMasks
            // 
            this.textBoxFileMasks.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.textBoxFileMasks.Location = new System.Drawing.Point(254, 9);
            this.textBoxFileMasks.Multiline = true;
            this.textBoxFileMasks.Name = "textBoxFileMasks";
            this.textBoxFileMasks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxFileMasks.Size = new System.Drawing.Size(86, 62);
            this.textBoxFileMasks.TabIndex = 17;
            this.textBoxFileMasks.Text = "*.*";
            // 
            // textBoxIgnoreFilesMasks
            // 
            this.textBoxIgnoreFilesMasks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIgnoreFilesMasks.Location = new System.Drawing.Point(445, 12);
            this.textBoxIgnoreFilesMasks.Multiline = true;
            this.textBoxIgnoreFilesMasks.Name = "textBoxIgnoreFilesMasks";
            this.textBoxIgnoreFilesMasks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxIgnoreFilesMasks.Size = new System.Drawing.Size(86, 62);
            this.textBoxIgnoreFilesMasks.TabIndex = 16;
            this.textBoxIgnoreFilesMasks.Text = "*.pdb\r\n*.deploy\r\n*.manifest\r\n*.zip\r\nConfirmit.CATI.Core.Fakes.dll\r\nEntityFramewor" +
    "k.dll\r\n";
            this.textBoxIgnoreFilesMasks.WordWrap = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(399, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 39);
            this.label2.TabIndex = 15;
            this.label2.Text = "Ignore\r\nfile\r\nmasks:";
            // 
            // buttonStopComparing
            // 
            this.buttonStopComparing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStopComparing.Enabled = false;
            this.buttonStopComparing.Location = new System.Drawing.Point(587, 92);
            this.buttonStopComparing.Name = "buttonStopComparing";
            this.buttonStopComparing.Size = new System.Drawing.Size(71, 25);
            this.buttonStopComparing.TabIndex = 17;
            this.buttonStopComparing.Text = "Stop";
            this.buttonStopComparing.UseVisualStyleBackColor = true;
            this.buttonStopComparing.Click += new System.EventHandler(this.ButtonStopComparingClick);
            // 
            // checkBoxShowDifferent
            // 
            this.checkBoxShowDifferent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxShowDifferent.AutoSize = true;
            this.checkBoxShowDifferent.Checked = true;
            this.checkBoxShowDifferent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowDifferent.Location = new System.Drawing.Point(702, 46);
            this.checkBoxShowDifferent.Name = "checkBoxShowDifferent";
            this.checkBoxShowDifferent.Size = new System.Drawing.Size(115, 17);
            this.checkBoxShowDifferent.TabIndex = 22;
            this.checkBoxShowDifferent.Text = "Show different files";
            this.checkBoxShowDifferent.UseVisualStyleBackColor = true;
            this.checkBoxShowDifferent.CheckedChanged += new System.EventHandler(this.SelectedShowModeChanged);
            // 
            // checkBoxShowUnique
            // 
            this.checkBoxShowUnique.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxShowUnique.AutoSize = true;
            this.checkBoxShowUnique.Checked = true;
            this.checkBoxShowUnique.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowUnique.Location = new System.Drawing.Point(702, 78);
            this.checkBoxShowUnique.Name = "checkBoxShowUnique";
            this.checkBoxShowUnique.Size = new System.Drawing.Size(109, 17);
            this.checkBoxShowUnique.TabIndex = 23;
            this.checkBoxShowUnique.Text = "Show unique files";
            this.checkBoxShowUnique.UseVisualStyleBackColor = true;
            this.checkBoxShowUnique.CheckedChanged += new System.EventHandler(this.SelectedShowModeChanged);
            // 
            // checkBoxShowSkipped
            // 
            this.checkBoxShowSkipped.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxShowSkipped.AutoSize = true;
            this.checkBoxShowSkipped.Checked = true;
            this.checkBoxShowSkipped.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowSkipped.Location = new System.Drawing.Point(702, 94);
            this.checkBoxShowSkipped.Name = "checkBoxShowSkipped";
            this.checkBoxShowSkipped.Size = new System.Drawing.Size(114, 17);
            this.checkBoxShowSkipped.TabIndex = 25;
            this.checkBoxShowSkipped.Text = "Show skipped files";
            this.checkBoxShowSkipped.UseVisualStyleBackColor = true;
            this.checkBoxShowSkipped.CheckedChanged += new System.EventHandler(this.SelectedShowModeChanged);
            // 
            // checkBoxShowEqual
            // 
            this.checkBoxShowEqual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxShowEqual.AutoSize = true;
            this.checkBoxShowEqual.Checked = true;
            this.checkBoxShowEqual.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowEqual.Location = new System.Drawing.Point(702, 62);
            this.checkBoxShowEqual.Name = "checkBoxShowEqual";
            this.checkBoxShowEqual.Size = new System.Drawing.Size(103, 17);
            this.checkBoxShowEqual.TabIndex = 26;
            this.checkBoxShowEqual.Text = "Show equal files";
            this.checkBoxShowEqual.UseVisualStyleBackColor = true;
            this.checkBoxShowEqual.CheckedChanged += new System.EventHandler(this.SelectedShowModeChanged);
            // 
            // checkBoxShowNotCompared
            // 
            this.checkBoxShowNotCompared.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxShowNotCompared.AutoSize = true;
            this.checkBoxShowNotCompared.Checked = true;
            this.checkBoxShowNotCompared.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowNotCompared.Location = new System.Drawing.Point(702, 110);
            this.checkBoxShowNotCompared.Name = "checkBoxShowNotCompared";
            this.checkBoxShowNotCompared.Size = new System.Drawing.Size(142, 17);
            this.checkBoxShowNotCompared.TabIndex = 27;
            this.checkBoxShowNotCompared.Text = "Show not compared files";
            this.checkBoxShowNotCompared.UseVisualStyleBackColor = true;
            this.checkBoxShowNotCompared.CheckedChanged += new System.EventHandler(this.SelectedShowModeChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 528);
            this.Controls.Add(this.checkBoxShowNotCompared);
            this.Controls.Add(this.checkBoxShowEqual);
            this.Controls.Add(this.checkBoxShowSkipped);
            this.Controls.Add(this.checkBoxShowUnique);
            this.Controls.Add(this.checkBoxShowDifferent);
            this.Controls.Add(this.buttonStopComparing);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.labelWait);
            this.Controls.Add(this.buttonStartComparing);
            this.Controls.Add(this.FilesList);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonSelectFolder2);
            this.Controls.Add(this.labelFolder2);
            this.Controls.Add(this.textBoxFolderPath2);
            this.Controls.Add(this.buttonSelectFolder1);
            this.Controls.Add(this.labelFolder1);
            this.Controls.Add(this.textBoxFolderPath1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(850, 200);
            this.Name = "MainForm";
            this.Text = "Files Comparer";
            ((System.ComponentModel.ISupportInitialize)(this.FilesList)).EndInit();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFolderPath1;
        private System.Windows.Forms.Label labelFolder1;
        private System.Windows.Forms.Button buttonSelectFolder1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox textBoxFolderPath2;
        private System.Windows.Forms.Label labelFolder2;
        private System.Windows.Forms.Button buttonSelectFolder2;
        private System.Windows.Forms.Button buttonStartComparing;
        private System.Windows.Forms.ProgressBar progressBar;
        public System.Windows.Forms.DataGridView FilesList;
        private System.Windows.Forms.CheckBox checkBoxCompareInSubfolders;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelWait;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.TextBox textBoxFileMasks;
        private System.Windows.Forms.TextBox textBoxIgnoreFilesMasks;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonStopComparing;
        private System.Windows.Forms.DataGridViewTextBoxColumn FilePathFolder1;
        private System.Windows.Forms.DataGridViewTextBoxColumn CompareValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn FilePathFolder2;
        private System.Windows.Forms.DataGridViewTextBoxColumn EmptyColumn;
        private System.Windows.Forms.CheckBox checkBoxShowDifferent;
        private System.Windows.Forms.CheckBox checkBoxShowUnique;
        private System.Windows.Forms.CheckBox checkBoxShowSkipped;
        private System.Windows.Forms.CheckBox checkBoxShowEqual;
        private System.Windows.Forms.CheckBox checkBoxShowNotCompared;
        private System.Windows.Forms.CheckBox checkBoxLogWrongComparedFiles;
    }
}

