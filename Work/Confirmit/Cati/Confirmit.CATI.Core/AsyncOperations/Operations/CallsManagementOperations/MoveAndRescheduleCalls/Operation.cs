using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.MoveAndRescheduleCalls
{
    public class Operation : ICallsManagementOperation
    {
        private readonly ISystemSettings _systemSettings;
        private readonly ICallsManagementOperationBase _OperationBase;
        private readonly IInterviewService _interviewService;
        private readonly ICallQueueService _callQueueService;

        public Operation(
            ISystemSettings systemSettings,
            ICallsManagementOperationBase operationBase,
            IInterviewService interviewService,
            ICallQueueService callQueueService)
        {
            _systemSettings = systemSettings;
            _OperationBase = operationBase;
            _interviewService = interviewService;
            _callQueueService = callQueueService;
        }

        public IOperationDescriptor Descriptor
        {
            get { return new Descriptor(); }
        }

        private Parameters DeserializeParameters(string parameters)
        {
            var serializer = new XmlSerializer(typeof(Parameters));

            using (var reader = new StringReader(parameters))
            {
                return (Parameters)serializer.Deserialize(reader);
            }
        }

        private BaseAsyncOperationManagementActivityEvent<Parameters> CreateEvent(BvAsyncOperationQueueEntity entity, Parameters parameters)
        {
            var surveyName = SurveyRepository.GetById(parameters.SurveyId).Name;
            switch (parameters.BatchParameters.Type)
            {
                case BatchType.Selected:
                    return new MoveAndRescheduleSelectedCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                case BatchType.Filtered:
                    return new MoveAndRescheduleFilteredCallsEvent(parameters.SurveyId, surveyName, parameters, entity);
                default:
                    throw new NotImplementedException(String.Format("Activity event doesn't specified for Move calls operation with {0} batch type.", parameters.BatchParameters.Type));
            }
        }

        public AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity, string serializedParameters, IAsyncOperationProgressLogger progressLogger, CancellationToken cancellationToken)
        {
            var parameters = DeserializeParameters(serializedParameters);

            var evt = CreateEvent(entity, parameters);

            var maxItem = parameters.BatchParameters.Type == BatchType.Selected ? Int32.MaxValue : SurveyService.LimitCallsForMoveAndRescheduleAction;

            var result = _OperationBase.Execute(
                this,
                parameters.BatchParameters,
                progressLogger,
                entity,
                parameters.SurveyId,
                _systemSettings.AsyncOperation.ActivatePortionSize,
                maxItem,
                "respondents",
                parameters, cancellationToken);

            if (evt.Details != null && result != null)
            {
                evt.Details.Result = result.ToString();
            }
            
            evt.Save();

            return result;
        }

        public void ProcessItem(ICallsManagementOperationBase operation, int interviewId, object state, BvAsyncOperationQueueEntity entity)
        {
            var parameters = (Parameters) state;
            var survey = SurveyRepository.GetById(parameters.SurveyId);
            var priority =
                (short) StateRepository.GetByItsAndStateGroupId(parameters.StateId, survey.StateGroupID).Priority;

            using (var transaction = new DatabaseTransactionScope("MoveAndRescheduleCalls", DeadlockPriority.Supervisor))
            {
                bool isCallLocked;
                var call = _callQueueService.GetCallWithTryLock(parameters.SurveyId, interviewId, out isCallLocked);

                if (call != null)
                {
                    if (call.CallState < 0 || !isCallLocked)
                    {
                        return;
                    }
                    else if (call.CallState == (int) CallState.DisabledByFCD)
                    {
                        // returns call to queue, because we has locked it there
                        CallQueueService.AddCall(call, 0, parameters.StateId);
                        return;
                    }

                    call.Priority = priority;
                }

                var interview = InterviewRepository.GetByIdWithCheck(parameters.SurveyId, interviewId);

                interview.TransientState = parameters.StateId;

                if (parameters.AppointmentPrm != null)
                {
                    _interviewService.AddAppointments(interview.SurveySID, interview.ID, 0, new Appointment[] { parameters.AppointmentPrm }, true);
                }

                var options = new SchedulingScriptExecutionOptions()
                                  {
                                      ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled,
                                      BatchID = 0,
                                      CallProvider = new CallMemoryProvider(call),
                                      IsLogToHistory = false,
                                      opType = OperationType.MovedAndReschedule,
                                      CallCenterID = entity.CallCenterId
                                     
                                  };

                InterviewRepository.Update(interview, options);
                transaction.Commit();
            }
        }
     }
}
