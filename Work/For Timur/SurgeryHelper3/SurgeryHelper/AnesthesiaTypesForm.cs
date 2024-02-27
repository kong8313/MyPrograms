using System;
using System.Windows.Forms;
using SurgeryHelper.Engines;

namespace SurgeryHelper
{
    public partial class AnesthesiaTypesForm : Form
    {
        private readonly DbEngine _dbEngine;
        private readonly OperationViewForm _operationViewForm;

        public AnesthesiaTypesForm(DbEngine dbEngine, OperationViewForm operationViewForm)
        {
            InitializeComponent();

            _dbEngine = dbEngine;
            _operationViewForm = operationViewForm;
        }

        private void AnesthesiaTypesForm_Load(object sender, EventArgs e)
        {
            ShowAnesthesiaTypes();
        }

        /// <summary>
        /// Показать список типов анестезий
        /// </summary>
        private void ShowAnesthesiaTypes()
        {
            int listCnt = 0;
            int orderlyCnt = 0;
            while (listCnt < checkedListBoxAnesthesiaTypes.Items.Count && orderlyCnt < _dbEngine.AnesthesiaTypesList.Count)
            {
                checkedListBoxAnesthesiaTypes.Items[listCnt] = _dbEngine.AnesthesiaTypesList[orderlyCnt].LastNameWithInitials;
                listCnt++;
                orderlyCnt++;
            }

            if (orderlyCnt == _dbEngine.AnesthesiaTypesList.Count)
            {
                while (listCnt < checkedListBoxAnesthesiaTypes.Items.Count)
                {
                    checkedListBoxAnesthesiaTypes.Items.RemoveAt(listCnt);
                }
            }
            else
            {
                while (orderlyCnt < _dbEngine.AnesthesiaTypesList.Count)
                {
                    checkedListBoxAnesthesiaTypes.Items.Add(_dbEngine.AnesthesiaTypesList[orderlyCnt].LastNameWithInitials);
                    orderlyCnt++;
                }
            }
        }

        /// <summary>
        /// Добавить новый тип анестезии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            new AnesthesiaTypeViewForm(_dbEngine, null).ShowDialog();
            ShowAnesthesiaTypes();
        }

        /// <summary>
        /// Удалить выделенный тип анестезии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (checkedListBoxAnesthesiaTypes.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Нет выделенных записей", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int currentNumber = checkedListBoxAnesthesiaTypes.SelectedIndex;
                if (DialogResult.Yes == MessageBox.Show("Вы уверены, что хотите удалить тип анестезии " + checkedListBoxAnesthesiaTypes.Items[currentNumber] + "?\r\nДанная операция необратима.", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _dbEngine.RemoveAnesthesiaType(_dbEngine.AnesthesiaTypesList[currentNumber].Id);
                }

                ShowAnesthesiaTypes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Редактировать выделенный тип анестезии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (checkedListBoxAnesthesiaTypes.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Нет выделенных записей", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            new AnesthesiaTypeViewForm(_dbEngine, _dbEngine.AnesthesiaTypesList[checkedListBoxAnesthesiaTypes.SelectedIndices[0]]).ShowDialog();
            ShowAnesthesiaTypes();
        }

        /// <summary>
        /// Отобразить на форме с операциями выбранные типы анестезий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (checkedListBoxAnesthesiaTypes.SelectedItems.Count == 0)
            {
                Close();
                return;
            }

            _operationViewForm.PutStringToObject("comboBoxAnesthesiaType", checkedListBoxAnesthesiaTypes.SelectedItem.ToString());
            Close();
        }

        #region Подсказки
        private void buttonAdd_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Добавить новый тим анестезии", buttonAdd, 15, -20);
            buttonAdd.FlatStyle = FlatStyle.Popup;
        }

        private void buttonAdd_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonAdd);
            buttonAdd.FlatStyle = FlatStyle.Flat;
        }

        private void buttonDelete_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Удалить выбранный тип анестезии", buttonDelete, 15, -20);
            buttonDelete.FlatStyle = FlatStyle.Popup;
        }

        private void buttonDelete_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonDelete);
            buttonDelete.FlatStyle = FlatStyle.Flat;
        }

        private void buttonEdit_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Редактировать выбранный тип анестезии", buttonEdit, 15, -20);
            buttonEdit.FlatStyle = FlatStyle.Popup;
        }

        private void buttonEdit_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonEdit);
            buttonEdit.FlatStyle = FlatStyle.Flat;
        }

        private void buttonOk_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Подтвердить выбор типов анестезии", buttonOk, 15, -20);
            buttonOk.FlatStyle = FlatStyle.Popup;
        }

        private void buttonOk_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonOk);
            buttonOk.FlatStyle = FlatStyle.Flat;
        }
        #endregion

        /// <summary>
        /// Выбор типа анестезии двойным кликом
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBoxAnesthesiaTypes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (checkedListBoxAnesthesiaTypes.SelectedItems.Count != 0)
            {
                buttonOk_Click(null, null);
            }
        }
       
    }
}