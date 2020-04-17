namespace LinesG
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.finishGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.howToPlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lidersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxFuture3 = new System.Windows.Forms.PictureBox();
            this.pictureBoxFuture2 = new System.Windows.Forms.PictureBox();
            this.pictureBoxFuture1 = new System.Windows.Forms.PictureBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelScore = new System.Windows.Forms.Label();
            this.labelMaxScore = new System.Windows.Forms.Label();
            this.panelField = new System.Windows.Forms.Panel();
            this.timerGame = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFuture3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFuture2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFuture1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.gameToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.lidersToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(409, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.fileToolStripMenuItem.Text = "&Файл";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "&Сохранить...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadToolStripMenuItem.Text = "&Загрузить...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "&Выход";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.stepBackToolStripMenuItem,
            this.finishGameToolStripMenuItem});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.gameToolStripMenuItem.Text = "&Игра";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.newToolStripMenuItem.Text = "&Новая";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // stepBackToolStripMenuItem
            // 
            this.stepBackToolStripMenuItem.Enabled = false;
            this.stepBackToolStripMenuItem.Name = "stepBackToolStripMenuItem";
            this.stepBackToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.stepBackToolStripMenuItem.Text = "&Шаг назад";
            this.stepBackToolStripMenuItem.Click += new System.EventHandler(this.stepBackToolStripMenuItem_Click);
            // 
            // finishGameToolStripMenuItem
            // 
            this.finishGameToolStripMenuItem.Name = "finishGameToolStripMenuItem";
            this.finishGameToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.finishGameToolStripMenuItem.Text = "&Закончить";
            this.finishGameToolStripMenuItem.Click += new System.EventHandler(this.finishGameToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.howToPlayToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.helpToolStripMenuItem.Text = "&Помощь";
            // 
            // howToPlayToolStripMenuItem
            // 
            this.howToPlayToolStripMenuItem.Name = "howToPlayToolStripMenuItem";
            this.howToPlayToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.howToPlayToolStripMenuItem.Text = "&Как играть...";
            this.howToPlayToolStripMenuItem.Click += new System.EventHandler(this.howToPlayToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.aboutToolStripMenuItem.Text = "&О программе...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // lidersToolStripMenuItem
            // 
            this.lidersToolStripMenuItem.Name = "lidersToolStripMenuItem";
            this.lidersToolStripMenuItem.Size = new System.Drawing.Size(116, 20);
            this.lidersToolStripMenuItem.Text = "&Таблица Лидеров";
            this.lidersToolStripMenuItem.Click += new System.EventHandler(this.lidersToolStripMenuItem_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.Black;
            this.panelTop.Controls.Add(this.pictureBox1);
            this.panelTop.Controls.Add(this.pictureBoxFuture3);
            this.panelTop.Controls.Add(this.pictureBoxFuture2);
            this.panelTop.Controls.Add(this.pictureBoxFuture1);
            this.panelTop.Controls.Add(this.labelTime);
            this.panelTop.Controls.Add(this.labelScore);
            this.panelTop.Controls.Add(this.labelMaxScore);
            this.panelTop.Location = new System.Drawing.Point(0, 30);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(409, 45);
            this.panelTop.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::LinesG.Properties.Resources.Lines;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Location = new System.Drawing.Point(189, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(25, 8);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBoxFuture3
            // 
            this.pictureBoxFuture3.BackgroundImage = global::LinesG.Properties.Resources.Aqua_black;
            this.pictureBoxFuture3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxFuture3.Location = new System.Drawing.Point(212, 8);
            this.pictureBoxFuture3.Name = "pictureBoxFuture3";
            this.pictureBoxFuture3.Size = new System.Drawing.Size(25, 25);
            this.pictureBoxFuture3.TabIndex = 7;
            this.pictureBoxFuture3.TabStop = false;
            this.pictureBoxFuture3.Visible = false;
            // 
            // pictureBoxFuture2
            // 
            this.pictureBoxFuture2.BackgroundImage = global::LinesG.Properties.Resources.Aqua_black;
            this.pictureBoxFuture2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxFuture2.Location = new System.Drawing.Point(187, 8);
            this.pictureBoxFuture2.Name = "pictureBoxFuture2";
            this.pictureBoxFuture2.Size = new System.Drawing.Size(25, 25);
            this.pictureBoxFuture2.TabIndex = 6;
            this.pictureBoxFuture2.TabStop = false;
            this.pictureBoxFuture2.Visible = false;
            // 
            // pictureBoxFuture1
            // 
            this.pictureBoxFuture1.BackgroundImage = global::LinesG.Properties.Resources.Aqua_black;
            this.pictureBoxFuture1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxFuture1.Location = new System.Drawing.Point(162, 8);
            this.pictureBoxFuture1.Name = "pictureBoxFuture1";
            this.pictureBoxFuture1.Size = new System.Drawing.Size(25, 25);
            this.pictureBoxFuture1.TabIndex = 5;
            this.pictureBoxFuture1.TabStop = false;
            this.pictureBoxFuture1.Visible = false;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTime.ForeColor = System.Drawing.Color.SkyBlue;
            this.labelTime.Location = new System.Drawing.Point(180, 30);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(44, 16);
            this.labelTime.TabIndex = 3;
            this.labelTime.Text = "0:00:00";
            // 
            // labelScore
            // 
            this.labelScore.AutoSize = true;
            this.labelScore.Font = new System.Drawing.Font("Arial Narrow", 38F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelScore.ForeColor = System.Drawing.Color.SkyBlue;
            this.labelScore.Location = new System.Drawing.Point(271, -6);
            this.labelScore.Name = "labelScore";
            this.labelScore.Size = new System.Drawing.Size(140, 59);
            this.labelScore.TabIndex = 2;
            this.labelScore.Text = "00000";
            // 
            // labelMaxScore
            // 
            this.labelMaxScore.AutoSize = true;
            this.labelMaxScore.Font = new System.Drawing.Font("Arial Narrow", 38F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMaxScore.ForeColor = System.Drawing.Color.SkyBlue;
            this.labelMaxScore.Location = new System.Drawing.Point(0, -7);
            this.labelMaxScore.Name = "labelMaxScore";
            this.labelMaxScore.Size = new System.Drawing.Size(140, 59);
            this.labelMaxScore.TabIndex = 1;
            this.labelMaxScore.Text = "00000";
            // 
            // panelField
            // 
            this.panelField.Location = new System.Drawing.Point(0, 77);
            this.panelField.Name = "panelField";
            this.panelField.Size = new System.Drawing.Size(408, 400);
            this.panelField.TabIndex = 5;
            this.panelField.Visible = false;
            this.panelField.Paint += new System.Windows.Forms.PaintEventHandler(this.panelField_Paint);
            // 
            // timerGame
            // 
            this.timerGame.Interval = 1000;
            this.timerGame.Tick += new System.EventHandler(this.timerGame_Tick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Line files|*.lns|All files|*.*";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Line files|*.lns|All files|*.*";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 542);
            this.Controls.Add(this.panelField);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Шарики";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFuture3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFuture2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFuture1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lidersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepBackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem finishGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem howToPlayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelField;
        private System.Windows.Forms.Label labelMaxScore;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelScore;
        private System.Windows.Forms.PictureBox pictureBoxFuture1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBoxFuture3;
        private System.Windows.Forms.PictureBox pictureBoxFuture2;
        private System.Windows.Forms.Timer timerGame;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

