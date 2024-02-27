using System;
using System.Windows.Forms;
using SurgeryHelper.Engines;
using SurgeryHelper.Entities;

namespace SurgeryHelper
{
    public partial class AnesthesiaTypeViewForm : Form
    {
        private readonly DbEngine _dbEngine;
        private readonly AnesthesiaTypesClass _anesthesiaTypeInfo;
        private bool _isFormClosingByButton;

        public sealed override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
            }
        }

        public AnesthesiaTypeViewForm(DbEngine dbEngine, AnesthesiaTypesClass anesthesiaTypeInfo)
        {
            InitializeComponent();

            _dbEngine = dbEngine;            

            if (anesthesiaTypeInfo == null)
            {
                _anesthesiaTypeInfo = new AnesthesiaTypesClass();
                Text = "Добавление нового типа анестезии";
            }
            else
            {
                _anesthesiaTypeInfo = anesthesiaTypeInfo;
                Text = "Редактирование типа анестезии";
                textBoxAnesthesiaType.Text = _anesthesiaTypeInfo.LastNameWithInitials;
            }
        }


        private void buttonOk_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Подтвердить", buttonOk, 15, -20);
            buttonOk.FlatStyle = FlatStyle.Popup;
        }

        private void buttonOk_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonOk);
            buttonOk.FlatStyle = FlatStyle.Flat;
        }

        private void buttonClose_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Отменить", buttonClose, 15, -20);
            buttonClose.FlatStyle = FlatStyle.Popup;
        }

        private void buttonClose_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(buttonClose);
            buttonClose.FlatStyle = FlatStyle.Flat;
        }

        /// <summary>
        /// Сохранение изменений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxAnesthesiaType.Text))
            {
                MessageBox.Show("Поля, отмеченные звёздочкой, обязательны для заполнения", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _anesthesiaTypeInfo.LastNameWithInitials = textBoxAnesthesiaType.Text;

                if (_anesthesiaTypeInfo.Id == 0)
                {
                    _dbEngine.AddAnesthesiaType(_anesthesiaTypeInfo);
                }
                else
                {
                    _dbEngine.UpdateAnesthesiaType(_anesthesiaTypeInfo);
                }

                _isFormClosingByButton = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Закрытие формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            _isFormClosingByButton = true;
            Close();
        }

        /// <summary>
        /// Отлов нажатия кнопок на форме
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                buttonOk_Click(null, null);
                return true;
            }

            if (keyData == Keys.Escape)
            {
                buttonClose_Click(null, null);
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void AnesthesiaTypeViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isFormClosingByButton)
            {
                e.Cancel = true;
            }
        }
    }
}
