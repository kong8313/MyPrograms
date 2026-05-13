using System;
using System.Reflection;
using System.Windows.Forms;

namespace SurgeryHelper
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Выход при нажатии Esc
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Закрытие формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_MouseEnter(object sender, EventArgs e)
        {
            buttonOK.FlatStyle = FlatStyle.Popup;
        }

        private void buttonOK_MouseLeave(object sender, EventArgs e)
        {
            buttonOK.FlatStyle = FlatStyle.Flat;
        }

        private void AboutForm_Shown(object sender, EventArgs e)
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            labelInfo.Text = $"Программа \"Помощник хирурга\" v. {v.Major}.{v.Minor}.{v.Build}\r\n\r\nПрограммист: Коновалов Григорий\r\nВсе права на программу принадлежат \r\nТорно Тимуру Эдуардовичу\r\nПожелания и предложения направляйте\r\nпо адресу: torno@mail.ru";
        }
    }
}
