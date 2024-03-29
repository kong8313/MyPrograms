﻿using System;
using System.Windows.Forms;
using SurgeryHelper.Engines;
using SurgeryHelper.Entities;

namespace SurgeryHelper
{
    public partial class MedicalInspectionForm : Form
    {
        public string MkbCodeFromMkbSelectForm { set; private get; }

        private readonly PatientClass _patientInfo;
        private bool _isFormClosingByButton;
        private bool _stopSaveParameters;
        private readonly DbEngine _dbEngine;

        public MedicalInspectionForm(PatientClass patientInfo, DbEngine dbEngine)
        {
            _stopSaveParameters = true;

            InitializeComponent();

            _dbEngine = dbEngine;
            _patientInfo = patientInfo;
        }
        
        private void MedicalInspectionForm_Load(object sender, EventArgs e)
        {
            if (_dbEngine.ConfigEngine.MedicalInspectionFormLocation.X >= 0 &&
               _dbEngine.ConfigEngine.MedicalInspectionFormLocation.Y >= 0)
            {
                Location = _dbEngine.ConfigEngine.MedicalInspectionFormLocation;
            }

            comboBoxWWW.Items.Clear();
            comboBoxWWW.Items.AddRange(_dbEngine.ConfigEngine.PatientViewFormLastWWW);

            comboBoxWWW.Text = _patientInfo.WWW;

            checkBoxIsPlanEnabled.Checked = _patientInfo.MedicalInspectionIsPlanEnabled;
            checkBoxMedicalInspectionWithBoss.Checked = _patientInfo.MedicalInspectionWithBoss;
            comboBoxInspectionPlan.Text = _patientInfo.MedicalInspectionInspectionPlan;
            comboBoxTreatmentType.Text = _patientInfo.MedicalInspectionTreatmentType;
            textBoxComplaints.Text = _patientInfo.MedicalInspectionComplaints;
            comboBoxTeoRisk.Text = _patientInfo.MedicalInspectionTeoRisk;
            checkBoxTeoRisk.Checked = _patientInfo.MedicalInspectionTeoRiskEnabled;

            if (_patientInfo.MedicalInspectionExpertAnamnese == 1)
            { 
                radioButtonLnWithNumber.Checked = true;
            }
            else if (_patientInfo.MedicalInspectionExpertAnamnese == 2)
            {
                radioButtonNewLn.Checked = true;
            }
            else 
            {
                radioButtonNoLn.Checked = true;
            }

            dateTimePickerLnWithNumberStart.Value = _patientInfo.MedicalInspectionLnWithNumberDateStart;
            dateTimePickerLnWithNumberEnd.Value = _patientInfo.MedicalInspectionLnWithNumberDateEnd;
            dateTimePickerLnFirstStart.Value = _patientInfo.MedicalInspectionLnFirstDateStart;
            textBoxStLocalisDescription.Text = _patientInfo.MedicalInspectionStLocalisDescription;
            comboBoxRentgen.Text = _patientInfo.MedicalInspectionStLocalisRentgen;

            checkBoxIsAnamnezEnabled.Checked = _patientInfo.MedicalInspectionIsAnamneseActive;

            textBoxAnMorbi.Text = _patientInfo.MedicalInspectionAnamneseAnMorbi;
            SetCheckBoxes(groupBoxAnVitae.Controls, _patientInfo.MedicalInspectionAnamneseAnVitae, 13);
            SetTextBoxes(tabPageAnamnes.Controls, _patientInfo.MedicalInspectionAnamneseTextBoxes, 1);
            SetCheckBoxes(groupBoxRiskTeo.Controls, _patientInfo.MedicalInspectionAnamneseCheckboxes, 1);

            SetComboBoxes(tabPageStPraesens.Controls, _patientInfo.MedicalInspectionStPraesensComboBoxes, 1);
            SetTextBoxes(tabPageStPraesens.Controls, _patientInfo.MedicalInspectionStPraesensTextBoxes, 209);
            SetNumericUpDowns(tabPageStPraesens.Controls, _patientInfo.MedicalInspectionStPraesensNumericUpDowns, 1);
            textBoxStPraesensOther.Text = _patientInfo.MedicalInspectionStPraesensOthers;
            textBoxStPraesensTemperature.Text = _patientInfo.MedicalInspectionStPraesensTemperature;

            checkBoxIsUpperExtremityJoint.Checked = _patientInfo.MedicalInspectionIsStLocalisPart1Enabled;
            comboBoxOppositionFinger.Text = _patientInfo.MedicalInspectionStLocalisPart1OppositionFinger;
            SetTextBoxes(tabPageStLocalis1.Controls, _patientInfo.MedicalInspectionStLocalisPart1Fields, 126);

            _stopSaveParameters = false;
        }

        private static void SetComboBoxes(Control.ControlCollection controls, string[] values, int startNumber)
        {
            for (int i = startNumber; i < startNumber + values.Length; i++)
            {
                controls["comboBox" + i].Text = values[i - startNumber];
            }
        }

        private static void SetNumericUpDowns(Control.ControlCollection controls, int[] values, int startNumber)
        {
            for (int i = startNumber; i < startNumber + values.Length; i++)
            {
                ((NumericUpDown)controls["numericUpDown" + i]).Value = values[i - startNumber];
            }
        }

        private static void SetCheckBoxes(Control.ControlCollection controls, bool[] values, int startNumber)
        {
            for (int i = startNumber; i < startNumber + values.Length; i++)
            {
                ((CheckBox)controls["checkBox" + i]).Checked = values[i - startNumber];
            }
        }

        private static void SetTextBoxes(Control.ControlCollection controls, string[] values, int startNumber)
        {
            for (int i = startNumber; i < startNumber + values.Length; i++)
            {
                controls["textBox" + i].Text = values[i - startNumber];
            }
        }

        private static string[] GetComboBoxes(Control.ControlCollection controls, int startNumber, int length)
        {
            var values = new string[length];
            for (int i = startNumber; i < startNumber + length; i++)
            {
                values[i - startNumber] = controls["comboBox" + i].Text;
            }

            return values;
        }

        private static int[] GetNumericUpDowns(Control.ControlCollection controls, int startNumber, int length)
        {
            var values = new int[length];
            for (int i = startNumber; i < startNumber + length; i++)
            {
                values[i - startNumber] = (int)((NumericUpDown)controls["numericUpDown" + i]).Value;
            }

            return values;
        }

        private static bool[] GetCheckBoxes(Control.ControlCollection controls, int startNumber, int length)
        {
            var values = new bool[length];
            for (int i = startNumber; i < startNumber + length; i++)
            {
                values[i - startNumber] = ((CheckBox)controls["checkBox" + i]).Checked;
            }

            return values;
        }

        private static string[] GetTextBoxes(Control.ControlCollection controls, int startNumber, int length)
        {
            var values = new string[length];
            for (int i = startNumber; i < startNumber + length; i++)
            {
                values[i - startNumber] = controls["textBox" + i].Text;
            }

            return values;
        }       

        private void buttonDocuments_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Сгенерировать отчёт в Word", buttonDocuments, 15, -20);
            buttonDocuments.FlatStyle = FlatStyle.Popup;
        }

        private void buttonDocuments_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonDocuments);
            buttonDocuments.FlatStyle = FlatStyle.Flat;
        }

        private void buttonOk_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Сохранить изменения", buttonOk, 15, -20);
            buttonOk.FlatStyle = FlatStyle.Popup;
        }

        private void buttonOk_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonOk);
            buttonOk.FlatStyle = FlatStyle.Flat;
        }

        private void buttonClose_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Закрыть форму без сохранения изменений", buttonClose, 15, -20);
            buttonClose.FlatStyle = FlatStyle.Popup;
        }

        private void buttonClose_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonClose);
            buttonClose.FlatStyle = FlatStyle.Flat;
        }

        /// <summary>
        /// Сгенерировать отчёт в Worde
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDocuments_Click(object sender, EventArgs e)
        {
            var tempPatientInfo = new PatientClass(_patientInfo);

            PutDataToPatient(tempPatientInfo);

            new WordExportEngine(_dbEngine).ExportMedicalInspection(tempPatientInfo);
        }

        /// <summary>
        /// Положить введённые данные в пациента
        /// </summary>
        /// <param name="patientInfo"></param>
        private void PutDataToPatient(PatientClass patientInfo)
        {
            patientInfo.WWW = comboBoxWWW.Text;
            patientInfo.MedicalInspectionIsPlanEnabled = checkBoxIsPlanEnabled.Checked;
            patientInfo.MedicalInspectionWithBoss = checkBoxMedicalInspectionWithBoss.Checked;
            patientInfo.MedicalInspectionInspectionPlan = comboBoxInspectionPlan.Text;
            patientInfo.MedicalInspectionTreatmentType = comboBoxTreatmentType.Text;
            patientInfo.MedicalInspectionComplaints = textBoxComplaints.Text;
            patientInfo.MedicalInspectionTeoRisk = comboBoxTeoRisk.Text;
            patientInfo.MedicalInspectionTeoRiskEnabled = checkBoxTeoRisk.Checked;

            if (radioButtonLnWithNumber.Checked)
            {
                patientInfo.MedicalInspectionExpertAnamnese = 1;
            }
            else if (radioButtonNewLn.Checked)
            {
                patientInfo.MedicalInspectionExpertAnamnese = 2;
            }
            else
            {
                patientInfo.MedicalInspectionExpertAnamnese = 3;
            }

            patientInfo.MedicalInspectionLnWithNumberDateStart = dateTimePickerLnWithNumberStart.Value;
            patientInfo.MedicalInspectionLnWithNumberDateEnd = dateTimePickerLnWithNumberEnd.Value;
            patientInfo.MedicalInspectionLnFirstDateStart = dateTimePickerLnFirstStart.Value;
            patientInfo.MedicalInspectionStLocalisDescription = textBoxStLocalisDescription.Text;
            patientInfo.MedicalInspectionStLocalisRentgen = comboBoxRentgen.Text;

            patientInfo.MedicalInspectionIsAnamneseActive = checkBoxIsAnamnezEnabled.Checked;

            patientInfo.MedicalInspectionAnamneseAnMorbi = textBoxAnMorbi.Text;
            patientInfo.MedicalInspectionAnamneseAnVitae = GetCheckBoxes(groupBoxAnVitae.Controls, 13, 4);
            patientInfo.MedicalInspectionAnamneseTextBoxes = GetTextBoxes(tabPageAnamnes.Controls, 1, 10);
            patientInfo.MedicalInspectionAnamneseCheckboxes = GetCheckBoxes(groupBoxRiskTeo.Controls, 1, 12);

            patientInfo.MedicalInspectionStPraesensComboBoxes = GetComboBoxes(tabPageStPraesens.Controls, 1, 5);
            patientInfo.MedicalInspectionStPraesensTextBoxes = GetTextBoxes(tabPageStPraesens.Controls, 209, 17);
            patientInfo.MedicalInspectionStPraesensNumericUpDowns = GetNumericUpDowns(tabPageStPraesens.Controls, 1, 7);
            patientInfo.MedicalInspectionStPraesensOthers = textBoxStPraesensOther.Text;
            patientInfo.MedicalInspectionStPraesensTemperature = textBoxStPraesensTemperature.Text;

            patientInfo.MedicalInspectionIsStLocalisPart1Enabled = checkBoxIsUpperExtremityJoint.Checked;
            patientInfo.MedicalInspectionStLocalisPart1OppositionFinger = comboBoxOppositionFinger.Text;
            patientInfo.MedicalInspectionStLocalisPart1Fields = GetTextBoxes(tabPageStLocalis1.Controls, 126, 62);

            _dbEngine.ConfigEngine.PatientViewFormLastWWW = ConvertEngine.GetLastUsedValues(comboBoxWWW);
        }

        /// <summary>
        /// Сохранить информацию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            try
            {
                PutDataToPatient(_patientInfo);

                _isFormClosingByButton = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Закрыть форму без сохранения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            _isFormClosingByButton = true;
            Close();
        }

        private void MedicalInspectionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isFormClosingByButton)
            {
                e.Cancel = true;
            }
        }

        private void checkBoxIsAnamnezEnabled_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var component in tabPageAnamnes.Controls)
            {
                if (component is TextBox)
                {
                    ((TextBox)component).Enabled = checkBoxIsAnamnezEnabled.Checked;
                }
                else if (component is CheckBox)
                {
                    if (((CheckBox)component).Name != "checkBoxIsAnamnezEnabled")
                    {
                        ((CheckBox)component).Enabled = checkBoxIsAnamnezEnabled.Checked;
                    }
                }
            }

            foreach (var component in groupBoxAnVitae.Controls)
            {
                if (component is CheckBox)
                {
                    ((CheckBox)component).Enabled = checkBoxIsAnamnezEnabled.Checked;
                }
            }
        }

        private void checkBoxIsPlanEnabled_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxInspectionPlan.Enabled = checkBoxIsPlanEnabled.Checked;
        }

        private void radioButtonNewLn_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerLnFirstStart.Enabled = radioButtonNewLn.Checked;
        }

        private void radioButtonLnWithNumber_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerLnWithNumberStart.Enabled = dateTimePickerLnWithNumberEnd.Enabled = radioButtonLnWithNumber.Checked;
        }

        /// <summary>
        /// Включение/выключение всех компонентов на вкладке с первой частью локалиса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxUpperExtremityJoint_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var component in tabPageStLocalis1.Controls)
            {
                if (component is ComboBox)
                {
                    ((ComboBox)component).Enabled = checkBoxIsUpperExtremityJoint.Checked;
                }
                else if (component is TextBox)
                {
                    ((TextBox)component).Enabled = checkBoxIsUpperExtremityJoint.Checked;
                }
            }
        }

        private void numericUpDown1_Enter(object sender, EventArgs e)
        {
            numericUpDown1.Select(0, 10);
        }

        private void numericUpDown2_Enter(object sender, EventArgs e)
        {
            numericUpDown2.Select(0, 10);
        }

        private void numericUpDown3_Enter(object sender, EventArgs e)
        {
            numericUpDown3.Select(0, 10);

        }

        private void numericUpDown4_Enter(object sender, EventArgs e)
        {
            numericUpDown4.Select(0, 10);
        }
        
        private void MedicalInspectionForm_LocationChanged(object sender, EventArgs e)
        {
            if (_stopSaveParameters)
            {
                return;
            }

            _dbEngine.ConfigEngine.MedicalInspectionFormLocation = Location;
        }

        private void checkBoxTeoRisk_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxTeoRisk.Enabled = checkBox1.Enabled = checkBox2.Enabled = checkBox3.Enabled = checkBox4.Enabled =
            checkBox5.Enabled = checkBox6.Enabled = checkBox7.Enabled = checkBox8.Enabled = checkBox9.Enabled =
            checkBox10.Enabled = checkBox11.Enabled = checkBox12.Enabled = checkBoxTeoRisk.Checked;
        }

        /// <summary>
        /// Выбрать код WWW из списка всех кодов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelWWW_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MkbCodeFromMkbSelectForm = "";
            new MKBSelectForm(this, _dbEngine).ShowDialog();
            if (!string.IsNullOrEmpty(MkbCodeFromMkbSelectForm))
            {
                comboBoxWWW.Text = MkbCodeFromMkbSelectForm;
            }
        }

        private void comboBoxWWW_MouseEnter(object sender, EventArgs e)
        {
            string mkbName = _dbEngine.GetMkbName(comboBoxWWW.Text);

            if (!string.IsNullOrEmpty(mkbName))
            {
                toolTip1.Show(mkbName, comboBoxWWW, 15, -20);
            }
        }

        private void comboBoxWWW_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(comboBoxWWW);
        }

        private void linkLabelWWW_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Выбрать код из списка кодов", linkLabelWWW, 15, -20);
        }

        private void linkLabelWWW_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(linkLabelWWW);
        }
    }
}
