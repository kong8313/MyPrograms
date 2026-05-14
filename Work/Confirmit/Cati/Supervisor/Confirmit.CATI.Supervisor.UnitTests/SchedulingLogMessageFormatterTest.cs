using System.Linq;
using Confirmit.CATI.Supervisor.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class SchedulingLogMessageFormatterTest
    {
        private readonly SchedulingLogMessageFormatter _formatter = new SchedulingLogMessageFormatter();

        [TestMethod]
        public void FormatLogMessages_PlainText_ReturnsFormattedLines()
        {
            var messages = new[] { "Simple log message" };
            var result = _formatter.FormatLogMessages(messages).ToList();
            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result[0].Contains("Simple log message"));
        }

        [TestMethod]
        public void FormatLogMessages_HtmlTags_ReturnsEncoded()
        {
            var messages = new[] { "<html><body>Error</body></html>" };
            var result = _formatter.FormatLogMessages(messages).ToList();
            Assert.IsTrue(result.Any(line => line.Contains("&lt;html&gt;")));
            Assert.IsTrue(result.Any(line => line.Contains("&lt;body&gt;")));
            Assert.IsFalse(result.Any(line => line.Contains("<html>") && !line.Contains("&lt;")));
        }

        [TestMethod]
        public void FormatLogMessages_ScriptTag_ReturnsEncoded()
        {
            var messages = new[] { "<script>alert('xss')</script>" };
            var result = _formatter.FormatLogMessages(messages).ToList();
            Assert.IsTrue(result.Any(line => line.Contains("&lt;script&gt;")));
            Assert.IsTrue(result.Any(line => line.Contains("&#39;")));
        }

        [TestMethod]
        public void FormatLogMessages_Ampersand_ReturnsEncoded()
        {
            var messages = new[] { "Tom & Jerry" };
            var result = _formatter.FormatLogMessages(messages).ToList();
            Assert.IsTrue(result.Any(line => line.Contains("Tom &amp; Jerry")));
        }

        [TestMethod]
        public void FormatLogMessages_Quotes_ReturnsEncoded()
        {
            var messages = new[] { "He said \"Hello\"" };
            var result = _formatter.FormatLogMessages(messages).ToList();
            Assert.IsTrue(result.Any(line => line.Contains("&quot;")));
        }

        [TestMethod]
        public void FormatLogMessages_FourSpaces_ReplacedWithNbsp()
        {
            var messages = new[] { "    indented text" };
            var result = _formatter.FormatLogMessages(messages).ToList();
            Assert.IsTrue(result.Any(line => line.Contains("&nbsp;&nbsp;&nbsp;&nbsp;indented text")));
        }

        [TestMethod]
        public void FormatLogMessages_BoldTarget_WrappedInBoldTags()
        {
            var messages = new[] { "Starting scheduling script execution." };
            var result = _formatter.FormatLogMessages(messages).ToList();
            Assert.IsTrue(result.Any(line => line.Contains("<b>Starting scheduling script execution.</b>")));
        }

        [TestMethod]
        public void FormatLogMessages_InterviewKeyword_WrappedInBoldTags()
        {
            var messages = new[] { "Interview: some data" };
            var result = _formatter.FormatLogMessages(messages).ToList();
            Assert.IsTrue(result.Any(line => line.Contains("<b>Interview:</b>")));
        }

        [TestMethod]
        public void FormatLogMessages_LinesEndWithBrTag()
        {
            var messages = new[] { "Some text" };
            var result = _formatter.FormatLogMessages(messages).ToList();
            Assert.IsTrue(result.All(line => line.EndsWith("<br />")));
        }

        [TestMethod]
        public void FormatLogMessages_MultipleMessages_SeparatedByHr()
        {
            var messages = new[] { "Message 1", "Message 2" };
            var result = _formatter.FormatLogMessages(messages).ToList();
            Assert.IsTrue(result.Any(line => line.Contains("<hr>")));
        }

        [TestMethod]
        public void FormatLogMessages_ErrorPageHtml_RendersAsText()
        {
            var errorPageHtml = @"<html>
<head><title>500 Internal Server Error</title></head>
<body>
<h1>The server encountered an error</h1>
<p>Technical error details:</p>
<p>500: Internal Server Error</p>
</body>
</html>";
            var messages = new[] { errorPageHtml };
            var result = _formatter.FormatLogMessages(messages).ToList();
            
            Assert.IsTrue(result.Any(line => line.Contains("&lt;html&gt;")));
            Assert.IsTrue(result.Any(line => line.Contains("&lt;h1&gt;")));
            Assert.IsFalse(result.Any(line => line.Contains("<html>") && !line.Contains("&lt;html&gt;")));
        }

        [TestMethod]
        public void FormatLogMessages_AllBoldKeywords_AreFormatted()
        {
            var messages = new[] { 
                "Starting scheduling script execution.",
                "Finishing scheduling script execution.",
                "Response: data",
                "Respondent: info",
                "Interview: details",
                "Call: status"
            };
            var result = _formatter.FormatLogMessages(messages).ToList();
            
            Assert.IsTrue(result.Any(line => line.Contains("<b>Starting scheduling script execution.</b>")));
            Assert.IsTrue(result.Any(line => line.Contains("<b>Finishing scheduling script execution.</b>")));
            Assert.IsTrue(result.Any(line => line.Contains("<b>Response:</b>")));
            Assert.IsTrue(result.Any(line => line.Contains("<b>Respondent:</b>")));
            Assert.IsTrue(result.Any(line => line.Contains("<b>Interview:</b>")));
            Assert.IsTrue(result.Any(line => line.Contains("<b>Call:</b>")));
        }
    }
}
