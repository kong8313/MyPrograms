namespace PreventLocking
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.labelInfo = new System.Windows.Forms.Label();
            this.buttonExcludeFromAutostart = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.textBoxServerName = new System.Windows.Forms.TextBox();
            this.textBoxSurveyC = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonRegistryRewrite = new System.Windows.Forms.Button();
            this.labelVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelInfo
            // 
            this.labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.Location = new System.Drawing.Point(11, 9);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(595, 263);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = resources.GetString("labelInfo.Text");
            // 
            // buttonExcludeFromAutostart
            // 
            this.buttonExcludeFromAutostart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExcludeFromAutostart.Location = new System.Drawing.Point(129, 344);
            this.buttonExcludeFromAutostart.Name = "buttonExcludeFromAutostart";
            this.buttonExcludeFromAutostart.Size = new System.Drawing.Size(174, 30);
            this.buttonExcludeFromAutostart.TabIndex = 1;
            this.buttonExcludeFromAutostart.Text = "Remove from autostart";
            this.buttonExcludeFromAutostart.UseVisualStyleBackColor = true;
            this.buttonExcludeFromAutostart.Click += new System.EventHandler(this.buttonExcludeFromAutostart_Click);
            // 
            // timer
            // 
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // textBoxServerName
            // 
            this.textBoxServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxServerName.Location = new System.Drawing.Point(129, 280);
            this.textBoxServerName.Name = "textBoxServerName";
            this.textBoxServerName.Size = new System.Drawing.Size(342, 22);
            this.textBoxServerName.TabIndex = 2;
            this.textBoxServerName.Text = "co-osl-tenta197";
            // 
            // textBoxSurveyC
            // 
            this.textBoxSurveyC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSurveyC.Location = new System.Drawing.Point(129, 305);
            this.textBoxSurveyC.Name = "textBoxSurveyC";
            this.textBoxSurveyC.Size = new System.Drawing.Size(342, 22);
            this.textBoxSurveyC.TabIndex = 3;
            this.textBoxSurveyC.Text = "UID=ConfirmitSurvey;PWD=%1confsur;";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 284);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "SQLServerName";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 309);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "SurveyC";
            // 
            // buttonRegistryRewrite
            // 
            this.buttonRegistryRewrite.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRegistryRewrite.Location = new System.Drawing.Point(477, 278);
            this.buttonRegistryRewrite.Name = "buttonRegistryRewrite";
            this.buttonRegistryRewrite.Size = new System.Drawing.Size(117, 49);
            this.buttonRegistryRewrite.TabIndex = 6;
            this.buttonRegistryRewrite.Text = "Rewrite registry values";
            this.buttonRegistryRewrite.UseVisualStyleBackColor = true;
            this.buttonRegistryRewrite.Click += new System.EventHandler(this.buttonRegistryRewrite_Click);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(563, 364);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(31, 13);
            this.labelVersion.TabIndex = 7;
            this.labelVersion.Text = "v.2.0";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 386);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.buttonRegistryRewrite);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSurveyC);
            this.Controls.Add(this.textBoxServerName);
            this.Controls.Add(this.buttonExcludeFromAutostart);
            this.Controls.Add(this.labelInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "CATI build server tuning app";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button buttonExcludeFromAutostart;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.TextBox textBoxServerName;
        private System.Windows.Forms.TextBox textBoxSurveyC;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonRegistryRewrite;
        private System.Windows.Forms.Label labelVersion;
    }
}

