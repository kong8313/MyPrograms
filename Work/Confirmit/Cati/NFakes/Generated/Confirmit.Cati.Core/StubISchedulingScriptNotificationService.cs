using System;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubISchedulingScriptNotificationService : ISchedulingScriptNotificationService 
    {
        private ISchedulingScriptNotificationService _inner;

        public StubISchedulingScriptNotificationService()
        {
            _inner = null;
        }

        public ISchedulingScriptNotificationService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void LogExceptionISchedulingScriptNotificatorInt32Int32Int32Int32ExceptionDelegate(ISchedulingScriptNotificator notificator, int batchId, int surveyId, int scheduleId, int respId, Exception e);
        public LogExceptionISchedulingScriptNotificatorInt32Int32Int32Int32ExceptionDelegate LogExceptionISchedulingScriptNotificatorInt32Int32Int32Int32Exception;

        void ISchedulingScriptNotificationService.LogException(ISchedulingScriptNotificator notificator, int batchId, int surveyId, int scheduleId, int respId, Exception e)
        {

            if (LogExceptionISchedulingScriptNotificatorInt32Int32Int32Int32Exception != null)
            {
                LogExceptionISchedulingScriptNotificatorInt32Int32Int32Int32Exception(notificator, batchId, surveyId, scheduleId, respId, e);
            } else if (_inner != null)
            {
                ((ISchedulingScriptNotificationService)_inner).LogException(notificator, batchId, surveyId, scheduleId, respId, e);
            }
        }

        public delegate int GetSafeSurveyIdBvInterviewEntityDelegate(BvInterviewEntity interview);
        public GetSafeSurveyIdBvInterviewEntityDelegate GetSafeSurveyIdBvInterviewEntity;

        int ISchedulingScriptNotificationService.GetSafeSurveyId(BvInterviewEntity interview)
        {


            if (GetSafeSurveyIdBvInterviewEntity != null)
            {
                return GetSafeSurveyIdBvInterviewEntity(interview);
            } else if (_inner != null)
            {
                return ((ISchedulingScriptNotificationService)_inner).GetSafeSurveyId(interview);
            }

            return default(int);
        }

        public delegate int GetSafeRespIdBvInterviewEntityDelegate(BvInterviewEntity interview);
        public GetSafeRespIdBvInterviewEntityDelegate GetSafeRespIdBvInterviewEntity;

        int ISchedulingScriptNotificationService.GetSafeRespId(BvInterviewEntity interview)
        {


            if (GetSafeRespIdBvInterviewEntity != null)
            {
                return GetSafeRespIdBvInterviewEntity(interview);
            } else if (_inner != null)
            {
                return ((ISchedulingScriptNotificationService)_inner).GetSafeRespId(interview);
            }

            return default(int);
        }

        public delegate int GetSafeScheduleIdBvInterviewEntityDelegate(BvInterviewEntity interview);
        public GetSafeScheduleIdBvInterviewEntityDelegate GetSafeScheduleIdBvInterviewEntity;

        int ISchedulingScriptNotificationService.GetSafeScheduleId(BvInterviewEntity interview)
        {


            if (GetSafeScheduleIdBvInterviewEntity != null)
            {
                return GetSafeScheduleIdBvInterviewEntity(interview);
            } else if (_inner != null)
            {
                return ((ISchedulingScriptNotificationService)_inner).GetSafeScheduleId(interview);
            }

            return default(int);
        }

        public delegate int GetSafeScheduleIdInt32Delegate(int surveryId);
        public GetSafeScheduleIdInt32Delegate GetSafeScheduleIdInt32;

        int ISchedulingScriptNotificationService.GetSafeScheduleId(int surveryId)
        {


            if (GetSafeScheduleIdInt32 != null)
            {
                return GetSafeScheduleIdInt32(surveryId);
            } else if (_inner != null)
            {
                return ((ISchedulingScriptNotificationService)_inner).GetSafeScheduleId(surveryId);
            }

            return default(int);
        }

    }
}