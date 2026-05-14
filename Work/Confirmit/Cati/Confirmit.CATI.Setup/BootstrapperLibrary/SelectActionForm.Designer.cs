namespace BootstrapperLibrary
{
    partial class SelectActionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectActionForm));
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelProductName = new System.Windows.Forms.Label();
            this.pictureBoxTitle = new System.Windows.Forms.PictureBox();
            this.checkBoxWarning = new System.Windows.Forms.CheckBox();
            this.radioButtonUpdate = new System.Windows.Forms.RadioButton();
            this.radioButtonRemove = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(398, 332);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 42;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancelClick);
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(289, 332);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(75, 23);
            this.buttonNext.TabIndex = 41;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.ButtonNextClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(0, 317);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(495, 2);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.White;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTitle.Location = new System.Drawing.Point(42, 39);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(173, 16);
            this.labelTitle.TabIndex = 32;
            this.labelTitle.Text = "Select action you want to do";
            // 
            // labelProductName
            // 
            this.labelProductName.AutoSize = true;
            this.labelProductName.BackColor = System.Drawing.Color.White;
            this.labelProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelProductName.Location = new System.Drawing.Point(11, 15);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(311, 16);
            this.labelProductName.TabIndex = 29;
            this.labelProductName.Text = "Product Name BranchName ver.number type setup";
            // 
            // pictureBoxTitle
            // 
            this.pictureBoxTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBoxTitle.Image = global::BootstrapperLibrary.Properties.Resources.bannrbmp;
            this.pictureBoxTitle.InitialImage = null;
            this.pictureBoxTitle.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxTitle.Name = "pictureBoxTitle";
            this.pictureBoxTitle.Size = new System.Drawing.Size(494, 60);
            this.pictureBoxTitle.TabIndex = 30;
            this.pictureBoxTitle.TabStop = false;
            // 
            // checkBoxWarning
            // 
            this.checkBoxWarning.Location = new System.Drawing.Point(48, 189);
            this.checkBoxWarning.Name = "checkBoxWarning";
            this.checkBoxWarning.Size = new System.Drawing.Size(439, 45);
            this.checkBoxWarning.TabIndex = 39;
            this.checkBoxWarning.Text = "Warning: The product name version you are installing is the same as the version a" +
    "lready installed.\r\nMark the checkbox to continue the installation anyway\r\n";
            this.checkBoxWarning.UseVisualStyleBackColor = true;
            this.checkBoxWarning.CheckedChanged += new System.EventHandler(this.CheckBoxWarningCheckedChanged);
            // 
            // radioButtonUpdate
            // 
            this.radioButtonUpdate.Checked = true;
            this.radioButtonUpdate.Location = new System.Drawing.Point(25, 143);
            this.radioButtonUpdate.Name = "radioButtonUpdate";
            this.radioButtonUpdate.Size = new System.Drawing.Size(457, 34);
            this.radioButtonUpdate.TabIndex = 25;
            this.radioButtonUpdate.TabStop = true;
            this.radioButtonUpdate.Text = "Update Product name (Unattended Mode)";
            this.radioButtonUpdate.UseVisualStyleBackColor = true;
            // 
            // radioButtonRemove
            // 
            this.radioButtonRemove.AutoSize = true;
            this.radioButtonRemove.Location = new System.Drawing.Point(25, 109);
            this.radioButtonRemove.Name = "radioButtonRemove";
            this.radioButtonRemove.Size = new System.Drawing.Size(236, 17);
            this.radioButtonRemove.TabIndex = 23;
            this.radioButtonRemove.Text = "Remove product name ver. xx.xx.xxxx.xxxxxx";
            this.radioButtonRemove.UseVisualStyleBackColor = true;
            this.radioButtonRemove.CheckedChanged += new System.EventHandler(this.RadioButtonRemoveCheckedChanged);
            // 
            // SelectActionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 360);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.labelProductName);
            this.Controls.Add(this.pictureBoxTitle);
            this.Controls.Add(this.checkBoxWarning);
            this.Controls.Add(this.radioButtonUpdate);
            this.Controls.Add(this.radioButtonRemove);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectActionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SelectActionForm";
            this.Shown += new System.EventHandler(this.SelectActionFormShown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.PictureBox pictureBoxTitle;
        private System.Windows.Forms.CheckBox checkBoxWarning;
        private System.Windows.Forms.RadioButton radioButtonUpdate;
        private System.Windows.Forms.RadioButton radioButtonRemove;
    }
}