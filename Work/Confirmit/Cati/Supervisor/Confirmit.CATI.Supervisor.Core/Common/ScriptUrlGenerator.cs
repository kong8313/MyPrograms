using System.Text;

namespace Confirmit.CATI.Supervisor.Core.Common
{
    public class ScriptUrlGenerator
    {
        private readonly StringBuilder _url;
        private bool _hasQuestionMark;

        public ScriptUrlGenerator(string basePath)
        {
            _url = new StringBuilder(string.Format(@"'{0}'", basePath));
            _hasQuestionMark = basePath.Contains("?");
        }

        private char GetSeparator()
        {
            char result = _hasQuestionMark ? '&' : '?';
            _hasQuestionMark = true;
            return result;
        }

        public void AddStaticParameter(string name, string value)
        {
            _url.AppendFormat(@" + '{0}{1}={2}'", GetSeparator(), name, value);
        }

        public void AddScriptParameter(string name, string valueScript)
        {
            _url.AppendFormat(@" + '{0}{1}=' + {2}", GetSeparator(), name, valueScript);
        }

        public string GetResult()
        {
            // append constant JS strings here to reduce script size and make resulting script more readable.
            return _url.Replace(@"' + '", string.Empty).ToString();
        }
    }
}