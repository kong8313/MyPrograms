using System;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Setup.UnitTests.FakeClasses
{
    public class FakeExternalInvoker : IExternalInvoker
    {
        public bool DoesExecutionFinishWithError { get; set; }
        public string Output { get; set; }

        public FakeExternalInvoker(bool doesExecutionFinishWithError, string output)
        {
            DoesExecutionFinishWithError = doesExecutionFinishWithError;
            Output = output;
        }

        public string Invoke(string scriptNameOrPath, string args)
        {
            return Invoke(scriptNameOrPath, args, null, -1, true);
        }

        public string Invoke(string scriptNameOrPath, string args, int delay)
        {
            return Invoke(scriptNameOrPath, args, null, delay, true);
        }

        public string Invoke(string scriptNameOrPath, string args, bool isNeedToWait)
        {
            return Invoke(scriptNameOrPath, args, null, -1, isNeedToWait);
        }

        public string Invoke(string scriptNameOrPath, string args, string tempFolderPath)
        {
            return Invoke(scriptNameOrPath, args, tempFolderPath, -1, true);
        }

        public string Invoke(string scriptNameOrPath, string args, string tempFolderPath, int delay, bool isNeedToWait, bool doNotVerifyExitCode = false)
        {
            if (DoesExecutionFinishWithError)
            {
                throw new Exception("Wrong exit code");
            }

            return Output;
        }
    }
}