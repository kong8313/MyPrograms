using System;
using System.Windows.Forms;
using SurgeryHelper.Engines;
using SurgeryHelper.Entities;

namespace SurgeryHelper
{
    public partial class OperationProtocolForm : Form
    {
        private readonly OperationClass _operationInfo;
        private readonly PatientClass _patientInfo;
        private bool _isFormClosingByButton;
        private bool _stopSaveParameters;
        private readonly DbEngine _dbEngine;

        public OperationProtocolForm(OperationClass operationInfo, PatientClass patientInfo, DbEngine dbEngine)
        {
            _stopSaveParameters = true;

            InitializeComponent();

            _dbEngine = dbEngine;
            _operationInfo = operationInfo;
            _patientInfo = patientInfo;

            comboBoxAntibioticProphylaxis.Items.Clear();
            comboBoxAntibioticProphylaxis.Items.AddRange(_dbEngine.ConfigEngine.OperationProtocolFormLastAntibioticProphylaxis);

            comboBoxPremedication.Items.Clear();
            comboBoxPremedication.Items.AddRange(_dbEngine.ConfigEngine.OperationProtocolFormLastPremedication);
        }

        private void OperationProtocolForm_Load(object sender, EventArgs e)
        {
            if (_dbEngine.ConfigEngine.MedicalInspectionFormLocation.X >= 0 &&
                _dbEngine.ConfigEngine.MedicalInspectionFormLocation.Y >= 0)
            {
                Location = _dbEngine.ConfigEngine.MedicalInspectionFormLocation;
            }

            checkBoxDnevnik.Checked = _operationInfo.BeforeOperationEpicrisisIsDairyEnabled;

            numericUpDownADFirst.Value = _operationInfo.BeforeOperationEpicrisisADFirst;
            numericUpDownADSecond.Value = _operationInfo.BeforeOperationEpicrisisADSecond;
            comboBoxBreath.Text = _operationInfo.BeforeOperationEpicrisisBreath;
            numericUpDownChDD.Value = _operationInfo.BeforeOperationEpicrisisChDD;
            textBoxComplaints.Text = _operationInfo.BeforeOperationEpicrisisComplaints;
            comboBoxState.Text = _operationInfo.BeforeOperationEpicrisisState;
            comboBoxHeartRhythm.Text = _operationInfo.BeforeOperationEpicrisisHeartRhythm;
            comboBoxHeartSounds.Text = _operationInfo.BeforeOperationEpicrisisHeartSounds;
            numericUpDownPulse.Value = _operationInfo.BeforeOperationEpicrisisPulse;
            textBoxStLocalis.Text = _operationInfo.BeforeOperationEpicrisisStLocalis;
            textBoxTemperature.Text = _operationInfo.BeforeOperationEpicrisisTemperature;
            comboBoxState.Text = _operationInfo.BeforeOperationEpicrisisState;

            textBoxWheeze.Text = _operationInfo.BeforeOperationEpicrisisWheeze;

            checkBoxAntibioticProphylaxis.Checked = _operationInfo.BeforeOperationEpicrisisIsAntibioticProphylaxisExist;
            comboBoxAntibioticProphylaxis.Text = _operationInfo.BeforeOperationEpicrisisAntibioticProphylaxis;
            checkBoxPremedication.Checked = _operationInfo.BeforeOperationEpicrisisIsPremedicationExist;
            comboBoxPremedication.Text = _operationInfo.BeforeOperationEpicrisisPremedication;

            textBoxOperationCourse.Text = _operationInfo.OperationCourse;
            textBoxImplants.Text = ConvertEngine.ListToMultilineString(_operationInfo.Implants);

            _stopSaveParameters = false;
        }

        private void buttonDocuments_Click(object sender, EventArgs e)
        {
            var tempOperationInfo = new OperationClass(_operationInfo);
            var tempPatientInfo = new PatientClass(_patientInfo);

            PutDataToOperation(tempOperationInfo);

            new WordExportEngine(_dbEngine).ExportOperationProtocol(tempOperationInfo, tempPatientInfo);
        }

        /// <summary>
        /// Положить введённые данные в операцию и пациента
        /// </summary>
        /// <param name="operationInfo">Информация про операцию</param>
        private void PutDataToOperation(OperationClass operationInfo)
        {
            operationInfo.BeforeOperationEpicrisisIsDairyEnabled = checkBoxDnevnik.Checked;

            operationInfo.BeforeOperationEpicrisisADFirst = (int)numericUpDownADFirst.Value;
            operationInfo.BeforeOperationEpicrisisADSecond = (int)numericUpDownADSecond.Value;
            operationInfo.BeforeOperationEpicrisisBreath = comboBoxBreath.Text;
            operationInfo.BeforeOperationEpicrisisChDD = (int)numericUpDownChDD.Value;
            operationInfo.BeforeOperationEpicrisisComplaints = textBoxComplaints.Text;
            operationInfo.BeforeOperationEpicrisisState = comboBoxState.Text;
            operationInfo.BeforeOperationEpicrisisHeartRhythm = comboBoxHeartRhythm.Text;
            operationInfo.BeforeOperationEpicrisisHeartSounds = comboBoxHeartSounds.Text;
            operationInfo.BeforeOperationEpicrisisPulse = (int)numericUpDownPulse.Value;
            operationInfo.BeforeOperationEpicrisisStLocalis = textBoxStLocalis.Text;
            operationInfo.BeforeOperationEpicrisisTemperature = textBoxTemperature.Text;
            operationInfo.BeforeOperationEpicrisisWheeze = textBoxWheeze.Text;

            operationInfo.BeforeOperationEpicrisisIsAntibioticProphylaxisExist = checkBoxAntibioticProphylaxis.Checked;
            operationInfo.BeforeOperationEpicrisisAntibioticProphylaxis = comboBoxAntibioticProphylaxis.Text;
            operationInfo.BeforeOperationEpicrisisIsPremedicationExist = checkBoxPremedication.Checked;
            operationInfo.BeforeOperationEpicrisisPremedication = comboBoxPremedication.Text;

            operationInfo.OperationCourse = textBoxOperationCourse.Text.TrimEnd('\r', '\n');
            operationInfo.Implants = ConvertEngine.MultilineStringToList(textBoxImplants.Text);

            _dbEngine.ConfigEngine.OperationProtocolFormLastAntibioticProphylaxis = ConvertEngine.GetLastUsedValues(comboBoxAntibioticProphylaxis);
            _dbEngine.ConfigEngine.OperationProtocolFormLastPremedication = ConvertEngine.GetLastUsedValues(comboBoxPremedication);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            try
            {
                PutDataToOperation(_operationInfo);

                _isFormClosingByButton = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            _isFormClosingByButton = true;
            Close();
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


        private void OperationProtocolForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isFormClosingByButton)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Разрешение/запрещение на ввод данных для дневника предоперационного эпикриза
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxDnevnik_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTemperature.Enabled = textBoxComplaints.Enabled = comboBoxState.Enabled =
            textBoxStLocalis.Enabled = 
            numericUpDownChDD.Enabled = comboBoxBreath.Enabled = textBoxWheeze.Enabled =
            comboBoxHeartSounds.Enabled = comboBoxHeartRhythm.Enabled = 
            numericUpDownPulse.Enabled = numericUpDownADFirst.Enabled = 
            numericUpDownADSecond.Enabled = checkBoxDnevnik.Checked;
        }

        private void OperationProtocolForm_LocationChanged(object sender, EventArgs e)
        {
            if (_stopSaveParameters)
            {
                return;
            }

            _dbEngine.ConfigEngine.MedicalInspectionFormLocation = Location;
        }

        private void checkBoxAntibioticProphylaxis_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxAntibioticProphylaxis.Enabled = checkBoxAntibioticProphylaxis.Checked;
        }

        private void checkBoxPremedication_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxPremedication.Enabled = checkBoxPremedication.Checked;
        }

        /// <summary>
        /// Открыть список с имплантатами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelImplantList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new ImplantsForm(_dbEngine, this, "textBoxImplants").ShowDialog();
        }

        /// <summary>
        /// Поместить строку в указанный объект
        /// </summary>
        /// <param name="objectName">Название объекта, куда класть текст</param>
        /// <param name="str">Текст, который надо положить в объект</param>
        public void PutStringToObject(string objectName, string str)
        {
            switch (objectName)
            {
                case "textBoxImplants":
                    if (!string.IsNullOrEmpty(textBoxImplants.Text))
                    {
                        if (!textBoxImplants.Text.EndsWith("\r\n"))
                        {
                            textBoxImplants.Text += "\r\n";
                        }

                        textBoxImplants.Text += str;
                    }
                    else
                    {
                        textBoxImplants.Text = str;
                    }

                    break;
            }
        }
    }
}
