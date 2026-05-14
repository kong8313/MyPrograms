namespace IISConfig
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
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.btnDebugOutTest = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lbInfos = new System.Windows.Forms.ListBox();
            this.btnClearInfos = new System.Windows.Forms.Button();
            this.btnConnectApplication = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nCount)).BeginInit();
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
            this.btnCreate.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(34, 52);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.button2_Click);
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
            // btnDeleteAll
            // 
            this.btnDeleteAll.Location = new System.Drawing.Point(34, 95);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteAll.TabIndex = 4;
            this.btnDeleteAll.Text = "Delete All";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // btnDebugOutTest
            // 
            this.btnDebugOutTest.Location = new System.Drawing.Point(34, 134);
            this.btnDebugOutTest.Name = "btnDebugOutTest";
            this.btnDebugOutTest.Size = new System.Drawing.Size(75, 23);
            this.btnDebugOutTest.TabIndex = 5;
            this.btnDebugOutTest.Text = "Debug Out Test";
            this.btnDebugOutTest.UseVisualStyleBackColor = true;
            this.btnDebugOutTest.Click += new System.EventHandler(this.btnDebugOutTest_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(34, 174);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
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
            this.btnConnectApplication.Location = new System.Drawing.Point(34, 254);
            this.btnConnectApplication.Name = "btnConnectApplication";
            this.btnConnectApplication.Size = new System.Drawing.Size(75, 23);
            this.btnConnectApplication.TabIndex = 9;
            this.btnConnectApplication.Text = "Connect A";
            this.btnConnectApplication.UseVisualStyleBackColor = true;
            this.btnConnectApplication.Click += new System.EventHandler(this.btnConnectApplication_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 310);
            this.Controls.Add(this.btnConnectApplication);
            this.Controls.Add(this.btnClearInfos);
            this.Controls.Add(this.lbInfos);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnDebugOutTest);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.nCount);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCreate);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.NumericUpDown nCount;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.Button btnDebugOutTest;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ListBox lbInfos;
        private System.Windows.Forms.Button btnClearInfos;
        private System.Windows.Forms.Button btnConnectApplication;
    }
}

