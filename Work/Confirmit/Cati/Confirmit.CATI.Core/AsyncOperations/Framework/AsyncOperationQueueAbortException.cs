namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationQueueAbortException : AsyncOperationQueueException
    {
        public AsyncOperationQueueAbortException(string message) : base(message)
        {
        }
    }
}