namespace CatiEncoder
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
            this.buttonEncryptCatiSqlServerName = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxCurrentConfirmitSqlServerName = new System.Windows.Forms.TextBox();
            this.textBoxCurrentSurveyC = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCurrentSessionStateConnectionString = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxCurrentConfirmlogConnectionString = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxCurrentConfirmConnectionString = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxCurrentCatiSqlServerName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxNewSurveyC = new System.Windows.Forms.TextBox();
            this.textBoxNewConfirmitSqlServerName = new System.Windows.Forms.TextBox();
            this.buttonEncryptConfirmitSettings = new System.Windows.Forms.Button();
            this.buttonEncryptSessionStateConnectionString = new System.Windows.Forms.Button();
            this.buttonEncryptConfirmlogConnectionString = new System.Windows.Forms.Button();
            this.labelInfo = new System.Windows.Forms.Label();
            this.buttonEncryptConfirmConnectionString = new System.Windows.Forms.Button();
            this.textBoxNewSessionStateConnectionString = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxNewConfirmlogConnectionString = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxNewConfirmConnectionString = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxNewCatiSqlServerName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonWebUtility = new System.Windows.Forms.RadioButton();
            this.radioButtonMachineKeyEncryptionMonitorIdentInfo = new System.Windows.Forms.RadioButton();
            this.radioButtonCryptComp = new System.Windows.Forms.RadioButton();
            this.radioButtonSecurityHelper = new System.Windows.Forms.RadioButton();
            this.richTextBoxDecryptedString = new System.Windows.Forms.RichTextBox();
            this.richTextBoxEncryptedString = new System.Windows.Forms.RichTextBox();
            this.buttonEncrypt = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.buttonDecrypt = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonEncryptCatiSqlServerName
            // 
            this.buttonEncryptCatiSqlServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEncryptCatiSqlServerName.Location = new System.Drawing.Point(612, 74);
            this.buttonEncryptCatiSqlServerName.Name = "buttonEncryptCatiSqlServerName";
            this.buttonEncryptCatiSqlServerName.Size = new System.Drawing.Size(155, 24);
            this.buttonEncryptCatiSqlServerName.TabIndex = 4;
            this.buttonEncryptCatiSqlServerName.Text = "Encrypt";
            this.buttonEncryptCatiSqlServerName.UseVisualStyleBackColor = true;
            this.buttonEncryptCatiSqlServerName.Click += new System.EventHandler(this.ButtonEncryptCatiSqlServerNameClick);
            this.buttonEncryptCatiSqlServerName.MouseEnter += new System.EventHandler(this.buttonLostChangesWarning_MouseEnter);
            this.buttonEncryptCatiSqlServerName.MouseLeave += new System.EventHandler(this.buttonLostChangesWarning_MouseLeave);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.textBoxCurrentConfirmitSqlServerName);
            this.groupBox1.Controls.Add(this.textBoxCurrentSurveyC);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxCurrentSessionStateConnectionString);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBoxCurrentConfirmlogConnectionString);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxCurrentConfirmConnectionString);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBoxCurrentCatiSqlServerName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(775, 206);
            this.groupBox1.TabIndex = 100;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Currect Values";
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(203, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(367, 13);
            this.label14.TabIndex = 13;
            this.label14.Text = "Confirmit SurveyC Credentials";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxCurrentConfirmitSqlServerName
            // 
            this.textBoxCurrentConfirmitSqlServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCurrentConfirmitSqlServerName.Location = new System.Drawing.Point(5, 32);
            this.textBoxCurrentConfirmitSqlServerName.Name = "textBoxCurrentConfirmitSqlServerName";
            this.textBoxCurrentConfirmitSqlServerName.ReadOnly = true;
            this.textBoxCurrentConfirmitSqlServerName.Size = new System.Drawing.Size(155, 20);
            this.textBoxCurrentConfirmitSqlServerName.TabIndex = 12;
            this.textBoxCurrentConfirmitSqlServerName.TabStop = false;
            // 
            // textBoxCurrentSurveyC
            // 
            this.textBoxCurrentSurveyC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCurrentSurveyC.Location = new System.Drawing.Point(203, 32);
            this.textBoxCurrentSurveyC.Name = "textBoxCurrentSurveyC";
            this.textBoxCurrentSurveyC.ReadOnly = true;
            this.textBoxCurrentSurveyC.Size = new System.Drawing.Size(367, 20);
            this.textBoxCurrentSurveyC.TabIndex = 2;
            this.textBoxCurrentSurveyC.TabStop = false;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(5, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Confirmit SQL Server Name";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxCurrentSessionStateConnectionString
            // 
            this.textBoxCurrentSessionStateConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCurrentSessionStateConnectionString.Location = new System.Drawing.Point(5, 177);
            this.textBoxCurrentSessionStateConnectionString.Name = "textBoxCurrentSessionStateConnectionString";
            this.textBoxCurrentSessionStateConnectionString.ReadOnly = true;
            this.textBoxCurrentSessionStateConnectionString.Size = new System.Drawing.Size(762, 20);
            this.textBoxCurrentSessionStateConnectionString.TabIndex = 10;
            this.textBoxCurrentSessionStateConnectionString.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(5, 161);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(258, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Session State Connection String in BvSystemSettings";
            // 
            // textBoxCurrentConfirmlogConnectionString
            // 
            this.textBoxCurrentConfirmlogConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCurrentConfirmlogConnectionString.Location = new System.Drawing.Point(5, 129);
            this.textBoxCurrentConfirmlogConnectionString.Name = "textBoxCurrentConfirmlogConnectionString";
            this.textBoxCurrentConfirmlogConnectionString.ReadOnly = true;
            this.textBoxCurrentConfirmlogConnectionString.Size = new System.Drawing.Size(762, 20);
            this.textBoxCurrentConfirmlogConnectionString.TabIndex = 8;
            this.textBoxCurrentConfirmlogConnectionString.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(5, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(242, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Confirmlog Connection String in BvSystemSettings";
            // 
            // textBoxCurrentConfirmConnectionString
            // 
            this.textBoxCurrentConfirmConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCurrentConfirmConnectionString.Location = new System.Drawing.Point(5, 80);
            this.textBoxCurrentConfirmConnectionString.Name = "textBoxCurrentConfirmConnectionString";
            this.textBoxCurrentConfirmConnectionString.ReadOnly = true;
            this.textBoxCurrentConfirmConnectionString.Size = new System.Drawing.Size(762, 20);
            this.textBoxCurrentConfirmConnectionString.TabIndex = 6;
            this.textBoxCurrentConfirmConnectionString.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(5, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(228, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Confirm Connection String in BvSystemSettings";
            // 
            // textBoxCurrentCatiSqlServerName
            // 
            this.textBoxCurrentCatiSqlServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCurrentCatiSqlServerName.Location = new System.Drawing.Point(612, 32);
            this.textBoxCurrentCatiSqlServerName.Name = "textBoxCurrentCatiSqlServerName";
            this.textBoxCurrentCatiSqlServerName.ReadOnly = true;
            this.textBoxCurrentCatiSqlServerName.Size = new System.Drawing.Size(155, 20);
            this.textBoxCurrentCatiSqlServerName.TabIndex = 4;
            this.textBoxCurrentCatiSqlServerName.TabStop = false;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(612, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "CATI SQL Server Name";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.textBoxNewSurveyC);
            this.groupBox2.Controls.Add(this.textBoxNewConfirmitSqlServerName);
            this.groupBox2.Controls.Add(this.buttonEncryptConfirmitSettings);
            this.groupBox2.Controls.Add(this.buttonEncryptSessionStateConnectionString);
            this.groupBox2.Controls.Add(this.buttonEncryptConfirmlogConnectionString);
            this.groupBox2.Controls.Add(this.labelInfo);
            this.groupBox2.Controls.Add(this.buttonEncryptConfirmConnectionString);
            this.groupBox2.Controls.Add(this.textBoxNewSessionStateConnectionString);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBoxNewConfirmlogConnectionString);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBoxNewConfirmConnectionString);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.textBoxNewCatiSqlServerName);
            this.groupBox2.Controls.Add(this.buttonEncryptCatiSqlServerName);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 225);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(775, 200);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Setting New Values";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(203, 34);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(367, 13);
            this.label9.TabIndex = 112;
            this.label9.Text = "Confirmit SurveyC Credentials";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(12, 34);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(155, 13);
            this.label11.TabIndex = 111;
            this.label11.Text = "Confirmit SQL Server Name";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(612, 34);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(155, 13);
            this.label15.TabIndex = 110;
            this.label15.Text = "CATI SQL Server Name";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxNewSurveyC
            // 
            this.textBoxNewSurveyC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNewSurveyC.Location = new System.Drawing.Point(203, 50);
            this.textBoxNewSurveyC.Name = "textBoxNewSurveyC";
            this.textBoxNewSurveyC.Size = new System.Drawing.Size(367, 20);
            this.textBoxNewSurveyC.TabIndex = 108;
            // 
            // textBoxNewConfirmitSqlServerName
            // 
            this.textBoxNewConfirmitSqlServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNewConfirmitSqlServerName.Location = new System.Drawing.Point(12, 50);
            this.textBoxNewConfirmitSqlServerName.Name = "textBoxNewConfirmitSqlServerName";
            this.textBoxNewConfirmitSqlServerName.Size = new System.Drawing.Size(155, 20);
            this.textBoxNewConfirmitSqlServerName.TabIndex = 1;
            // 
            // buttonEncryptConfirmitSettings
            // 
            this.buttonEncryptConfirmitSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEncryptConfirmitSettings.Location = new System.Drawing.Point(12, 74);
            this.buttonEncryptConfirmitSettings.Name = "buttonEncryptConfirmitSettings";
            this.buttonEncryptConfirmitSettings.Size = new System.Drawing.Size(558, 24);
            this.buttonEncryptConfirmitSettings.TabIndex = 2;
            this.buttonEncryptConfirmitSettings.Text = "Encrypt";
            this.buttonEncryptConfirmitSettings.UseVisualStyleBackColor = true;
            this.buttonEncryptConfirmitSettings.Click += new System.EventHandler(this.buttonEncryptConfirmitConnectionString_Click);
            this.buttonEncryptConfirmitSettings.MouseEnter += new System.EventHandler(this.buttonLostChangesWarning_MouseEnter);
            this.buttonEncryptConfirmitSettings.MouseLeave += new System.EventHandler(this.buttonLostChangesWarning_MouseLeave);
            // 
            // buttonEncryptSessionStateConnectionString
            // 
            this.buttonEncryptSessionStateConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEncryptSessionStateConnectionString.Location = new System.Drawing.Point(695, 164);
            this.buttonEncryptSessionStateConnectionString.Name = "buttonEncryptSessionStateConnectionString";
            this.buttonEncryptSessionStateConnectionString.Size = new System.Drawing.Size(74, 24);
            this.buttonEncryptSessionStateConnectionString.TabIndex = 10;
            this.buttonEncryptSessionStateConnectionString.Text = "Encrypt";
            this.buttonEncryptSessionStateConnectionString.UseVisualStyleBackColor = true;
            this.buttonEncryptSessionStateConnectionString.Click += new System.EventHandler(this.ButtonEncryptSessionStateConnectionStringClick);
            // 
            // buttonEncryptConfirmlogConnectionString
            // 
            this.buttonEncryptConfirmlogConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEncryptConfirmlogConnectionString.Location = new System.Drawing.Point(695, 134);
            this.buttonEncryptConfirmlogConnectionString.Name = "buttonEncryptConfirmlogConnectionString";
            this.buttonEncryptConfirmlogConnectionString.Size = new System.Drawing.Size(74, 24);
            this.buttonEncryptConfirmlogConnectionString.TabIndex = 8;
            this.buttonEncryptConfirmlogConnectionString.Text = "Encrypt";
            this.buttonEncryptConfirmlogConnectionString.UseVisualStyleBackColor = true;
            this.buttonEncryptConfirmlogConnectionString.Click += new System.EventHandler(this.ButtonEncryptConfirmlogConnectionStringClick);
            // 
            // labelInfo
            // 
            this.labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.ForeColor = System.Drawing.Color.Red;
            this.labelInfo.Location = new System.Drawing.Point(81, 13);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(590, 18);
            this.labelInfo.TabIndex = 104;
            this.labelInfo.Text = "Be careful: changes are committed immediately after pressing any Encrypt button";
            this.labelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonEncryptConfirmConnectionString
            // 
            this.buttonEncryptConfirmConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEncryptConfirmConnectionString.Location = new System.Drawing.Point(694, 104);
            this.buttonEncryptConfirmConnectionString.Name = "buttonEncryptConfirmConnectionString";
            this.buttonEncryptConfirmConnectionString.Size = new System.Drawing.Size(74, 24);
            this.buttonEncryptConfirmConnectionString.TabIndex = 6;
            this.buttonEncryptConfirmConnectionString.Text = "Encrypt";
            this.buttonEncryptConfirmConnectionString.UseVisualStyleBackColor = true;
            this.buttonEncryptConfirmConnectionString.Click += new System.EventHandler(this.ButtonEncryptConfirmConnectionStringClick);
            // 
            // textBoxNewSessionStateConnectionString
            // 
            this.textBoxNewSessionStateConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNewSessionStateConnectionString.Location = new System.Drawing.Point(170, 167);
            this.textBoxNewSessionStateConnectionString.Name = "textBoxNewSessionStateConnectionString";
            this.textBoxNewSessionStateConnectionString.Size = new System.Drawing.Size(518, 20);
            this.textBoxNewSessionStateConnectionString.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 170);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Session State Connection String";
            // 
            // textBoxNewConfirmlogConnectionString
            // 
            this.textBoxNewConfirmlogConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNewConfirmlogConnectionString.Location = new System.Drawing.Point(170, 137);
            this.textBoxNewConfirmlogConnectionString.Name = "textBoxNewConfirmlogConnectionString";
            this.textBoxNewConfirmlogConnectionString.Size = new System.Drawing.Size(518, 20);
            this.textBoxNewConfirmlogConnectionString.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(9, 140);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(143, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Confirmlog Connection String";
            // 
            // textBoxNewConfirmConnectionString
            // 
            this.textBoxNewConfirmConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNewConfirmConnectionString.Location = new System.Drawing.Point(170, 107);
            this.textBoxNewConfirmConnectionString.Name = "textBoxNewConfirmConnectionString";
            this.textBoxNewConfirmConnectionString.Size = new System.Drawing.Size(518, 20);
            this.textBoxNewConfirmConnectionString.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(9, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(129, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Confirm Connection String";
            // 
            // textBoxNewCatiSqlServerName
            // 
            this.textBoxNewCatiSqlServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNewCatiSqlServerName.Location = new System.Drawing.Point(612, 50);
            this.textBoxNewCatiSqlServerName.Name = "textBoxNewCatiSqlServerName";
            this.textBoxNewCatiSqlServerName.Size = new System.Drawing.Size(155, 20);
            this.textBoxNewCatiSqlServerName.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.richTextBoxDecryptedString);
            this.groupBox3.Controls.Add(this.richTextBoxEncryptedString);
            this.groupBox3.Controls.Add(this.buttonEncrypt);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.buttonDecrypt);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 439);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(775, 199);
            this.groupBox3.TabIndex = 102;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Quick Converter";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButtonWebUtility);
            this.groupBox4.Controls.Add(this.radioButtonMachineKeyEncryptionMonitorIdentInfo);
            this.groupBox4.Controls.Add(this.radioButtonCryptComp);
            this.groupBox4.Controls.Add(this.radioButtonSecurityHelper);
            this.groupBox4.Location = new System.Drawing.Point(12, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(756, 56);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Encryption method";
            // 
            // radioButtonWebUtility
            // 
            this.radioButtonWebUtility.AutoSize = true;
            this.radioButtonWebUtility.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonWebUtility.Location = new System.Drawing.Point(6, 24);
            this.radioButtonWebUtility.Name = "radioButtonWebUtility";
            this.radioButtonWebUtility.Size = new System.Drawing.Size(161, 17);
            this.radioButtonWebUtility.TabIndex = 10;
            this.radioButtonWebUtility.Text = "EncryptionUsingMachineKey";
            this.radioButtonWebUtility.UseVisualStyleBackColor = true;
            // 
            // radioButtonMachineKeyEncryptionMonitorIdentInfo
            // 
            this.radioButtonMachineKeyEncryptionMonitorIdentInfo.AutoSize = true;
            this.radioButtonMachineKeyEncryptionMonitorIdentInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonMachineKeyEncryptionMonitorIdentInfo.Location = new System.Drawing.Point(191, 17);
            this.radioButtonMachineKeyEncryptionMonitorIdentInfo.Name = "radioButtonMachineKeyEncryptionMonitorIdentInfo";
            this.radioButtonMachineKeyEncryptionMonitorIdentInfo.Size = new System.Drawing.Size(210, 30);
            this.radioButtonMachineKeyEncryptionMonitorIdentInfo.TabIndex = 3;
            this.radioButtonMachineKeyEncryptionMonitorIdentInfo.Text = "EncryptionUsingMachineKey\r\n(MonitoringIdentityInfo serialised object)";
            this.radioButtonMachineKeyEncryptionMonitorIdentInfo.UseVisualStyleBackColor = true;
            // 
            // radioButtonCryptComp
            // 
            this.radioButtonCryptComp.AutoSize = true;
            this.radioButtonCryptComp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonCryptComp.Location = new System.Drawing.Point(535, 24);
            this.radioButtonCryptComp.Name = "radioButtonCryptComp";
            this.radioButtonCryptComp.Size = new System.Drawing.Size(76, 17);
            this.radioButtonCryptComp.TabIndex = 5;
            this.radioButtonCryptComp.Text = "CryptComp";
            this.radioButtonCryptComp.UseVisualStyleBackColor = true;
            // 
            // radioButtonSecurityHelper
            // 
            this.radioButtonSecurityHelper.AutoSize = true;
            this.radioButtonSecurityHelper.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonSecurityHelper.Location = new System.Drawing.Point(422, 24);
            this.radioButtonSecurityHelper.Name = "radioButtonSecurityHelper";
            this.radioButtonSecurityHelper.Size = new System.Drawing.Size(94, 17);
            this.radioButtonSecurityHelper.TabIndex = 7;
            this.radioButtonSecurityHelper.Text = "SecurityHelper";
            this.radioButtonSecurityHelper.UseVisualStyleBackColor = true;
            // 
            // richTextBoxDecryptedString
            // 
            this.richTextBoxDecryptedString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxDecryptedString.Location = new System.Drawing.Point(12, 97);
            this.richTextBoxDecryptedString.Name = "richTextBoxDecryptedString";
            this.richTextBoxDecryptedString.Size = new System.Drawing.Size(325, 95);
            this.richTextBoxDecryptedString.TabIndex = 16;
            this.richTextBoxDecryptedString.Text = "";
            // 
            // richTextBoxEncryptedString
            // 
            this.richTextBoxEncryptedString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxEncryptedString.Location = new System.Drawing.Point(444, 97);
            this.richTextBoxEncryptedString.Name = "richTextBoxEncryptedString";
            this.richTextBoxEncryptedString.Size = new System.Drawing.Size(325, 95);
            this.richTextBoxEncryptedString.TabIndex = 14;
            this.richTextBoxEncryptedString.Text = "";
            // 
            // buttonEncrypt
            // 
            this.buttonEncrypt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEncrypt.Location = new System.Drawing.Point(354, 103);
            this.buttonEncrypt.Name = "buttonEncrypt";
            this.buttonEncrypt.Size = new System.Drawing.Size(74, 24);
            this.buttonEncrypt.TabIndex = 6;
            this.buttonEncrypt.Text = "Encrypt -->";
            this.buttonEncrypt.UseVisualStyleBackColor = true;
            this.buttonEncrypt.Click += new System.EventHandler(this.ButtonEncryptClick);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(586, 81);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 13);
            this.label12.TabIndex = 13;
            this.label12.Text = "Encrypted";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(145, 81);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 13);
            this.label13.TabIndex = 11;
            this.label13.Text = "Decrypted";
            // 
            // buttonDecrypt
            // 
            this.buttonDecrypt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDecrypt.Location = new System.Drawing.Point(354, 159);
            this.buttonDecrypt.Name = "buttonDecrypt";
            this.buttonDecrypt.Size = new System.Drawing.Size(74, 24);
            this.buttonDecrypt.TabIndex = 4;
            this.buttonDecrypt.Text = "<-- Decrypt";
            this.buttonDecrypt.UseVisualStyleBackColor = true;
            this.buttonDecrypt.Click += new System.EventHandler(this.ButtonDecryptClick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "scpo files|*.cspo";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 647);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CATI Encoder v5.0";
            this.Shown += new System.EventHandler(this.MainFormShown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonEncryptCatiSqlServerName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxCurrentSessionStateConnectionString;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxCurrentConfirmlogConnectionString;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxCurrentConfirmConnectionString;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxCurrentCatiSqlServerName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonEncryptSessionStateConnectionString;
        private System.Windows.Forms.Button buttonEncryptConfirmlogConnectionString;
        private System.Windows.Forms.Button buttonEncryptConfirmConnectionString;
        private System.Windows.Forms.TextBox textBoxNewSessionStateConnectionString;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxNewConfirmlogConnectionString;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxNewConfirmConnectionString;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxNewCatiSqlServerName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonEncrypt;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button buttonDecrypt;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RichTextBox richTextBoxEncryptedString;
        private System.Windows.Forms.RichTextBox richTextBoxDecryptedString;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButtonCryptComp;
        private System.Windows.Forms.RadioButton radioButtonSecurityHelper;
        private System.Windows.Forms.RadioButton radioButtonMachineKeyEncryptionMonitorIdentInfo;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.RadioButton radioButtonWebUtility;
        private System.Windows.Forms.TextBox textBoxCurrentSurveyC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxNewConfirmitSqlServerName;
        private System.Windows.Forms.Button buttonEncryptConfirmitSettings;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxCurrentConfirmitSqlServerName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxNewSurveyC;
    }
}

