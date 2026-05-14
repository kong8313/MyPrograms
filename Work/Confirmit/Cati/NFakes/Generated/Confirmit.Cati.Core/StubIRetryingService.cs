using System;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIRetryingService : IRetryingService 
    {
        private IRetryingService _inner;

        public StubIRetryingService()
        {
            _inner = null;
        }

        public IRetryingService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void RetryStringActionDelegate(string description, Action action);
        public RetryStringActionDelegate RetryStringAction;

        void IRetryingService.Retry(string description, Action action)
        {

            if (RetryStringAction != null)
            {
                RetryStringAction(description, action);
            } else if (_inner != null)
            {
                ((IRetryingService)_inner).Retry(description, action);
            }
        }

        T IRetryingService.Retry<T>(string description, Func<T> action)
        {


            return default(T);
        }

        public delegate void RetryInt32StringActionDelegate(int countOfAttemt, string description, Action action);
        public RetryInt32StringActionDelegate RetryInt32StringAction;

        void IRetryingService.Retry(int countOfAttemt, string description, Action action)
        {

            if (RetryInt32StringAction != null)
            {
                RetryInt32StringAction(countOfAttemt, description, action);
            } else if (_inner != null)
            {
                ((IRetryingService)_inner).Retry(countOfAttemt, description, action);
            }
        }

        T IRetryingService.Retry<T>(int countOfAttemt, string description, Func<T> action)
        {


            return default(T);
        }

    }
}