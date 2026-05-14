using System;
using System.Collections.Generic;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Batch;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.CallManagement
{
    /*
    [TestClass]
    public class CallQueueServiceTest
    {
        class DummyBatchFactory : IBatchFactory<IDatabaseBatch>
        {
            public IDatabaseBatch CreateBatch()
            {
                return new DummyDatabaseBatch();
            }
        }

        class DummyDatabaseBatch : IDatabaseBatch
        {
            public int Size
            {
                get { return 0; }
            }

            public void Dispose()
            {
            }

            public int Id
            {
                get { return 0; }
            }

            public IEnumerable<IDatabaseBatch> GetSubBatches(int subBatchSize)
            {
                return new DummyDatabaseBatch[0];
            }
        }

        class DummyOperationProgressLOggerFactory : IOperationProgressLoggerFactory
        {
            public IOperationProgressLogger CreateOperationLogger(int operationId, IAsyncOperationRepository operationRepository)
            {
                return new DummyOperationProgressLogger();
            }
        }

        class DummyOperationProgressLogger : IOperationProgressLogger
        {
            public void LogProgressState(int percentage, string message) { }

            public void LogProgressState(int percentage, string message, OperationStatus status) { }

            public void LogInProgressState(int processedRecordsCount, int wholeRecordsCount) { }

            public void LogCompleteState(int successfullyProcessedRecordsCount, int failedRecordsCount, int wholeRecordsCount) { }

            public void LogFailState() { }
        }

        class DummyActivityEventFactory : IActivityEventFactory
        {
            public IActivityEvent CreateActivityEvent()
            {
                return new DummyActivityEvent();
            }
        }

        class DummyActivityEvent : IActivityEvent
        {
            public void Finish() { }

            public void Save() { }

            public bool IsRunning() { return true; }
        }

        class DummyAsyncOperationRepository : IAsyncOperationRepository
        {
            public int Insert(int status, int operationType, string supervisorName) { return 0; }

            public BvAsyncOperationsEntity GetById(int id) { return new BvAsyncOperationsEntity(); }

            public void Update(BvAsyncOperationsEntity entity) { }
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ActivateCalls_SurveyIdIsInvalid_ExceptionThrows()
        {
            var operation = CreateOperation(0, 1, CallStates.All, 0, 1, DateTime.UtcNow, false);
            operation.Execute();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ActivateCalls_ShiftTypeIdIsInvalid_ExceptionThrows()
        {
            var operation = CreateOperation(1, 1, CallStates.All, 0, -2, DateTime.UtcNow, false);
            operation.Execute();
        }

        private IOperation CreateOperation(int surveyId, int priority, CallStates callState, int personOrGroupId, int shiftType, DateTime timeToCall, bool enable)
        {
            return new ActivateCallsOperation(
                surveyId,
                priority,
                personOrGroupId,
                shiftType,
                timeToCall,
                callState,
                enable,
                string.Empty,
                1000,
                new SynchronousOperationExecutor(), 
                new DummyAsyncOperationRepository(), 
                new DummyOperationProgressLOggerFactory(),
                new DummyActivityEventFactory(),
                new DummyBatchFactory());
        }
    }
    */
}
