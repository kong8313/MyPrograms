using System.Drawing;
using System.Windows.Forms;

namespace Confirmit.CATI.Installation.Common
{
    public static class TopMostMessageBox
    {
        public static bool IsQuietMode;

        /// <summary>
        /// Show message box over all windows
        /// </summary>
        /// <param name="message">Text of message</param>
        /// <returns></returns>
        public static DialogResult Show(string message)
        {
            return Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, DialogResult.OK);
        }

        /// <summary>
        /// Show message box over all windows
        /// </summary>
        /// <param name="message">Text of message</param>
        /// <param name="title">Title of message box</param>
        /// <returns></returns>
        public static DialogResult Show(string message, string title)
        {
            return Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.None, DialogResult.OK);
        }

        /// <summary>
        /// Show message box over all windows
        /// </summary>
        /// <param name="message">Text of message</param>
        /// <param name="title">Title of message box</param>
        /// <param name="icon">Message box icon</param>
        /// <returns></returns>
        public static DialogResult Show(
            string message,
            string title,
            MessageBoxIcon icon)
        {
            return Show(message, title, MessageBoxButtons.OK, icon, DialogResult.OK);
        }

        /// <summary>
        /// Show message box over all windows
        /// </summary>
        /// <param name="message">Text of message</param>
        /// <param name="title">Title of message box</param>
        /// <param name="buttons">Message box buttons</param>
        /// <param name="icon">Message box icon</param>
        /// <param name="defaultDialogResult">This value will be returned, if quiet mode is enabled</param>
        /// <returns></returns>
        public static DialogResult Show(
            string message,
            string title,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            DialogResult defaultDialogResult)
        {
            if (IsQuietMode)
            {
                return defaultDialogResult;
            }

            DialogResult result;

            // Create a host form that is a TopMost window which will be the 
            // parent of the MessageBox
            using (var topmostForm = new Form
            {
                Size = new Size(1, 1),
                StartPosition = FormStartPosition.Manual,
                Icon = Properties.Resources.message,
                Text = title
            })
            {
                // We do not want anyone to see this window so position it off the 
                // visible screen and make it as small as possible
                Rectangle rect = SystemInformation.VirtualScreen;
                topmostForm.Location = new Point(rect.Bottom + 10, rect.Right + 10);
                topmostForm.Show();

                // Make this form the active form and make it TopMost
                topmostForm.Focus();
                topmostForm.BringToFront();
                topmostForm.TopMost = true;

                // Finally show the MessageBox with the form just created as its owner
                result = MessageBox.Show(topmostForm, message, title, buttons, icon);
            }

            return result;
        }
    }
}
