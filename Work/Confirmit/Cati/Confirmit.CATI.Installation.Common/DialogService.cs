using System.Windows.Forms;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Installation.Common
{
    public class DialogService : IDialogService
    {
        public DialogResult Show(string message)
        {
            return Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        public DialogResult Show(string message, string title)
        {
            return Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        public DialogResult Show(
            string message,
            string title,
            MessageBoxIcon icon)
        {
            return Show(message, title, MessageBoxButtons.OK, icon);
        }

        public DialogResult Show(
            string message,
            string title,
            MessageBoxButtons buttons,
            MessageBoxIcon icon)
        {
            return MessageBox.Show(message, title, buttons, icon);
        }
    }
}