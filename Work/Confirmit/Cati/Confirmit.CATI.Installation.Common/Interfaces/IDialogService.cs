using System.Windows.Forms;

namespace Confirmit.CATI.Installation.Common.Interfaces
{
    public interface IDialogService
    {
         /// <summary>
        /// Show message box
        /// </summary>
        /// <param name="message">Text of message</param>
        /// <returns></returns>
        DialogResult Show(string message);

        /// <summary>
        /// Show message box
        /// </summary>
        /// <param name="message">Text of message</param>
        /// <param name="title">Title of message box</param>
        /// <returns></returns>
        DialogResult Show(string message, string title);

        /// <summary>
        /// Show message box
        /// </summary>
        /// <param name="message">Text of message</param>
        /// <param name="title">Title of message box</param>
        /// <param name="icon">Message box icon</param>
        /// <returns></returns>
        DialogResult Show(
            string message,
            string title,
            MessageBoxIcon icon);

        /// <summary>
        /// Show message box
        /// </summary>
        /// <param name="message">Text of message</param>
        /// <param name="title">Title of message box</param>
        /// <param name="buttons">Message box buttons</param>
        /// <param name="icon">Message box icon</param>
        /// <returns></returns>
        DialogResult Show(
            string message,
            string title,
            MessageBoxButtons buttons,
            MessageBoxIcon icon);
        
    }
}