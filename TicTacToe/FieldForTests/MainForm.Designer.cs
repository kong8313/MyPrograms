namespace FieldForTests
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
            this.panelField = new System.Windows.Forms.Panel();
            this.buttonSetX = new System.Windows.Forms.Button();
            this.buttonSetO = new System.Windows.Forms.Button();
            this.buttonClearCell = new System.Windows.Forms.Button();
            this.buttonClearField = new System.Windows.Forms.Button();
            this.richTextBoxTextField = new System.Windows.Forms.RichTextBox();
            this.buttonPictureToText = new System.Windows.Forms.Button();
            this.buttonTextToPicture = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.buttonDoStep = new System.Windows.Forms.Button();
            this.labelStep = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // panelField
            // 
            this.panelField.Location = new System.Drawing.Point(20, 17);
            this.panelField.Name = "panelField";
            this.panelField.Size = new System.Drawing.Size(601, 601);
            this.panelField.TabIndex = 6;
            this.panelField.TabStop = true;
            this.panelField.Paint += new System.Windows.Forms.PaintEventHandler(this.panelField_Paint);
            this.panelField.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelField_MouseDown);
            // 
            // buttonSetX
            // 
            this.buttonSetX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSetX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSetX.Location = new System.Drawing.Point(627, 12);
            this.buttonSetX.Name = "buttonSetX";
            this.buttonSetX.Size = new System.Drawing.Size(35, 35);
            this.buttonSetX.TabIndex = 7;
            this.buttonSetX.TabStop = false;
            this.buttonSetX.Text = "X";
            this.toolTip.SetToolTip(this.buttonSetX, "Left mouse button");
            this.buttonSetX.UseVisualStyleBackColor = true;
            this.buttonSetX.Click += new System.EventHandler(this.buttonSetX_Click);
            // 
            // buttonSetO
            // 
            this.buttonSetO.Location = new System.Drawing.Point(668, 12);
            this.buttonSetO.Name = "buttonSetO";
            this.buttonSetO.Size = new System.Drawing.Size(35, 35);
            this.buttonSetO.TabIndex = 8;
            this.buttonSetO.TabStop = false;
            this.buttonSetO.Text = "O";
            this.toolTip.SetToolTip(this.buttonSetO, "Right mouse button");
            this.buttonSetO.UseVisualStyleBackColor = true;
            this.buttonSetO.Click += new System.EventHandler(this.buttonSetO_Click);
            // 
            // buttonClearCell
            // 
            this.buttonClearCell.Location = new System.Drawing.Point(648, 53);
            this.buttonClearCell.Name = "buttonClearCell";
            this.buttonClearCell.Size = new System.Drawing.Size(35, 35);
            this.buttonClearCell.TabIndex = 9;
            this.buttonClearCell.TabStop = false;
            this.toolTip.SetToolTip(this.buttonClearCell, "Middle mouse button");
            this.buttonClearCell.UseVisualStyleBackColor = true;
            this.buttonClearCell.Click += new System.EventHandler(this.buttonClearCell_Click);
            // 
            // buttonClearField
            // 
            this.buttonClearField.Location = new System.Drawing.Point(627, 99);
            this.buttonClearField.Name = "buttonClearField";
            this.buttonClearField.Size = new System.Drawing.Size(76, 35);
            this.buttonClearField.TabIndex = 10;
            this.buttonClearField.TabStop = false;
            this.buttonClearField.Text = "Clear all";
            this.buttonClearField.UseVisualStyleBackColor = true;
            this.buttonClearField.Click += new System.EventHandler(this.buttonClearField_Click);
            // 
            // richTextBoxTextField
            // 
            this.richTextBoxTextField.Location = new System.Drawing.Point(709, 17);
            this.richTextBoxTextField.Name = "richTextBoxTextField";
            this.richTextBoxTextField.Size = new System.Drawing.Size(256, 601);
            this.richTextBoxTextField.TabIndex = 1;
            this.richTextBoxTextField.Text = "";
            // 
            // buttonPictureToText
            // 
            this.buttonPictureToText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPictureToText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPictureToText.Location = new System.Drawing.Point(668, 312);
            this.buttonPictureToText.Name = "buttonPictureToText";
            this.buttonPictureToText.Size = new System.Drawing.Size(35, 35);
            this.buttonPictureToText.TabIndex = 11;
            this.buttonPictureToText.TabStop = false;
            this.buttonPictureToText.Text = "-->";
            this.buttonPictureToText.UseVisualStyleBackColor = true;
            this.buttonPictureToText.Click += new System.EventHandler(this.buttonPictureToText_Click);
            // 
            // buttonTextToPicture
            // 
            this.buttonTextToPicture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTextToPicture.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTextToPicture.Location = new System.Drawing.Point(627, 312);
            this.buttonTextToPicture.Name = "buttonTextToPicture";
            this.buttonTextToPicture.Size = new System.Drawing.Size(35, 35);
            this.buttonTextToPicture.TabIndex = 12;
            this.buttonTextToPicture.TabStop = false;
            this.buttonTextToPicture.Text = "<--";
            this.buttonTextToPicture.UseVisualStyleBackColor = true;
            this.buttonTextToPicture.Click += new System.EventHandler(this.buttonTextToPicture_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(29, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(586, 15);
            this.label1.TabIndex = 13;
            this.label1.Text = "0        1        2        3        4       5       6       7        8        9  " +
    "     10     11     12     13     14      15     16      17     18     19";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(-2, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 585);
            this.label2.TabIndex = 14;
            this.label2.Text = " 0\r\n\r\n 1\r\n\r\n 2\r\n\r\n 3\r\n\r\n 4\r\n\r\n 5\r\n\r\n 6\r\n\r\n 7\r\n\r\n 8\r\n\r\n 9\r\n\r\n10\r\n\r\n11\r\n\r\n12\r\n\r\n13\r" +
    "\n\r\n14\r\n\r\n15\r\n\r\n16\r\n\r\n17\r\n\r\n18\r\n\r\n19";
            // 
            // buttonDoStep
            // 
            this.buttonDoStep.Location = new System.Drawing.Point(627, 379);
            this.buttonDoStep.Name = "buttonDoStep";
            this.buttonDoStep.Size = new System.Drawing.Size(76, 35);
            this.buttonDoStep.TabIndex = 15;
            this.buttonDoStep.TabStop = false;
            this.buttonDoStep.Text = "Do step";
            this.buttonDoStep.UseVisualStyleBackColor = true;
            this.buttonDoStep.Click += new System.EventHandler(this.buttonDoStep_Click);
            // 
            // labelStep
            // 
            this.labelStep.Location = new System.Drawing.Point(627, 417);
            this.labelStep.Name = "labelStep";
            this.labelStep.Size = new System.Drawing.Size(76, 15);
            this.labelStep.TabIndex = 16;
            this.labelStep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(970, 630);
            this.Controls.Add(this.labelStep);
            this.Controls.Add(this.buttonDoStep);
            this.Controls.Add(this.buttonTextToPicture);
            this.Controls.Add(this.buttonPictureToText);
            this.Controls.Add(this.richTextBoxTextField);
            this.Controls.Add(this.buttonClearField);
            this.Controls.Add(this.buttonClearCell);
            this.Controls.Add(this.buttonSetO);
            this.Controls.Add(this.buttonSetX);
            this.Controls.Add(this.panelField);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelField;
        private System.Windows.Forms.Button buttonSetX;
        private System.Windows.Forms.Button buttonSetO;
        private System.Windows.Forms.Button buttonClearCell;
        private System.Windows.Forms.Button buttonClearField;
        private System.Windows.Forms.RichTextBox richTextBoxTextField;
        private System.Windows.Forms.Button buttonPictureToText;
        private System.Windows.Forms.Button buttonTextToPicture;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button buttonDoStep;
        private System.Windows.Forms.Label labelStep;
    }
}

