using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.PerformanceCounters;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationExecutor : IAsyncOperationExecutor
    {
        private readonly IAsyncOperationRepository _repository;
        private readonly IAsyncOperationProgressLogger _progressLogger;
        private readonly IAsyncOperationFactory _operationsFactory;
        private readonly IAsyncOperationRetry _operationRetry;
        private readonly IAsyncOperationQueue _queue;
        private readonly IPerformanceCountersContainer _performanceCountersContainer;
        private readonly Dictionary<int, Task<AsyncOperationResult>> _operationId2OperationTask;
        private readonly object _runningOperationsLock;
        private readonly IAsyncManager _asyncManager;
        private readonly ThreadIdentityService _threadIdentityService = new ThreadIdentityService();
        private readonly AsyncOperationCancellationService _cancellationService;

        public AsyncOperationExecutor(
            IAsyncManager asyncManager,
            IAsyncOperationQueue queue,
            IAsyncOperationRepository repository,
            IAsyncOperationProgressLogger progressLogger,
            IAsyncOperationFactory operationsFactory,
            IAsyncOperationRetry operationRetry,
            IPerformanceCountersContainer performanceCountersContainer, 
            AsyncOperationCancellationService cancellationService)
        {
            _queue = queue;
            _repository = repository;
            _progressLogger = progressLogger;
            _operationsFactory = operationsFactory;
            _operationRetry = operationRetry;
            _performanceCountersContainer = performanceCountersContainer;
            _cancellationService = cancellationService;
            _operationId2OperationTask = new Dictionary<int, Task<AsyncOperationResult>>();
            _runningOperationsLock = new object();
            _asyncManager = asyncManager;
        }

        public bool DequeueAndExecute()
        {
            var entity = _queue.Dequeue();

            if (entity != null)
            {
                ExecuteOperationAsync(entity);
            }

            return entity != null;
        }

        public Task<AsyncOperationResult> ExecuteOperationAsync(BvAsyncOperationQueueEntity entity)
        {
            var task = _asyncManager.CreateTask(() =>
            {
                AsyncOperationResult result = null;
                try
                {
                    result = ExecuteOperationSync(entity);

                }
                catch (Exception e)
                {
                    TraceHelper.TraceException(e);
                }

                return result;
            });

            lock (_runningOperationsLock)
            {
                _operationId2OperationTask.Add(entity.Id, task);
            }

            _asyncManager.StartTask(task);

            return task;
        }

        public void WaitForAllRunningOperationsToComplete()
        {
            List<Task<AsyncOperationResult>> tasksToWait;
            lock (_runningOperationsLock)
            {
                tasksToWait = new List<Task<AsyncOperationResult>>(_operationId2OperationTask.Values);
            }

            Task.WaitAll(tasksToWait.ToArray());
        }

        public IEnumerable<int> GetExecutingOperationIds()
        {
            lock (_runningOperationsLock)
            {
                var operationIds = new List<int>(_operationId2OperationTask.Keys);
                return operationIds;
            }
        }

        public AsyncOperationResult ExecuteOperationSync(
            BvAsyncOperationQueueEntity entity)
        {
            AsyncOperationResult result;

            try
            {
                _threadIdentityService.SetPrincipal(entity.CreatedBySupervisorName);
                _performanceCountersContainer.AsyncOperationsCount.Increment();

                var operationInstance = _operationsFactory.CreateOperationFromType((OperationTypes)entity.Type);
                
                var cancellationToken = _cancellationService.InitializeOperation(entity.Id);
                
                result = operationInstance.Execute(entity, entity.Parameters, _progressLogger, cancellationToken);

                _operationRetry.ExecuteAction(() =>
                {
                    var completedOperation = _repository.Get(entity.Id);

                    completedOperation.State = (byte)result.State;
                    completedOperation.FinishedDate = DateTime.UtcNow;
                    completedOperation.ProcessedItemsCount = result.ProcessedItemsCount;
                    completedOperation.FailedItemsCount = result.FailedItemsCount;
                    completedOperation.AbortedBySupervisorName = null;
                    
                    completedOperation.Error =
                        string.Join(Environment.NewLine,
                            result.Warnings.Select(x => "Warning! " + x).Union(
                                result.Errors.Select(x => "Error! " + x)));

                    _repository.Update(completedOperation);
                });

            }
            catch (OperationCanceledException)
            {
                _operationRetry.ExecuteAction(() =>
                {
                    var cancelledOperation = _repository.Get(entity.Id);

                    cancelledOperation.State = (byte)AsyncOperationState.PartiallyCompleted;
                    cancelledOperation.FinishedDate = DateTime.UtcNow;
                    
                    _repository.Update(cancelledOperation);
                });

                result = new AsyncOperationResult {
                    State = AsyncOperationState.PartiallyCompleted
                };
            }
            catch (Exception e)
            {
                _operationRetry.ExecuteAction(() =>
                {
                    var failedOperation = _repository.Get(entity.Id);

                    failedOperation.State = (byte)AsyncOperationState.Failed;
                    failedOperation.FinishedDate = DateTime.UtcNow;
                    failedOperation.Error = e.ToString();

                    _repository.Update(failedOperation);
                });

                result = new AsyncOperationResult {
                    State = AsyncOperationState.Failed,
                    Errors = new List<Exception> { e }
                };

                TraceHelper.TraceException(
                    e,
                    string.Format(
                        "Asynchronous operation '{0}' failed.",
                        entity.Title));
            }
            finally
            {
                _performanceCountersContainer.AsyncOperationsCount.Decrement();
                _threadIdentityService.ResetPrincipal();
                _cancellationService.DisposeOperation(entity.Id);
            }

            // Remove ourself from the dictionary
            try
            {
                lock (_runningOperationsLock)
                {
                    _operationId2OperationTask.Remove(entity.Id);
                }
            }
            catch (Exception e)
            {
                TraceHelper.TraceException(e);
            }

            return result;
        }
    }
}