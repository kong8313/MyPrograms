using System.Windows.Forms;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Setup.UnitTests.FakeClasses
{
    public class FakeDialogService : IDialogService
    {
        public DialogResult DefaultDialogResult { get; set; }

        public int ExecutingCount { get; private set; }
        public int ErrorExecutingCount { get; private set; }
        public int WarningExecutingCount { get; private set; }
        public int InformationExecutingCount { get; private set; }

        public FakeDialogService(DialogResult defaultDialogResult)
        {
            DefaultDialogResult = defaultDialogResult;
            ExecutingCount = ErrorExecutingCount = WarningExecutingCount = InformationExecutingCount = 0;
        }

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
            ExecutingCount++;
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    ErrorExecutingCount++;
                    break;
                case MessageBoxIcon.Warning:
                    WarningExecutingCount++;
                    break;
                case MessageBoxIcon.Information:
                    InformationExecutingCount++;
                    break;
            }
            

            return DefaultDialogResult;
        }
    }
}