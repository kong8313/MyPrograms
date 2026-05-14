using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class SchedulingLogMessageFormatter
    {
        private static readonly string[] BoldTargets = {
            "Starting scheduling script execution.",
            "Finishing scheduling script execution.",
            "Response:",
            "Respondent:",
            "Interview:",
            "Call:"
        };

        public IEnumerable<string> FormatLogMessages(IEnumerable<string> logMessages)
        {
            var messages = String.Join(
                Environment.NewLine + Environment.NewLine + "<hr>" + Environment.NewLine,
                logMessages.Select(HtmlEncodeMessage));

            foreach (var message in messages.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                yield return FormatLine(message);
            }
        }

        private string HtmlEncodeMessage(string message)
        {
            return HttpUtility.HtmlEncode(message);
        }

        private string FormatLine(string line)
        {
            var text = line.Replace("    ", "&nbsp;&nbsp;&nbsp;&nbsp;");
            text = BoldTargets.Aggregate(text, MakeBold);
            return text + "<br />";
        }

        private static string MakeBold(string container, string text)
        {
            return container.Replace(text, $"<b>{text}</b>");
        }
    }
}
