using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.RoutineMaintenance
{
    public class AsyncOperationRoutineMaintenanceLogger : IRoutineMaintenanceLogger
    {
        private readonly IAsyncOperationProgressLogger _asyncOperationProgressLogger;
        private readonly int _asyncOperationId;

        public AsyncOperationRoutineMaintenanceLogger(IAsyncOperationProgressLogger asyncOperationProgressLogger, int asyncOperationId)
        {
            _asyncOperationProgressLogger = asyncOperationProgressLogger;
            _asyncOperationId = asyncOperationId;
        }

        public void AppendText(string text, TimeSpan elapsed, bool isNewLine)
        {
            _asyncOperationProgressLogger.AppendText(_asyncOperationId, text, elapsed, isNewLine);
        }

        public void UpdateProgress(int total, int successful, int failed)
        {
            _asyncOperationProgressLogger.UpdateProgress(_asyncOperationId, total, successful, failed);
        }
    }
}