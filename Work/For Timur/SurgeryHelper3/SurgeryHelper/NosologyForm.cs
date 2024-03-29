﻿using System;
using System.Windows.Forms;
using SurgeryHelper.Engines;

namespace SurgeryHelper
{
    public partial class NosologyForm : Form
    {
        private readonly DbEngine _dbEngine;
        private readonly PatientViewForm _patientViewForm;

        public NosologyForm(DbEngine dbEngine, PatientViewForm patientViewForm)
        {
            InitializeComponent();

            _dbEngine = dbEngine;
            _patientViewForm = patientViewForm;
        }

        private void NosologyForm_Load(object sender, EventArgs e)
        {
            ShowNosologyes();
        }

        /// <summary>
        /// Показать список нозологий
        /// </summary>
        private void ShowNosologyes()
        {
            int listCnt = 0;
            int nosologyCnt = 0;
            while (listCnt < NosologiesList.Rows.Count && nosologyCnt < _dbEngine.NosologyList.Count)
            {
                NosologiesList.Rows[listCnt].Cells[0].Value = _dbEngine.NosologyList[nosologyCnt].LastNameWithInitials;
                NosologiesList.Rows[listCnt].Cells[1].Value = _dbEngine.NosologyList[nosologyCnt].DairyInfo;
                listCnt++;
                nosologyCnt++;
            }

            if (nosologyCnt == _dbEngine.NosologyList.Count)
            {
                while (listCnt < NosologiesList.Rows.Count)
                {
                    NosologiesList.Rows.RemoveAt(listCnt);
                }
            }
            else
            {
                while (nosologyCnt < _dbEngine.NosologyList.Count)
                {
                    var param = new[]
                    {
                        _dbEngine.NosologyList[nosologyCnt].LastNameWithInitials,
                        _dbEngine.NosologyList[nosologyCnt].DairyInfo
                    };
                    NosologiesList.Rows.Add(param);
                    nosologyCnt++;
                }
            }
        }

        /// <summary>
        /// Добавить новую нозологию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            new NosologyViewForm(_dbEngine, null).ShowDialog();
            ShowNosologyes();
        }

        /// <summary>
        /// Удалить выделенную нозологию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int currentNumber = NosologiesList.CurrentCellAddress.Y;
            if (currentNumber < 0)
            {
                MessageBox.Show("Нет выделенных записей", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            new NosologyRemoveForm(_dbEngine, _dbEngine.NosologyList[currentNumber]).ShowDialog();
            ShowNosologyes();
        }

        /// <summary>
        /// Редактировать выделенную нозологию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            int currentNumber = NosologiesList.CurrentCellAddress.Y;
            if (currentNumber < 0)
            {
                MessageBox.Show("Нет выделенных записей", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            new NosologyViewForm(_dbEngine, _dbEngine.NosologyList[currentNumber]).ShowDialog();
            ShowNosologyes();
        }

        /// <summary>
        /// Отобразить на форме с операциями выбранную нозологию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            int currentNumber = NosologiesList.CurrentCellAddress.Y;
            if (currentNumber < 0)
            {
                Close();
                return;
            }

            try
            {
                string nosologyName = NosologiesList.Rows[currentNumber].Cells[0].Value.ToString();

                _patientViewForm.PutObjectsToComboBox(_dbEngine.NosologyList.ToArray(), _patientViewForm.comboBoxNosology);
                _patientViewForm.PutStringToObject("comboBoxNosology", nosologyName);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Выбор нозологии двойным кликом
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NosologiesList_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                buttonOk_Click(null, null);
            }
        }

        #region Подсказки
        private void buttonAdd_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Добавить новую нозологию", buttonAdd, 15, -20);
            buttonAdd.FlatStyle = FlatStyle.Popup;
        }

        private void buttonAdd_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonAdd);
            buttonAdd.FlatStyle = FlatStyle.Flat;
        }

        private void buttonDelete_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Удалить выбранную нозологию", buttonDelete, 15, -20);
            buttonDelete.FlatStyle = FlatStyle.Popup;
        }

        private void buttonDelete_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonDelete);
            buttonDelete.FlatStyle = FlatStyle.Flat;
        }

        private void buttonEdit_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Редактировать выбранную нозологию", buttonEdit, 15, -20);
            buttonEdit.FlatStyle = FlatStyle.Popup;
        }

        private void buttonEdit_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonEdit);
            buttonEdit.FlatStyle = FlatStyle.Flat;
        }

        private void buttonOk_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Подтвердить выбор нозологии", buttonOk, 15, -20);
            buttonOk.FlatStyle = FlatStyle.Popup;
        }

        private void buttonOk_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonOk);
            buttonOk.FlatStyle = FlatStyle.Flat;
        }
        #endregion
    }
}
