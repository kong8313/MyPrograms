using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace SqlServiceRunner
{
    [SuppressMessage("ReSharper", "LocalizableElement")]
    public class Logger
    {
        private readonly RichTextBox _richTextBoxLog;

        public Logger(RichTextBox richTextBoxLog)
        {
            _richTextBoxLog = richTextBoxLog;
        }

        public void WriteLog(string message)
        {
            _richTextBoxLog.Text = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss}: {message}\r\n" + _richTextBoxLog.Text;
        }
    }
}