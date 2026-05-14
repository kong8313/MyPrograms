using System;
using System.Data.SqlClient;
using System.Threading;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationRetry : IAsyncOperationRetry
    {
        private readonly ISystemSettings _settings;

        public AsyncOperationRetry(ISystemSettings settings)
        {
            _settings = settings;
        }

        public void ExecuteAction(Action action)
        {
            int attemptNumber = 0;
            bool attemptIsFailed;
            do
            {
                try
                {
                    action();
                    attemptIsFailed = false;
                }
                catch (SqlException e)
                {
                    TraceHelper.TraceException(e);

                    attemptIsFailed = true;
                    ++attemptNumber;
                    Thread.Sleep(_settings.AsyncOperations.DelayBetweenRetriesInSeconds*1000);
                }
            } while (attemptIsFailed && attemptNumber < _settings.AsyncOperations.NumberOfRetries);
        }
    }
}