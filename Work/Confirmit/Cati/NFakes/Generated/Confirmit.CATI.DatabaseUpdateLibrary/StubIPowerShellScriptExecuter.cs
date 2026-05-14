using System;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes
{
    public class StubIPowerShellScriptExecuter : IPowerShellScriptExecutor 
    {
        private IPowerShellScriptExecutor _inner;

        public StubIPowerShellScriptExecuter()
        {
            _inner = null;
        }

        public IPowerShellScriptExecutor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string ExecuteStringStringDelegate(string databaseName, string scriptText);
        public ExecuteStringStringDelegate ExecuteStringString;

        string IPowerShellScriptExecutor.Execute(string databaseName, string scriptText)
        {


            if (ExecuteStringString != null)
            {
                return ExecuteStringString(databaseName, scriptText);
            } else if (_inner != null)
            {
                return ((IPowerShellScriptExecutor)_inner).Execute(databaseName, scriptText);
            }

            return default(string);
        }

    }
}