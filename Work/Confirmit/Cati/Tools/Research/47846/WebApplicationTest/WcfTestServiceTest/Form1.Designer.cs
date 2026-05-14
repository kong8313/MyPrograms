namespace WcfTestServiceTest
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
            this.btnGetId = new System.Windows.Forms.Button();
            this.rtbInfo = new System.Windows.Forms.RichTextBox();
            this.tbUrl = new System.Windows.Forms.TextBox();
            this.cbUseConfig = new System.Windows.Forms.CheckBox();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.cbUseSsl = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnGetId
            // 
            this.btnGetId.Location = new System.Drawing.Point(13, 13);
            this.btnGetId.Name = "btnGetId";
            this.btnGetId.Size = new System.Drawing.Size(75, 23);
            this.btnGetId.TabIndex = 0;
            this.btnGetId.Text = "Get Id";
            this.btnGetId.UseVisualStyleBackColor = true;
            this.btnGetId.Click += new System.EventHandler(this.btnGetId_Click);
            // 
            // rtbInfo
            // 
            this.rtbInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbInfo.Location = new System.Drawing.Point(111, 92);
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.Size = new System.Drawing.Size(640, 253);
            this.rtbInfo.TabIndex = 1;
            this.rtbInfo.Text = "";
            // 
            // tbUrl
            // 
            this.tbUrl.Location = new System.Drawing.Point(111, 43);
            this.tbUrl.Name = "tbUrl";
            this.tbUrl.Size = new System.Drawing.Size(640, 20);
            this.tbUrl.TabIndex = 2;
            this.tbUrl.Text = "http://localhost:1234/ws0/1.svc/test";
            // 
            // cbUseConfig
            // 
            this.cbUseConfig.AutoSize = true;
            this.cbUseConfig.Checked = true;
            this.cbUseConfig.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUseConfig.Location = new System.Drawing.Point(13, 43);
            this.cbUseConfig.Name = "cbUseConfig";
            this.cbUseConfig.Size = new System.Drawing.Size(77, 17);
            this.cbUseConfig.TabIndex = 3;
            this.cbUseConfig.Text = "Use config";
            this.cbUseConfig.UseVisualStyleBackColor = true;
            this.cbUseConfig.CheckedChanged += new System.EventHandler(this.cbUseConfig_CheckedChanged);
            // 
            // tbAddress
            // 
            this.tbAddress.Location = new System.Drawing.Point(111, 12);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(640, 20);
            this.tbAddress.TabIndex = 4;
            this.tbAddress.Text = "http://localhost:1234/ws0/1.svc/test";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(12, 322);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // cbUseSsl
            // 
            this.cbUseSsl.AutoSize = true;
            this.cbUseSsl.Location = new System.Drawing.Point(13, 76);
            this.cbUseSsl.Name = "cbUseSsl";
            this.cbUseSsl.Size = new System.Drawing.Size(68, 17);
            this.cbUseSsl.TabIndex = 6;
            this.cbUseSsl.Text = "Use SSL";
            this.cbUseSsl.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 357);
            this.Controls.Add(this.cbUseSsl);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.tbAddress);
            this.Controls.Add(this.cbUseConfig);
            this.Controls.Add(this.tbUrl);
            this.Controls.Add(this.rtbInfo);
            this.Controls.Add(this.btnGetId);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetId;
        private System.Windows.Forms.RichTextBox rtbInfo;
        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.CheckBox cbUseConfig;
        private System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.CheckBox cbUseSsl;
    }
}

