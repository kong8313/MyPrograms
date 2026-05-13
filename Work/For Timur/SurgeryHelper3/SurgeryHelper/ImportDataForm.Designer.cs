namespace SurgeryHelper
{
    partial class ImportDataForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportDataForm));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkedListBoxForeignPatients = new System.Windows.Forms.CheckedListBox();
            this.buttonGetData = new System.Windows.Forms.Button();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.checkedListBoxForeignNosology = new System.Windows.Forms.CheckedListBox();
            this.checkBoxSelectAllPatients = new System.Windows.Forms.CheckBox();
            this.checkBoxSelectAllNosologies = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(10, 33);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(704, 20);
            this.textBoxPath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(671, 13);
            this.label1.TabIndex = 79;
            this.label1.Text = "Путь до папки, содержащей файлы patients.save и nosologys.save";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkedListBoxForeignPatients
            // 
            this.checkedListBoxForeignPatients.CheckOnClick = true;
            this.checkedListBoxForeignPatients.FormattingEnabled = true;
            this.checkedListBoxForeignPatients.Location = new System.Drawing.Point(10, 125);
            this.checkedListBoxForeignPatients.Name = "checkedListBoxForeignPatients";
            this.checkedListBoxForeignPatients.ScrollAlwaysVisible = true;
            this.checkedListBoxForeignPatients.Size = new System.Drawing.Size(358, 319);
            this.checkedListBoxForeignPatients.TabIndex = 2;
            // 
            // buttonGetData
            // 
            this.buttonGetData.BackgroundImage = global::SurgeryHelper.Properties.Resources.GetData;
            this.buttonGetData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonGetData.FlatAppearance.BorderSize = 0;
            this.buttonGetData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonGetData.Location = new System.Drawing.Point(337, 58);
            this.buttonGetData.Name = "buttonGetData";
            this.buttonGetData.Size = new System.Drawing.Size(40, 40);
            this.buttonGetData.TabIndex = 81;
            this.buttonGetData.TabStop = false;
            this.buttonGetData.UseVisualStyleBackColor = true;
            this.buttonGetData.Click += new System.EventHandler(this.buttonGetData_Click);
            this.buttonGetData.MouseEnter += new System.EventHandler(this.buttonGetData_MouseEnter);
            this.buttonGetData.MouseLeave += new System.EventHandler(this.buttonGetData_MouseLeave);
            // 
            // buttonOpen
            // 
            this.buttonOpen.BackgroundImage = global::SurgeryHelper.Properties.Resources.open16;
            this.buttonOpen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonOpen.FlatAppearance.BorderSize = 0;
            this.buttonOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOpen.Location = new System.Drawing.Point(720, 32);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(20, 20);
            this.buttonOpen.TabIndex = 77;
            this.buttonOpen.TabStop = false;
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            this.buttonOpen.MouseEnter += new System.EventHandler(this.buttonOpen_MouseEnter);
            this.buttonOpen.MouseLeave += new System.EventHandler(this.buttonOpen_MouseLeave);
            // 
            // buttonOk
            // 
            this.buttonOk.BackgroundImage = global::SurgeryHelper.Properties.Resources.OK;
            this.buttonOk.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonOk.Enabled = false;
            this.buttonOk.FlatAppearance.BorderSize = 0;
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOk.Location = new System.Drawing.Point(654, 449);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(40, 40);
            this.buttonOk.TabIndex = 60;
            this.buttonOk.TabStop = false;
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            this.buttonOk.MouseEnter += new System.EventHandler(this.buttonOk_MouseEnter);
            this.buttonOk.MouseLeave += new System.EventHandler(this.buttonOk_MouseLeave);
            // 
            // buttonClose
            // 
            this.buttonClose.BackgroundImage = global::SurgeryHelper.Properties.Resources.close;
            this.buttonClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonClose.FlatAppearance.BorderSize = 0;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.Location = new System.Drawing.Point(700, 449);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(40, 40);
            this.buttonClose.TabIndex = 59;
            this.buttonClose.TabStop = false;
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            this.buttonClose.MouseEnter += new System.EventHandler(this.buttonClose_MouseEnter);
            this.buttonClose.MouseLeave += new System.EventHandler(this.buttonClose_MouseLeave);
            // 
            // checkedListBoxForeignNosology
            // 
            this.checkedListBoxForeignNosology.CheckOnClick = true;
            this.checkedListBoxForeignNosology.FormattingEnabled = true;
            this.checkedListBoxForeignNosology.Location = new System.Drawing.Point(382, 125);
            this.checkedListBoxForeignNosology.Name = "checkedListBoxForeignNosology";
            this.checkedListBoxForeignNosology.ScrollAlwaysVisible = true;
            this.checkedListBoxForeignNosology.Size = new System.Drawing.Size(358, 319);
            this.checkedListBoxForeignNosology.TabIndex = 4;
            // 
            // checkBoxSelectAllPatients
            // 
            this.checkBoxSelectAllPatients.AutoSize = true;
            this.checkBoxSelectAllPatients.Location = new System.Drawing.Point(10, 102);
            this.checkBoxSelectAllPatients.Name = "checkBoxSelectAllPatients";
            this.checkBoxSelectAllPatients.Size = new System.Drawing.Size(358, 17);
            this.checkBoxSelectAllPatients.TabIndex = 85;
            this.checkBoxSelectAllPatients.Text = "Выберите пациентов, которые будут импортированы в нашу базу";
            this.checkBoxSelectAllPatients.UseVisualStyleBackColor = true;
            this.checkBoxSelectAllPatients.CheckedChanged += new System.EventHandler(this.checkBoxSelectAllPatients_CheckedChanged);
            // 
            // checkBoxSelectAllNosologies
            // 
            this.checkBoxSelectAllNosologies.AutoSize = true;
            this.checkBoxSelectAllNosologies.Location = new System.Drawing.Point(382, 102);
            this.checkBoxSelectAllNosologies.Name = "checkBoxSelectAllNosologies";
            this.checkBoxSelectAllNosologies.Size = new System.Drawing.Size(358, 17);
            this.checkBoxSelectAllNosologies.TabIndex = 86;
            this.checkBoxSelectAllNosologies.Text = "Выберите нозологии, которые будут импортированы в нашу базу";
            this.checkBoxSelectAllNosologies.UseVisualStyleBackColor = true;
            this.checkBoxSelectAllNosologies.CheckedChanged += new System.EventHandler(this.checkBoxSelectAllNosologies_CheckedChanged);
            // 
            // ImportDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 493);
            this.Controls.Add(this.checkBoxSelectAllNosologies);
            this.Controls.Add(this.checkBoxSelectAllPatients);
            this.Controls.Add(this.checkedListBoxForeignNosology);
            this.Controls.Add(this.buttonGetData);
            this.Controls.Add(this.checkedListBoxForeignPatients);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportDataForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Импорт данных из других файлов баз данных";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox checkedListBoxForeignPatients;
        private System.Windows.Forms.Button buttonGetData;
        private System.Windows.Forms.CheckedListBox checkedListBoxForeignNosology;
        private System.Windows.Forms.CheckBox checkBoxSelectAllPatients;
        private System.Windows.Forms.CheckBox checkBoxSelectAllNosologies;
    }
}