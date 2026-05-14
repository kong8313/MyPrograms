using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIAsyncOperationProgressLogger : IAsyncOperationProgressLogger 
    {
        private IAsyncOperationProgressLogger _inner;

        public StubIAsyncOperationProgressLogger()
        {
            _inner = null;
        }

        public IAsyncOperationProgressLogger Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AppendTextInt32StringTimeSpanBooleanDelegate(int operationId, string textToAppend, TimeSpan elapsedTime, bool isNewLine);
        public AppendTextInt32StringTimeSpanBooleanDelegate AppendTextInt32StringTimeSpanBoolean;

        void IAsyncOperationProgressLogger.AppendText(int operationId, string textToAppend, TimeSpan elapsedTime, bool isNewLine)
        {

            if (AppendTextInt32StringTimeSpanBoolean != null)
            {
                AppendTextInt32StringTimeSpanBoolean(operationId, textToAppend, elapsedTime, isNewLine);
            } else if (_inner != null)
            {
                ((IAsyncOperationProgressLogger)_inner).AppendText(operationId, textToAppend, elapsedTime, isNewLine);
            }
        }

        public delegate void UpdateProgressInt32Int32Int32Int32Delegate(int operationId, int totalItemsCount, int succeededItemsCount, int failedItemsCount);
        public UpdateProgressInt32Int32Int32Int32Delegate UpdateProgressInt32Int32Int32Int32;

        void IAsyncOperationProgressLogger.UpdateProgress(int operationId, int totalItemsCount, int succeededItemsCount, int failedItemsCount)
        {

            if (UpdateProgressInt32Int32Int32Int32 != null)
            {
                UpdateProgressInt32Int32Int32Int32(operationId, totalItemsCount, succeededItemsCount, failedItemsCount);
            } else if (_inner != null)
            {
                ((IAsyncOperationProgressLogger)_inner).UpdateProgress(operationId, totalItemsCount, succeededItemsCount, failedItemsCount);
            }
        }

        public delegate void UpdateHeartBeatInt32Delegate(int operationId);
        public UpdateHeartBeatInt32Delegate UpdateHeartBeatInt32;

        void IAsyncOperationProgressLogger.UpdateHeartBeat(int operationId)
        {

            if (UpdateHeartBeatInt32 != null)
            {
                UpdateHeartBeatInt32(operationId);
            } else if (_inner != null)
            {
                ((IAsyncOperationProgressLogger)_inner).UpdateHeartBeat(operationId);
            }
        }

    }
}