using System;
using System.Xml.Serialization;

using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.Extensions;
using Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationQueue : IAsyncOperationQueue
    {
        private readonly IAsyncOperationRepository _repository;
        private readonly IAsyncOperationFactory _operationFactory;
        private readonly ITimeService _timeService;
        private readonly ISystemSettings _settings;
        private readonly IProcessAndEnvironmentInfo _environmentInfo;
        private readonly ICatiBackendNotificationPublisher _catiBackendNotificationPublisher;

        public AsyncOperationQueue(
            IAsyncOperationRepository repository,
            IAsyncOperationFactory operationFactory,
            ITimeService timeService,
            ISystemSettings settings,
            IProcessAndEnvironmentInfo environmentInfo, 
            ICatiBackendNotificationPublisher catiBackendNotificationPublisher)
        {
            _repository = repository;
            _operationFactory = operationFactory;
            _timeService = timeService;
            _settings = settings;
            _environmentInfo = environmentInfo;
            _catiBackendNotificationPublisher = catiBackendNotificationPublisher;
        }

        private string SerializeParameters(object parameters)
        {
            var serializer = new XmlSerializer(parameters.GetType());

            var serializedOperationParameters = serializer.SerializeToString(parameters);

            return serializedOperationParameters;
        }

        public BvAsyncOperationQueueEntity Enqueue(
            int callCenterId,
            string title,
            bool isInitiatedBySystem,
            IAsyncOperationParameters parameters,
            int priority,
            string supervisorName)
        {
            var serializedOperationParameters = SerializeParameters(parameters);

            var type = _operationFactory.GetOperationDescriptorFromOperationParametersType(parameters.GetType()).OperationTypeId;
            
            var entity = new BvAsyncOperationQueueEntity
            {
                CallCenterId = callCenterId,
                Title = title,
                IsInitiatedBySystem = isInitiatedBySystem,
                Type = (byte)type,
                Parameters = serializedOperationParameters,
                SurveySid = parameters.SurveyId,
                Priority = priority,
                CreatedBySupervisorName = supervisorName,
                QueuedDate = _timeService.GetUtcNow(),
                State = (byte)AsyncOperationState.Queued
            };

            _repository.Insert(entity);

            return entity;
        }

        public void UpdateHanged()
        {
            using (var connection = new ConnectionScope())
            {
                using (var dbLock = AsyncOperationLock.CreateLock("UpdateHanged"))
                {
                    if (dbLock.TryEnterLock())
                    {
                        BvSpAsyncOperationQueue_UpdateHangedAdapter.ExecuteNonQuery(
                            (int)AsyncOperationState.Executing,
                            (int)AsyncOperationState.Hanged,
                            _settings.AsyncOperations.TimeToTreatOperationHangedInMinutes);
                        
                        BvSpAsyncOperationQueue_UpdateHangedAdapter.ExecuteNonQuery(
                            (int)AsyncOperationState.Cancelling,
                            (int)AsyncOperationState.Hanged,
                            _settings.AsyncOperations.TimeToTreatOperationHangedInMinutes);
                    }
                }
            }
        }

        public BvAsyncOperationQueueEntity Dequeue()
        {
            var evt = new AsyncOperationDequeueEvent();

            BvAsyncOperationQueueEntity entity = null;

            try
            {
                using (var connection = new ConnectionScope())
                {
                    evt.Details.AddTiming("new ConnectionScope()");

                    using (var dbLock = AsyncOperationLock.CreateLock("Dequeue"))
                    {
                        evt.Details.AddTiming("CreateLock");

                        if (dbLock.TryEnterLock())
                        {
                            evt.Details.AddTiming("TryEnterLock");

                            var operationIdEntity = BvSpAsyncOperationQueue_DequeueAdapter.ExecuteEntity(
                                _settings.AsyncOperations.MaximumRunningAsyncOperations,
                                (int)AsyncOperationState.Queued,
                                (int)AsyncOperationState.Executing);

                            evt.Details.AddTiming("BvSpAsyncOperationQueue_Dequeue");

                            if (operationIdEntity != null && operationIdEntity.Id.HasValue)
                            {
                                entity = _repository.Get(operationIdEntity.Id.Value);

                                evt.Details.AddTiming("_repository.Get");

                                entity.State = (byte)AsyncOperationState.Executing;
                                entity.StartedDate = _timeService.GetUtcNow();
                                entity.HeartBeat = entity.StartedDate;
                                entity.TotalItemsCount = 0;
                                entity.ProcessedItemsCount = 0;
                                entity.FailedItemsCount = 0;
                                entity.Server = _environmentInfo.MachineName;
                                entity.Text = "";
                                entity.Error = "";

                                _repository.Update(entity);

                                evt.Details.AsyncOperationEntity = entity;

                                evt.Details.AddTiming("_repository.Update");
                            }
                        }
                        else
                        {
                            evt.Details.AddTiming("!dbLock.TryEnterLock()");
                        }
                    } // using (var dbLock = AsyncOperationLock.CreateLock("AsyncOperationQueue.Dequeue"))
                    evt.Details.AddTiming("dbLock.Dispose()");

                } // using (var connection = new ConnectionScope())
                evt.Details.AddTiming("ConnectionScope.Dispose()");
            }
            catch (Exception)
            {
                evt.Details.AddTiming("catch(Exception)");

                evt.Save();

                throw;
            }

            // Needed to investigate bug only...
            if (entity != null || evt.Details.GetElapsed() > TimeSpan.FromMilliseconds(500))
            {
                evt.Save();
            }
            else
            {
                evt.SaveMetric();
            }

            return entity;
        }

        public void Abort(int id, string supervisorName)
        {
            var evt = new AsyncOperationAbortEvent();

            using (var connection = new ConnectionScope())
            {
                evt.Details.AddTiming("new ConnectionScope()");

                using (var dbLock = AsyncOperationLock.CreateLock("Abort"))
                {
                    if (dbLock.TryEnterLock())
                    {
                        evt.Details.AddTiming("TryEnterLock");

                        var entity = _repository.Get(id);

                        evt.Details.AddTiming("_repository.Get");

                        evt.Details.AsyncOperationEntity = entity;
                        
                        var state = (AsyncOperationState)entity.State;
                        if (state == AsyncOperationState.Queued || state == AsyncOperationState.Executing)
                        {
                            if (state == AsyncOperationState.Executing)
                            {
                                _catiBackendNotificationPublisher.PublishAsyncOperationCancelled(entity.Id);
                                evt.Details.AddTiming("cancelling executing operation");
                            }
                            
                            entity.AbortedBySupervisorName = supervisorName;
                            entity.State = state == AsyncOperationState.Queued ? (byte)AsyncOperationState.Aborted : (byte)AsyncOperationState.Cancelling;
                            _repository.Update(entity);
                            evt.Details.AddTiming("_repository.Update");
                        }
                        else
                        {
                            throw new AsyncOperationQueueAbortException(
                                string.Format(
                                    "Async operation cannot be aborted because it state is not 'Queued' or 'Executing' but '{0}'",
                                    ((AsyncOperationState)entity.State).ToString()));
                        }
                    }
                    else
                    {
                        throw new AsyncOperationQueueAbortException(
                            "Async operation cannot be aborted because because of timeout. Try again later.");
                    }

                } // using (var dbLock = AsyncOperationLock.CreateLock("AsyncOperationQueue.Abort"))
                evt.Details.AddTiming("dbLock.Dispose()");

            } // using (var connection = new ConnectionScope())
            evt.Details.AddTiming("ConnectionScope.Dispose()");

            evt.Save();
        }
    }
}