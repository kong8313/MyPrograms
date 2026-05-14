using System;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes
{
    public class StubIPowerShellScriptExecutor : IPowerShellScriptExecutor 
    {
        private IPowerShellScriptExecutor _inner;

        public StubIPowerShellScriptExecutor()
        {
            _inner = null;
        }

        public IPowerShellScriptExecutor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string ExecuteILoggerStringDelegate(ILogger logger, string scriptText);
        public ExecuteILoggerStringDelegate ExecuteILoggerString;

        string IPowerShellScriptExecutor.Execute(ILogger logger, string scriptText)
        {


            if (ExecuteILoggerString != null)
            {
                return ExecuteILoggerString(logger, scriptText);
            } else if (_inner != null)
            {
                return ((IPowerShellScriptExecutor)_inner).Execute(logger, scriptText);
            }

            return default(string);
        }

    }
}