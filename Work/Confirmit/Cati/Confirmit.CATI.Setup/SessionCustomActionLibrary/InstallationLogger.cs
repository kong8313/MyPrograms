using System;
using System.Diagnostics;
using System.Text;
using Confirmit.CATI.Installation.Common.Interfaces;
using Microsoft.Deployment.WindowsInstaller;

namespace SessionCustomAction
{
    public class InstallationLogger : ILogger
    {
        private readonly Session _session;
        private readonly bool _useStandardLog;
        private readonly string[] _secretLogWords;

        public InstallationLogger(Session session)
            : this(session, false)
        {
        }

        public InstallationLogger(Session session, bool useStandardLog)
            : this(session, useStandardLog, new string[0])
        {

        }

        public InstallationLogger(Session session, bool useStandardLog, string[] secretLogWords)
        {
            _session = session;
            _useStandardLog = useStandardLog;
            _secretLogWords = secretLogWords;
        }

        public void WriteLog(string message, params object[] parameters)
        {
            WriteLog(false, message, parameters);
        }

        public void WriteLog(bool isPrintOnConsole, string message, params object[] parameters)
        {
            WriteLog(isPrintOnConsole, TraceEventType.Information, message, parameters);
        }

        public void WriteLog(TraceEventType traceType, string message, params object[] parameters)
        {
            WriteLog(false, traceType, message, parameters);
        }

        public void WriteLog(bool isPrintOnConsole, TraceEventType traceType, string message, params object[] parameters)
        {
            if (parameters.Length > 0)
            {
                message = string.Format(message, parameters);
            }

            message = traceType + ": " + HideSecretWords(message);

            if (_useStandardLog)
            {
                _session.Log(string.Format("{0} {1}", DateTime.Now.ToString("HH:mm:ss.ms"), message));
            }
            else
            {
                int cnt = 0;
                while (cnt < message.Length)
                {
                    int len = Math.Min(500, message.Length - cnt);
                    _session["MYDEBUG"] = message.Substring(cnt, len);
                    _session["MYDEBUG"] = string.Empty;
                    cnt += len;
                }
            }

            if (isPrintOnConsole)
            {
                Console.WriteLine(message);
            }
        }

        private string HideSecretWords(string message)
        {
            message = " " + message + " ";

            foreach (var secretLogWord in _secretLogWords)
            {
                if (string.IsNullOrWhiteSpace(secretLogWord))
                {
                    continue;
                }

                int n = 0;
                while ((n = message.IndexOf(secretLogWord, n, StringComparison.OrdinalIgnoreCase)) != -1)
                {
                    if (!IsAlphaOrDigitOrUnderscore(message[n - 1]) && !IsAlphaOrDigitOrUnderscore(message[n + secretLogWord.Length]))
                    {
                        message = message.Substring(0, n) + GetStars(secretLogWord.Length) + message.Substring(n + secretLogWord.Length);
                    }

                    n += secretLogWord.Length;
                }
            }

            return message.Substring(1, message.Length - 2);
        }

        private bool IsAlphaOrDigitOrUnderscore(char ch)
        {
            return (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '_';
        }

        private string GetStars(int starsCount)
        {
            var stars = new StringBuilder();

            for (int i = 0; i < starsCount; i++)
            {
                stars.Append("*");
            }

            return stars.ToString();
        }
    }
}