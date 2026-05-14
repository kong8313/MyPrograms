using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIAsyncOperationRetry : IAsyncOperationRetry 
    {
        private IAsyncOperationRetry _inner;

        public StubIAsyncOperationRetry()
        {
            _inner = null;
        }

        public IAsyncOperationRetry Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExecuteActionActionDelegate(Action action);
        public ExecuteActionActionDelegate ExecuteActionAction;

        void IAsyncOperationRetry.ExecuteAction(Action action)
        {

            if (ExecuteActionAction != null)
            {
                ExecuteActionAction(action);
            } else if (_inner != null)
            {
                ((IAsyncOperationRetry)_inner).ExecuteAction(action);
            }
        }

    }
}