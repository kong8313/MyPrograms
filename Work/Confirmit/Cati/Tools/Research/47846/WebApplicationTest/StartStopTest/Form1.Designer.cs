namespace StartStopTest
{
    partial class Form1
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
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.nCount = new System.Windows.Forms.NumericUpDown();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lbInfos = new System.Windows.Forms.ListBox();
            this.btnClearInfos = new System.Windows.Forms.Button();
            this.btnConnectApplication = new System.Windows.Forms.Button();
            this.nStackUsage = new System.Windows.Forms.NumericUpDown();
            this.btnCollect = new System.Windows.Forms.Button();
            this.btnThreadPoolInfo = new System.Windows.Forms.Button();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.cbRandomStart = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nStackUsage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(34, 13);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 0;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreateClick);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(34, 52);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDeleteClick);
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(157, 52);
            this.pbProgress.Maximum = 3000;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(375, 23);
            this.pbProgress.TabIndex = 2;
            this.pbProgress.Value = 1;
            // 
            // nCount
            // 
            this.nCount.Location = new System.Drawing.Point(157, 15);
            this.nCount.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nCount.Name = "nCount";
            this.nCount.Size = new System.Drawing.Size(120, 20);
            this.nCount.TabIndex = 3;
            this.nCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(34, 95);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Start";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lbInfos
            // 
            this.lbInfos.FormattingEnabled = true;
            this.lbInfos.Location = new System.Drawing.Point(157, 95);
            this.lbInfos.Name = "lbInfos";
            this.lbInfos.ScrollAlwaysVisible = true;
            this.lbInfos.Size = new System.Drawing.Size(375, 160);
            this.lbInfos.TabIndex = 7;
            // 
            // btnClearInfos
            // 
            this.btnClearInfos.Location = new System.Drawing.Point(34, 216);
            this.btnClearInfos.Name = "btnClearInfos";
            this.btnClearInfos.Size = new System.Drawing.Size(75, 23);
            this.btnClearInfos.TabIndex = 8;
            this.btnClearInfos.Text = "Clear Infos";
            this.btnClearInfos.UseVisualStyleBackColor = true;
            this.btnClearInfos.Click += new System.EventHandler(this.btnClearInfos_Click);
            // 
            // btnConnectApplication
            // 
            this.btnConnectApplication.Location = new System.Drawing.Point(34, 134);
            this.btnConnectApplication.Name = "btnConnectApplication";
            this.btnConnectApplication.Size = new System.Drawing.Size(75, 23);
            this.btnConnectApplication.TabIndex = 9;
            this.btnConnectApplication.Text = "Stop";
            this.btnConnectApplication.UseVisualStyleBackColor = true;
            this.btnConnectApplication.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // nStackUsage
            // 
            this.nStackUsage.Location = new System.Drawing.Point(296, 15);
            this.nStackUsage.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nStackUsage.Name = "nStackUsage";
            this.nStackUsage.Size = new System.Drawing.Size(120, 20);
            this.nStackUsage.TabIndex = 10;
            this.nStackUsage.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // btnCollect
            // 
            this.btnCollect.Location = new System.Drawing.Point(34, 173);
            this.btnCollect.Name = "btnCollect";
            this.btnCollect.Size = new System.Drawing.Size(75, 23);
            this.btnCollect.TabIndex = 11;
            this.btnCollect.Text = "GC.Collect";
            this.btnCollect.UseVisualStyleBackColor = true;
            this.btnCollect.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnThreadPoolInfo
            // 
            this.btnThreadPoolInfo.Location = new System.Drawing.Point(34, 259);
            this.btnThreadPoolInfo.Name = "btnThreadPoolInfo";
            this.btnThreadPoolInfo.Size = new System.Drawing.Size(75, 23);
            this.btnThreadPoolInfo.TabIndex = 12;
            this.btnThreadPoolInfo.Text = "Pool Info";
            this.btnThreadPoolInfo.UseVisualStyleBackColor = true;
            this.btnThreadPoolInfo.Click += new System.EventHandler(this.btnThreadPoolInfo_Click);
            // 
            // cbType
            // 
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            "Sql Thread",
            "Sql Timer",
            "Threading.Timer",
            "Timers.Timer",
            "JustThread"});
            this.cbType.Location = new System.Drawing.Point(434, 13);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(98, 21);
            this.cbType.TabIndex = 13;
            // 
            // cbRandomStart
            // 
            this.cbRandomStart.AutoSize = true;
            this.cbRandomStart.Location = new System.Drawing.Point(539, 15);
            this.cbRandomStart.Name = "cbRandomStart";
            this.cbRandomStart.Size = new System.Drawing.Size(91, 17);
            this.cbRandomStart.TabIndex = 14;
            this.cbRandomStart.Text = "Random Start";
            this.cbRandomStart.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 310);
            this.Controls.Add(this.cbRandomStart);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.btnThreadPoolInfo);
            this.Controls.Add(this.btnCollect);
            this.Controls.Add(this.nStackUsage);
            this.Controls.Add(this.btnConnectApplication);
            this.Controls.Add(this.btnClearInfos);
            this.Controls.Add(this.lbInfos);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.nCount);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCreate);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nStackUsage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.NumericUpDown nCount;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ListBox lbInfos;
        private System.Windows.Forms.Button btnClearInfos;
        private System.Windows.Forms.Button btnConnectApplication;
        private System.Windows.Forms.NumericUpDown nStackUsage;
        private System.Windows.Forms.Button btnCollect;
        private System.Windows.Forms.Button btnThreadPoolInfo;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.CheckBox cbRandomStart;
    }
}

