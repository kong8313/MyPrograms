namespace WaterSolver
{
    partial class WaterSolverForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pictureBoxField = new PictureBox();
            dataGridViewSteps = new DataGridView();
            buttonAddFlask = new Button();
            buttonRemoveFlask = new Button();
            pictureBoxCells = new PictureBox();
            buttonFindSolution = new Button();
            buttonLoadPosition = new Button();
            buttonSavePosition = new Button();
            saveFileDialog = new SaveFileDialog();
            openFileDialog = new OpenFileDialog();
            labelInfo1 = new Label();
            labelInfo2 = new Label();
            panelInfo = new Panel();
            labelInfo6 = new Label();
            labelInfo5 = new Label();
            labelInfo3 = new Label();
            labelInfo4 = new Label();
            timer = new System.Windows.Forms.Timer(components);
            Number = new DataGridViewTextBoxColumn();
            StepInfo = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)pictureBoxField).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewSteps).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCells).BeginInit();
            panelInfo.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBoxField
            // 
            pictureBoxField.Location = new Point(12, 12);
            pictureBoxField.Name = "pictureBoxField";
            pictureBoxField.Size = new Size(1000, 150);
            pictureBoxField.TabIndex = 0;
            pictureBoxField.TabStop = false;
            pictureBoxField.MouseClick += pictureBoxField_MouseClick;
            // 
            // dataGridViewSteps
            // 
            dataGridViewSteps.AllowUserToAddRows = false;
            dataGridViewSteps.AllowUserToDeleteRows = false;
            dataGridViewSteps.AllowUserToOrderColumns = true;
            dataGridViewSteps.AllowUserToResizeColumns = false;
            dataGridViewSteps.AllowUserToResizeRows = false;
            dataGridViewSteps.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewSteps.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewSteps.ColumnHeadersVisible = false;
            dataGridViewSteps.Columns.AddRange(new DataGridViewColumn[] { Number, StepInfo });
            dataGridViewSteps.GridColor = SystemColors.ControlLight;
            dataGridViewSteps.Location = new Point(1023, 12);
            dataGridViewSteps.MultiSelect = false;
            dataGridViewSteps.Name = "dataGridViewSteps";
            dataGridViewSteps.ReadOnly = true;
            dataGridViewSteps.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewSteps.RowHeadersVisible = false;
            dataGridViewSteps.ScrollBars = ScrollBars.Vertical;
            dataGridViewSteps.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewSteps.ShowCellToolTips = false;
            dataGridViewSteps.ShowEditingIcon = false;
            dataGridViewSteps.Size = new Size(231, 262);
            dataGridViewSteps.TabIndex = 1;
            dataGridViewSteps.RowEnter += dataGridViewSteps_RowEnter;
            // 
            // buttonAddFlask
            // 
            buttonAddFlask.Location = new Point(12, 168);
            buttonAddFlask.Name = "buttonAddFlask";
            buttonAddFlask.Size = new Size(75, 40);
            buttonAddFlask.TabIndex = 2;
            buttonAddFlask.Text = "Добавить колбу";
            buttonAddFlask.UseVisualStyleBackColor = true;
            buttonAddFlask.Click += buttonAddFlask_Click;
            // 
            // buttonRemoveFlask
            // 
            buttonRemoveFlask.Location = new Point(93, 168);
            buttonRemoveFlask.Name = "buttonRemoveFlask";
            buttonRemoveFlask.Size = new Size(75, 40);
            buttonRemoveFlask.TabIndex = 4;
            buttonRemoveFlask.Text = "Удалить колбу";
            buttonRemoveFlask.UseVisualStyleBackColor = true;
            buttonRemoveFlask.Click += buttonRemoveFlask_Click;
            // 
            // pictureBoxCells
            // 
            pictureBoxCells.Location = new Point(12, 214);
            pictureBoxCells.Name = "pictureBoxCells";
            pictureBoxCells.Size = new Size(1000, 60);
            pictureBoxCells.TabIndex = 5;
            pictureBoxCells.TabStop = false;
            pictureBoxCells.MouseClick += pictureBoxCells_MouseClick;
            // 
            // buttonFindSolution
            // 
            buttonFindSolution.Location = new Point(781, 168);
            buttonFindSolution.Name = "buttonFindSolution";
            buttonFindSolution.Size = new Size(231, 40);
            buttonFindSolution.TabIndex = 7;
            buttonFindSolution.Text = "Найти решение";
            buttonFindSolution.UseVisualStyleBackColor = true;
            buttonFindSolution.Click += buttonFindSolution_Click;
            // 
            // buttonLoadPosition
            // 
            buttonLoadPosition.Location = new Point(294, 168);
            buttonLoadPosition.Name = "buttonLoadPosition";
            buttonLoadPosition.Size = new Size(75, 40);
            buttonLoadPosition.TabIndex = 9;
            buttonLoadPosition.Text = "Загрузить позицию";
            buttonLoadPosition.UseVisualStyleBackColor = true;
            buttonLoadPosition.Click += buttonLoadPosition_Click;
            // 
            // buttonSavePosition
            // 
            buttonSavePosition.Location = new Point(213, 168);
            buttonSavePosition.Name = "buttonSavePosition";
            buttonSavePosition.Size = new Size(75, 40);
            buttonSavePosition.TabIndex = 8;
            buttonSavePosition.Text = "Сохранить позицию";
            buttonSavePosition.UseVisualStyleBackColor = true;
            buttonSavePosition.Click += buttonSavePosition_Click;
            // 
            // saveFileDialog
            // 
            saveFileDialog.Filter = "WaterSolver files | *.pzn";
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "WaterSolver files | *.pzn";
            // 
            // labelInfo1
            // 
            labelInfo1.AutoSize = true;
            labelInfo1.Location = new Point(3, 5);
            labelInfo1.Name = "labelInfo1";
            labelInfo1.Size = new Size(63, 30);
            labelInfo1.TabIndex = 10;
            labelInfo1.Text = "Время:\r\nПрогресс:";
            // 
            // labelInfo2
            // 
            labelInfo2.AutoSize = true;
            labelInfo2.Location = new Point(65, 5);
            labelInfo2.Name = "labelInfo2";
            labelInfo2.Size = new Size(49, 30);
            labelInfo2.TabIndex = 11;
            labelInfo2.Text = "00:00:00\r\n0";
            // 
            // panelInfo
            // 
            panelInfo.Controls.Add(labelInfo6);
            panelInfo.Controls.Add(labelInfo5);
            panelInfo.Controls.Add(labelInfo3);
            panelInfo.Controls.Add(labelInfo4);
            panelInfo.Controls.Add(labelInfo1);
            panelInfo.Controls.Add(labelInfo2);
            panelInfo.Location = new Point(397, 168);
            panelInfo.Name = "panelInfo";
            panelInfo.Size = new Size(378, 40);
            panelInfo.TabIndex = 12;
            panelInfo.Visible = false;
            // 
            // labelInfo6
            // 
            labelInfo6.AutoSize = true;
            labelInfo6.Location = new Point(315, 20);
            labelInfo6.Name = "labelInfo6";
            labelInfo6.Size = new Size(13, 15);
            labelInfo6.TabIndex = 15;
            labelInfo6.Text = "0";
            // 
            // labelInfo5
            // 
            labelInfo5.AutoSize = true;
            labelInfo5.Location = new Point(283, 5);
            labelInfo5.Name = "labelInfo5";
            labelInfo5.Size = new Size(91, 15);
            labelInfo5.TabIndex = 14;
            labelInfo5.Text = "Всего позиций:";
            // 
            // labelInfo3
            // 
            labelInfo3.AutoSize = true;
            labelInfo3.Location = new Point(131, 5);
            labelInfo3.Name = "labelInfo3";
            labelInfo3.Size = new Size(115, 30);
            labelInfo3.TabIndex = 12;
            labelInfo3.Text = "Макс. глубина:\r\nКонечных позиций:";
            // 
            // labelInfo4
            // 
            labelInfo4.AutoSize = true;
            labelInfo4.Location = new Point(252, 6);
            labelInfo4.Name = "labelInfo4";
            labelInfo4.Size = new Size(13, 30);
            labelInfo4.TabIndex = 13;
            labelInfo4.Text = "0\r\n0";
            // 
            // timer
            // 
            timer.Interval = 500;
            timer.Tick += timer_Tick;
            // 
            // Number
            // 
            Number.HeaderText = "Number";
            Number.Name = "Number";
            Number.ReadOnly = true;
            Number.Width = 30;
            // 
            // StepInfo
            // 
            StepInfo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            StepInfo.HeaderText = "Steps";
            StepInfo.Name = "StepInfo";
            StepInfo.ReadOnly = true;
            // 
            // WaterSolverForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 283);
            Controls.Add(panelInfo);
            Controls.Add(buttonLoadPosition);
            Controls.Add(buttonSavePosition);
            Controls.Add(buttonFindSolution);
            Controls.Add(pictureBoxCells);
            Controls.Add(buttonRemoveFlask);
            Controls.Add(buttonAddFlask);
            Controls.Add(dataGridViewSteps);
            Controls.Add(pictureBoxField);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "WaterSolverForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Water Solver";
            Shown += WaterSolverForm_Shown;
            ((System.ComponentModel.ISupportInitialize)pictureBoxField).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewSteps).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCells).EndInit();
            panelInfo.ResumeLayout(false);
            panelInfo.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBoxField;
        private DataGridView dataGridViewSteps;
        private Button buttonAddFlask;
        private Button buttonRemoveFlask;
        private PictureBox pictureBoxCells;
        private Button buttonFindSolution;
        private Button buttonLoadPosition;
        private Button buttonSavePosition;
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;
        private Label labelInfo1;
        private Label labelInfo2;
        private Panel panelInfo;
        private System.Windows.Forms.Timer timer;
        private Label labelInfo3;
        private Label labelInfo4;
        private Label labelInfo5;
        private Label labelInfo6;
        private DataGridViewTextBoxColumn Number;
        private DataGridViewTextBoxColumn StepInfo;
    }
}
