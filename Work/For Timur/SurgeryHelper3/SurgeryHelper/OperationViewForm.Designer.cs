﻿namespace SurgeryHelper
{
    partial class OperationViewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OperationViewForm));
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerDataOfOperation = new System.Windows.Forms.DateTimePicker();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSurgeons = new System.Windows.Forms.TextBox();
            this.textBoxAssistents = new System.Windows.Forms.TextBox();
            this.linkLabelSurgeonList = new System.Windows.Forms.LinkLabel();
            this.linkLabelAssistentList = new System.Windows.Forms.LinkLabel();
            this.linkLabelScrubNurseList = new System.Windows.Forms.LinkLabel();
            this.linkLabelOrderlyList = new System.Windows.Forms.LinkLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.comboBoxScrubNurse = new System.Windows.Forms.ComboBox();
            this.comboBoxOrderly = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePickerEndTimeOfOperation = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerStartTimeOfOperation = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabelHeAnestethist = new System.Windows.Forms.LinkLabel();
            this.linkLabelSheAnestethistList = new System.Windows.Forms.LinkLabel();
            this.comboBoxHeAnestethist = new System.Windows.Forms.ComboBox();
            this.comboBoxSheAnestethist = new System.Windows.Forms.ComboBox();
            this.linkLabelAnesthesiaTypesList = new System.Windows.Forms.LinkLabel();
            this.buttonProtocol = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.numericUpDownRiskLevel = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxAnesthesiaType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRiskLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 59;
            this.label1.Text = "* Название операции";
            // 
            // dateTimePickerDataOfOperation
            // 
            this.dateTimePickerDataOfOperation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dateTimePickerDataOfOperation.CustomFormat = "dd.MM.yyyy";
            this.dateTimePickerDataOfOperation.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerDataOfOperation.Location = new System.Drawing.Point(168, 72);
            this.dateTimePickerDataOfOperation.Name = "dateTimePickerDataOfOperation";
            this.dateTimePickerDataOfOperation.Size = new System.Drawing.Size(124, 20);
            this.dateTimePickerDataOfOperation.TabIndex = 3;
            this.dateTimePickerDataOfOperation.ValueChanged += new System.EventHandler(this.dateTimePickerDataOfOperation_ValueChanged);
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxName.Location = new System.Drawing.Point(168, 15);
            this.textBoxName.Multiline = true;
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxName.Size = new System.Drawing.Size(225, 51);
            this.textBoxName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 62;
            this.label2.Text = "* Дата операции";
            // 
            // textBoxSurgeons
            // 
            this.textBoxSurgeons.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxSurgeons.Location = new System.Drawing.Point(168, 203);
            this.textBoxSurgeons.MaxLength = 200000;
            this.textBoxSurgeons.Multiline = true;
            this.textBoxSurgeons.Name = "textBoxSurgeons";
            this.textBoxSurgeons.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSurgeons.Size = new System.Drawing.Size(225, 47);
            this.textBoxSurgeons.TabIndex = 11;
            this.textBoxSurgeons.TextChanged += new System.EventHandler(this.textBoxSurgeons_TextChanged);
            // 
            // textBoxAssistents
            // 
            this.textBoxAssistents.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxAssistents.Location = new System.Drawing.Point(168, 256);
            this.textBoxAssistents.MaxLength = 200000;
            this.textBoxAssistents.Multiline = true;
            this.textBoxAssistents.Name = "textBoxAssistents";
            this.textBoxAssistents.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxAssistents.Size = new System.Drawing.Size(225, 47);
            this.textBoxAssistents.TabIndex = 15;
            this.textBoxAssistents.TextChanged += new System.EventHandler(this.textBoxAssistents_TextChanged);
            // 
            // linkLabelSurgeonList
            // 
            this.linkLabelSurgeonList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelSurgeonList.AutoSize = true;
            this.linkLabelSurgeonList.Location = new System.Drawing.Point(12, 221);
            this.linkLabelSurgeonList.Name = "linkLabelSurgeonList";
            this.linkLabelSurgeonList.Size = new System.Drawing.Size(99, 13);
            this.linkLabelSurgeonList.TabIndex = 9;
            this.linkLabelSurgeonList.TabStop = true;
            this.linkLabelSurgeonList.Text = "* Список хирургов";
            this.linkLabelSurgeonList.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSurgeonList_LinkClicked);
            this.linkLabelSurgeonList.MouseEnter += new System.EventHandler(this.linkLabelSurgeonList_MouseEnter);
            this.linkLabelSurgeonList.MouseLeave += new System.EventHandler(this.linkLabelSurgeonList_MouseLeave);
            // 
            // linkLabelAssistentList
            // 
            this.linkLabelAssistentList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelAssistentList.AutoSize = true;
            this.linkLabelAssistentList.Location = new System.Drawing.Point(12, 272);
            this.linkLabelAssistentList.Name = "linkLabelAssistentList";
            this.linkLabelAssistentList.Size = new System.Drawing.Size(111, 13);
            this.linkLabelAssistentList.TabIndex = 13;
            this.linkLabelAssistentList.TabStop = true;
            this.linkLabelAssistentList.Text = "Список ассистентов";
            this.linkLabelAssistentList.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAssistentList_LinkClicked);
            this.linkLabelAssistentList.MouseEnter += new System.EventHandler(this.linkLabelAssistentList_MouseEnter);
            this.linkLabelAssistentList.MouseLeave += new System.EventHandler(this.linkLabelAssistentList_MouseLeave);
            // 
            // linkLabelScrubNurseList
            // 
            this.linkLabelScrubNurseList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelScrubNurseList.AutoSize = true;
            this.linkLabelScrubNurseList.Location = new System.Drawing.Point(12, 365);
            this.linkLabelScrubNurseList.Name = "linkLabelScrubNurseList";
            this.linkLabelScrubNurseList.Size = new System.Drawing.Size(119, 13);
            this.linkLabelScrubNurseList.TabIndex = 21;
            this.linkLabelScrubNurseList.TabStop = true;
            this.linkLabelScrubNurseList.Text = "* Операц. мед. сестра";
            this.linkLabelScrubNurseList.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelScrubNurseList_LinkClicked);
            this.linkLabelScrubNurseList.MouseEnter += new System.EventHandler(this.linkLabelScrubNurseList_MouseEnter);
            this.linkLabelScrubNurseList.MouseLeave += new System.EventHandler(this.linkLabelScrubNurseList_MouseLeave);
            // 
            // linkLabelOrderlyList
            // 
            this.linkLabelOrderlyList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelOrderlyList.AutoSize = true;
            this.linkLabelOrderlyList.Location = new System.Drawing.Point(12, 392);
            this.linkLabelOrderlyList.Name = "linkLabelOrderlyList";
            this.linkLabelOrderlyList.Size = new System.Drawing.Size(49, 13);
            this.linkLabelOrderlyList.TabIndex = 25;
            this.linkLabelOrderlyList.TabStop = true;
            this.linkLabelOrderlyList.Text = "Санитар";
            this.linkLabelOrderlyList.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelOrderlyList_LinkClicked);
            this.linkLabelOrderlyList.MouseEnter += new System.EventHandler(this.linkLabelOrderlyList_MouseEnter);
            this.linkLabelOrderlyList.MouseLeave += new System.EventHandler(this.linkLabelOrderlyList_MouseLeave);
            // 
            // comboBoxScrubNurse
            // 
            this.comboBoxScrubNurse.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxScrubNurse.FormattingEnabled = true;
            this.comboBoxScrubNurse.Location = new System.Drawing.Point(168, 362);
            this.comboBoxScrubNurse.Name = "comboBoxScrubNurse";
            this.comboBoxScrubNurse.Size = new System.Drawing.Size(225, 21);
            this.comboBoxScrubNurse.TabIndex = 23;
            this.comboBoxScrubNurse.TextChanged += new System.EventHandler(this.comboBoxScrubNurse_TextChanged);
            // 
            // comboBoxOrderly
            // 
            this.comboBoxOrderly.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxOrderly.FormattingEnabled = true;
            this.comboBoxOrderly.Location = new System.Drawing.Point(168, 389);
            this.comboBoxOrderly.Name = "comboBoxOrderly";
            this.comboBoxOrderly.Size = new System.Drawing.Size(225, 21);
            this.comboBoxOrderly.TabIndex = 27;
            this.comboBoxOrderly.TextChanged += new System.EventHandler(this.comboBoxOrderly_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 13);
            this.label3.TabIndex = 70;
            this.label3.Text = "Время окончания операции";
            // 
            // dateTimePickerEndTimeOfOperation
            // 
            this.dateTimePickerEndTimeOfOperation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dateTimePickerEndTimeOfOperation.CustomFormat = "HH:mm";
            this.dateTimePickerEndTimeOfOperation.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerEndTimeOfOperation.Location = new System.Drawing.Point(168, 124);
            this.dateTimePickerEndTimeOfOperation.Name = "dateTimePickerEndTimeOfOperation";
            this.dateTimePickerEndTimeOfOperation.ShowUpDown = true;
            this.dateTimePickerEndTimeOfOperation.Size = new System.Drawing.Size(124, 20);
            this.dateTimePickerEndTimeOfOperation.TabIndex = 7;
            // 
            // dateTimePickerStartTimeOfOperation
            // 
            this.dateTimePickerStartTimeOfOperation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dateTimePickerStartTimeOfOperation.CustomFormat = "HH:mm";
            this.dateTimePickerStartTimeOfOperation.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStartTimeOfOperation.Location = new System.Drawing.Point(168, 98);
            this.dateTimePickerStartTimeOfOperation.Name = "dateTimePickerStartTimeOfOperation";
            this.dateTimePickerStartTimeOfOperation.ShowUpDown = true;
            this.dateTimePickerStartTimeOfOperation.Size = new System.Drawing.Size(124, 20);
            this.dateTimePickerStartTimeOfOperation.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 13);
            this.label4.TabIndex = 73;
            this.label4.Text = "* Время начала операции";
            // 
            // linkLabelHeAnestethist
            // 
            this.linkLabelHeAnestethist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelHeAnestethist.AutoSize = true;
            this.linkLabelHeAnestethist.Location = new System.Drawing.Point(12, 312);
            this.linkLabelHeAnestethist.Name = "linkLabelHeAnestethist";
            this.linkLabelHeAnestethist.Size = new System.Drawing.Size(78, 13);
            this.linkLabelHeAnestethist.TabIndex = 74;
            this.linkLabelHeAnestethist.TabStop = true;
            this.linkLabelHeAnestethist.Text = "Анестезиолог";
            this.linkLabelHeAnestethist.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelHeAnestethist_LinkClicked);
            this.linkLabelHeAnestethist.MouseEnter += new System.EventHandler(this.linkLabelHeAnestethist_MouseEnter);
            this.linkLabelHeAnestethist.MouseLeave += new System.EventHandler(this.linkLabelHeAnestethist_MouseLeave);
            // 
            // linkLabelSheAnestethistList
            // 
            this.linkLabelSheAnestethistList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelSheAnestethistList.AutoSize = true;
            this.linkLabelSheAnestethistList.Location = new System.Drawing.Point(12, 338);
            this.linkLabelSheAnestethistList.Name = "linkLabelSheAnestethistList";
            this.linkLabelSheAnestethistList.Size = new System.Drawing.Size(78, 13);
            this.linkLabelSheAnestethistList.TabIndex = 75;
            this.linkLabelSheAnestethistList.TabStop = true;
            this.linkLabelSheAnestethistList.Text = "Анестезистка";
            this.linkLabelSheAnestethistList.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSheAnestethistList_LinkClicked);
            this.linkLabelSheAnestethistList.MouseEnter += new System.EventHandler(this.linkLabelSheAnestethistList_MouseEnter);
            this.linkLabelSheAnestethistList.MouseLeave += new System.EventHandler(this.linkLabelSheAnestethistList_MouseLeave);
            // 
            // comboBoxHeAnestethist
            // 
            this.comboBoxHeAnestethist.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxHeAnestethist.FormattingEnabled = true;
            this.comboBoxHeAnestethist.Location = new System.Drawing.Point(168, 309);
            this.comboBoxHeAnestethist.Name = "comboBoxHeAnestethist";
            this.comboBoxHeAnestethist.Size = new System.Drawing.Size(225, 21);
            this.comboBoxHeAnestethist.TabIndex = 17;
            this.comboBoxHeAnestethist.TextChanged += new System.EventHandler(this.comboBoxHeAnestethist_TextChanged);
            // 
            // comboBoxSheAnestethist
            // 
            this.comboBoxSheAnestethist.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxSheAnestethist.FormattingEnabled = true;
            this.comboBoxSheAnestethist.Location = new System.Drawing.Point(168, 335);
            this.comboBoxSheAnestethist.Name = "comboBoxSheAnestethist";
            this.comboBoxSheAnestethist.Size = new System.Drawing.Size(225, 21);
            this.comboBoxSheAnestethist.TabIndex = 19;
            this.comboBoxSheAnestethist.TextChanged += new System.EventHandler(this.comboBoxSheAnestethist_TextChanged);
            // 
            // linkLabelAnesthesiaTypesList
            // 
            this.linkLabelAnesthesiaTypesList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelAnesthesiaTypesList.AutoSize = true;
            this.linkLabelAnesthesiaTypesList.Location = new System.Drawing.Point(12, 153);
            this.linkLabelAnesthesiaTypesList.Name = "linkLabelAnesthesiaTypesList";
            this.linkLabelAnesthesiaTypesList.Size = new System.Drawing.Size(89, 13);
            this.linkLabelAnesthesiaTypesList.TabIndex = 79;
            this.linkLabelAnesthesiaTypesList.TabStop = true;
            this.linkLabelAnesthesiaTypesList.Text = "* Вид анестезии";
            this.linkLabelAnesthesiaTypesList.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAnesthesiaTypesList_LinkClicked);
            // 
            // buttonProtocol
            // 
            this.buttonProtocol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonProtocol.BackgroundImage = global::SurgeryHelper.Properties.Resources.DOCUMENT;
            this.buttonProtocol.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonProtocol.FlatAppearance.BorderSize = 0;
            this.buttonProtocol.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonProtocol.Location = new System.Drawing.Point(12, 423);
            this.buttonProtocol.Name = "buttonProtocol";
            this.buttonProtocol.Size = new System.Drawing.Size(40, 40);
            this.buttonProtocol.TabIndex = 50;
            this.buttonProtocol.TabStop = false;
            this.buttonProtocol.UseVisualStyleBackColor = true;
            this.buttonProtocol.Click += new System.EventHandler(this.buttonProtocol_Click);
            this.buttonProtocol.MouseEnter += new System.EventHandler(this.buttonProtocol_MouseEnter);
            this.buttonProtocol.MouseLeave += new System.EventHandler(this.buttonProtocol_MouseLeave);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOk.BackgroundImage = global::SurgeryHelper.Properties.Resources.OK;
            this.buttonOk.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonOk.FlatAppearance.BorderSize = 0;
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOk.Location = new System.Drawing.Point(276, 423);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(40, 40);
            this.buttonOk.TabIndex = 57;
            this.buttonOk.TabStop = false;
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            this.buttonOk.MouseEnter += new System.EventHandler(this.buttonOk_MouseEnter);
            this.buttonOk.MouseLeave += new System.EventHandler(this.buttonOk_MouseLeave);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonClose.BackgroundImage = global::SurgeryHelper.Properties.Resources.close;
            this.buttonClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonClose.FlatAppearance.BorderSize = 0;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.Location = new System.Drawing.Point(353, 423);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(40, 40);
            this.buttonClose.TabIndex = 58;
            this.buttonClose.TabStop = false;
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            this.buttonClose.MouseEnter += new System.EventHandler(this.buttonClose_MouseEnter);
            this.buttonClose.MouseLeave += new System.EventHandler(this.buttonClose_MouseLeave);
            // 
            // numericUpDownRiskLevel
            // 
            this.numericUpDownRiskLevel.Location = new System.Drawing.Point(168, 177);
            this.numericUpDownRiskLevel.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDownRiskLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRiskLevel.Name = "numericUpDownRiskLevel";
            this.numericUpDownRiskLevel.Size = new System.Drawing.Size(225, 20);
            this.numericUpDownRiskLevel.TabIndex = 10;
            this.numericUpDownRiskLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 179);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 80;
            this.label5.Text = "Риск операции";
            // 
            // comboBoxAnesthesiaType
            // 
            this.comboBoxAnesthesiaType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxAnesthesiaType.FormattingEnabled = true;
            this.comboBoxAnesthesiaType.Location = new System.Drawing.Point(168, 150);
            this.comboBoxAnesthesiaType.Name = "comboBoxAnesthesiaType";
            this.comboBoxAnesthesiaType.Size = new System.Drawing.Size(225, 21);
            this.comboBoxAnesthesiaType.TabIndex = 81;
            this.comboBoxAnesthesiaType.TextChanged += new System.EventHandler(this.comboBoxAnesthesiaType_TextChanged);
            // 
            // OperationViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 475);
            this.Controls.Add(this.comboBoxAnesthesiaType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDownRiskLevel);
            this.Controls.Add(this.linkLabelAnesthesiaTypesList);
            this.Controls.Add(this.comboBoxSheAnestethist);
            this.Controls.Add(this.comboBoxHeAnestethist);
            this.Controls.Add(this.linkLabelSheAnestethistList);
            this.Controls.Add(this.linkLabelHeAnestethist);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dateTimePickerStartTimeOfOperation);
            this.Controls.Add(this.dateTimePickerEndTimeOfOperation);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxOrderly);
            this.Controls.Add(this.comboBoxScrubNurse);
            this.Controls.Add(this.buttonProtocol);
            this.Controls.Add(this.linkLabelOrderlyList);
            this.Controls.Add(this.linkLabelScrubNurseList);
            this.Controls.Add(this.linkLabelAssistentList);
            this.Controls.Add(this.linkLabelSurgeonList);
            this.Controls.Add(this.textBoxAssistents);
            this.Controls.Add(this.textBoxSurgeons);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.dateTimePickerDataOfOperation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OperationViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "OperationViewForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OperationViewForm_FormClosing);
            this.Load += new System.EventHandler(this.OperationViewForm_Load);
            this.LocationChanged += new System.EventHandler(this.OperationViewForm_LocationChanged);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRiskLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerDataOfOperation;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSurgeons;
        private System.Windows.Forms.TextBox textBoxAssistents;
        private System.Windows.Forms.LinkLabel linkLabelSurgeonList;
        private System.Windows.Forms.LinkLabel linkLabelAssistentList;
        private System.Windows.Forms.LinkLabel linkLabelScrubNurseList;
        private System.Windows.Forms.LinkLabel linkLabelOrderlyList;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonProtocol;
        private System.Windows.Forms.ComboBox comboBoxScrubNurse;
        private System.Windows.Forms.ComboBox comboBoxOrderly;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateTimePickerEndTimeOfOperation;
        private System.Windows.Forms.DateTimePicker dateTimePickerStartTimeOfOperation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabelHeAnestethist;
        private System.Windows.Forms.LinkLabel linkLabelSheAnestethistList;
        private System.Windows.Forms.ComboBox comboBoxHeAnestethist;
        private System.Windows.Forms.ComboBox comboBoxSheAnestethist;
        private System.Windows.Forms.LinkLabel linkLabelAnesthesiaTypesList;
        private System.Windows.Forms.NumericUpDown numericUpDownRiskLevel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxAnesthesiaType;
    }
}