using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.Interfaces;
using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services
{
    public class SchedulingScriptLogger : ISchedulingScriptLogger
    {
        private readonly IScheduleErrorRepository _scheduleErrorRepository;
        private readonly ISchedulingScriptSettings _schedulingScriptSettings;

        public SchedulingScriptLogger(IScheduleErrorRepository scheduleErrorRepository, ISchedulingScriptSettings schedulingScriptSettings)
        {
            _scheduleErrorRepository = scheduleErrorRepository;
            _schedulingScriptSettings = schedulingScriptSettings;
        }

        public void LogError(Exception exception, int interviewId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason = SchedulingScriptExecutionReason.Unspecified, string currentITS = "", bool notificationSent = false)
        {
            DeleteOldErrorsIfNeeded(scheduleId);

            var entity = GetErrorEntity(exception, interviewId, surveyId, scheduleId, executionReason, currentITS, notificationSent);

            _scheduleErrorRepository.Insert(entity);
        }


        private BvScheduleErrorEntity GetErrorEntity(Exception exception, int interviewId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason = SchedulingScriptExecutionReason.Unspecified, string currentITS = "", bool notificationSent = false)
        {
            BvScheduleErrorEntity entity = new BvScheduleErrorEntity()
            {
                ScheduleID = scheduleId,
                SurveySid = surveyId,
                InterviewId = interviewId,
                Timestamp = DateTime.UtcNow,
                TriggeredBy = SchedulingScriptExecutionReasonConverter.ConvertToString(executionReason),
                ExtendedStatus = currentITS,
                RuleNumber = "",
                Action = "",
                Message = exception.Message,
                NotificationSent = notificationSent
            };

            if (exception is SchedulingExecutionException exceptionWithContext)
            {
                entity.RuleNumber = $"{exceptionWithContext.RuleNumber + 1}.{exceptionWithContext.SubRuleNumber + 1}";
                entity.Action = $"{exceptionWithContext.ActionNumber + 1}";
            }

            return entity;
        }

        private void DeleteOldErrorsIfNeeded(int scheduleId)
        {
            var errorsCount = _scheduleErrorRepository.GetErrorsCountByScheduleID(scheduleId);
            var errorLogSize = _schedulingScriptSettings.ErrorLogSize;

            if (errorsCount >= errorLogSize)
            {
                var errorsToDelete = errorsCount - errorLogSize + 1;
                var lastErrorToDelete = _scheduleErrorRepository.GetByRowNumber(errorsToDelete, scheduleId);

                _scheduleErrorRepository.DeleteOldErrors(lastErrorToDelete, scheduleId);
            }
        }
    }
}
