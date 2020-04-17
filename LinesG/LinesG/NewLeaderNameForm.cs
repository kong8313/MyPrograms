using System;
using System.Windows.Forms;

namespace LinesG
{
    public partial class NewLeaderNameForm : Form
    {
        public string UserName { get; private set; }

        public NewLeaderNameForm()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text.Length == 0)
            {
                MessageBox.Show("Введите имя", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserName = textBoxName.Text;
            Close();
        }
    }
}
