namespace AppDomainTest
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
            this.btnStart = new System.Windows.Forms.Button();
            this.lbInfos = new System.Windows.Forms.ListBox();
            this.btnClearInfos = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.nStackUsage = new System.Windows.Forms.NumericUpDown();
            this.btnCollect = new System.Windows.Forms.Button();
            this.btnTPoolEx = new System.Windows.Forms.Button();
            this.btnSetPool = new System.Windows.Forms.Button();
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
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(34, 95);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lbInfos
            // 
            this.lbInfos.FormattingEnabled = true;
            this.lbInfos.Location = new System.Drawing.Point(157, 95);
            this.lbInfos.Name = "lbInfos";
            this.lbInfos.ScrollAlwaysVisible = true;
            this.lbInfos.Size = new System.Drawing.Size(375, 134);
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
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(34, 134);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnConnectApplication_Click);
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
            1,
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
            // btnTPoolEx
            // 
            this.btnTPoolEx.Location = new System.Drawing.Point(34, 260);
            this.btnTPoolEx.Name = "btnTPoolEx";
            this.btnTPoolEx.Size = new System.Drawing.Size(75, 23);
            this.btnTPoolEx.TabIndex = 12;
            this.btnTPoolEx.Text = "TPool Ex";
            this.btnTPoolEx.UseVisualStyleBackColor = true;
            this.btnTPoolEx.Click += new System.EventHandler(this.btnTPoolEx_Click);
            // 
            // btnSetPool
            // 
            this.btnSetPool.Location = new System.Drawing.Point(132, 260);
            this.btnSetPool.Name = "btnSetPool";
            this.btnSetPool.Size = new System.Drawing.Size(75, 23);
            this.btnSetPool.TabIndex = 13;
            this.btnSetPool.Text = "Set Pool";
            this.btnSetPool.UseVisualStyleBackColor = true;
            this.btnSetPool.Click += new System.EventHandler(this.btnSetPool_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 310);
            this.Controls.Add(this.btnSetPool);
            this.Controls.Add(this.btnTPoolEx);
            this.Controls.Add(this.btnCollect);
            this.Controls.Add(this.nStackUsage);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnClearInfos);
            this.Controls.Add(this.lbInfos);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.nCount);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCreate);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nStackUsage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.NumericUpDown nCount;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ListBox lbInfos;
        private System.Windows.Forms.Button btnClearInfos;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.NumericUpDown nStackUsage;
        private System.Windows.Forms.Button btnCollect;
        private System.Windows.Forms.Button btnTPoolEx;
        private System.Windows.Forms.Button btnSetPool;
    }
}

