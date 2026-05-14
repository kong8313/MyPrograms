using System;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationQueueException : Exception
    {
        public AsyncOperationQueueException(string message) : base(message)
        {
        }
    }
}