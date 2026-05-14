using System;
using System.Data.SqlClient;
using System.Diagnostics;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services
{
    class RetryingService : IRetryingService
    {
        private readonly IRetryingServiceSettings _retryingServiceSettings;
        private readonly IAsyncManager _asyncManager;

        public RetryingService(
            IRetryingServiceSettings settings,
            IAsyncManager asyncManager)
        {
            _retryingServiceSettings = settings;
            _asyncManager = asyncManager;
        }

        public void Retry(string description, Action action)
        {
            Retry(_retryingServiceSettings.NumberOfRetryAttempts, description, action);
        }

        public T Retry<T>(string description, Func<T> func)
        {
            return Retry(_retryingServiceSettings.NumberOfRetryAttempts, description, func);
        }

        public T Retry<T>(int countOfAttemt, string description, Func<T> func)
        {
            T result = default(T);

            Retry(countOfAttemt, description, () => { result = func(); });

            return result;
        }

        public void Retry(int countOfAttemt, string description, Action action)
        {
            if (DatabaseTransactionScope.Current != null)
            {
                throw new Exception("Can't use retry inside transaction scope.");
            }

            int attemptNumber = 0;
            while (true)
            {
                try
                {
                    action();
                }
                catch (UserMessageException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    attemptNumber++;
                    Trace.TraceError(
                        "Attemt {0} of '{1}' was failed. Exception:{2}",
                        attemptNumber,
                        description,
                        ex);

                    if (ex is SqlException && ((SqlException)ex).Number == 1205)
                        _asyncManager.Sleep(_retryingServiceSettings.DelayBetweenRetriesInMilliseconds);

                    if (attemptNumber < countOfAttemt)
                    {
                        continue;
                    }
                    throw;
                }
                break;
            }
        }
    }
}
