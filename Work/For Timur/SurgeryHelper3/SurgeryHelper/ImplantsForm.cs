using System;
using System.Text;
using System.Windows.Forms;
using SurgeryHelper.Engines;

namespace SurgeryHelper
{
    public partial class ImplantsForm : Form
    {
        private readonly DbEngine _dbEngine;
        private readonly OperationProtocolForm _operationProtocolForm;
        private readonly string _objectBoxNameOnForm;

        public ImplantsForm(DbEngine dbEngine, OperationProtocolForm operationProtocolForm, string objectBoxNameOnForm)
        {
            InitializeComponent();

            _dbEngine = dbEngine;
            _operationProtocolForm = operationProtocolForm;
            _objectBoxNameOnForm = objectBoxNameOnForm;
        }

        private void ImplantsForm_Load(object sender, EventArgs e)
        {
            ShowImplants();
        }

        /// <summary>
        /// Показать список имплантатов
        /// </summary>
        private void ShowImplants()
        {
            int listCnt = 0;
            int orderlyCnt = 0;
            while (listCnt < checkedListBoxImplants.Items.Count && orderlyCnt < _dbEngine.ImplantList.Count)
            {
                checkedListBoxImplants.Items[listCnt] = _dbEngine.ImplantList[orderlyCnt].LastNameWithInitials;
                listCnt++;
                orderlyCnt++;
            }

            if (orderlyCnt == _dbEngine.ImplantList.Count)
            {
                while (listCnt < checkedListBoxImplants.Items.Count)
                {
                    checkedListBoxImplants.Items.RemoveAt(listCnt);
                }
            }
            else
            {
                while (orderlyCnt < _dbEngine.ImplantList.Count)
                {
                    checkedListBoxImplants.Items.Add(_dbEngine.ImplantList[orderlyCnt].LastNameWithInitials);
                    orderlyCnt++;
                }
            }
        }

        /// <summary>
        /// Добавить новый имплантат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            new ImplantsViewForm(_dbEngine, null).ShowDialog();
            ShowImplants();
        }

        /// <summary>
        /// Удалить выделенный имплантат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (checkedListBoxImplants.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Нет выделенных записей", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int currentNumber = checkedListBoxImplants.SelectedIndex;
                if (DialogResult.Yes == MessageBox.Show("Вы уверены, что хотите удалить имплантат " + checkedListBoxImplants.Items[currentNumber] + "?\r\nДанная операция необратима.", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _dbEngine.RemoveImplant(_dbEngine.ImplantList[currentNumber].Id);
                }

                ShowImplants();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Редактировать выделенный имплантат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (checkedListBoxImplants.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Нет выделенных записей", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            new ImplantsViewForm(_dbEngine, _dbEngine.ImplantList[checkedListBoxImplants.SelectedIndices[0]]).ShowDialog();
            ShowImplants();
        }

        /// <summary>
        /// Отобразить на форме с операциями выбранные имплантаты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (checkedListBoxImplants.CheckedItems.Count == 0)
            {
                Close();
                return;
            }

            var orderlyMultilineStr = new StringBuilder();
            for (int i = 0; i < checkedListBoxImplants.CheckedItems.Count; i++)
            {
                orderlyMultilineStr.Append(checkedListBoxImplants.CheckedItems[i] + "\r\n");
            }

            if (orderlyMultilineStr.Length > 2)
            {
                orderlyMultilineStr.Remove(orderlyMultilineStr.Length - 2, 2);
            }

            _operationProtocolForm.PutStringToObject(_objectBoxNameOnForm, orderlyMultilineStr.ToString());

            Close();
        }

        #region Подсказки
        private void buttonAdd_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Добавить новый имплантат", buttonAdd, 15, -20);
            buttonAdd.FlatStyle = FlatStyle.Popup;
        }

        private void buttonAdd_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonAdd);
            buttonAdd.FlatStyle = FlatStyle.Flat;
        }

        private void buttonDelete_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Удалить выбранный имплантат", buttonDelete, 15, -20);
            buttonDelete.FlatStyle = FlatStyle.Popup;
        }

        private void buttonDelete_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonDelete);
            buttonDelete.FlatStyle = FlatStyle.Flat;
        }

        private void buttonEdit_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Редактировать выбранный имплантат", buttonEdit, 15, -20);
            buttonEdit.FlatStyle = FlatStyle.Popup;
        }

        private void buttonEdit_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonEdit);
            buttonEdit.FlatStyle = FlatStyle.Flat;
        }

        private void buttonOk_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Подтвердить выбор имплантатов", buttonOk, 15, -20);
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