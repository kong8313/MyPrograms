using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Misc
{
    public class PiecemealBatchExecutionResult
    {
        public PiecemealBatchExecutionResult(PiecemealExecutionState state, int processedRecordsNumber, int failedRecordsNumber, IEnumerable<Exception> errors)
        {
            ExecutionState = state;
            SuccessfullyProcessedRecordsCount = processedRecordsNumber;
            FailedRecordsCount = failedRecordsNumber;
            Errors = errors;
        }

        public PiecemealExecutionState ExecutionState { get; private set; }

        public int SuccessfullyProcessedRecordsCount { get; private set; }

        public int FailedRecordsCount { get; private set; }

        public IEnumerable<Exception> Errors { get; private set; }
    }
}
