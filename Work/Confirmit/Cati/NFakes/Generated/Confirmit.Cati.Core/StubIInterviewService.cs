using System;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Threading;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIInterviewService : IInterviewService 
    {
        private IInterviewService _inner;

        public StubIInterviewService()
        {
            _inner = null;
        }

        public IInterviewService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AddAppointmentsInt32Int32Int32ArrayOfAppointmentBooleanDelegate(int surveySid, int interviewId, int batchId, Appointment[] appointments, bool allowOutsideShift);
        public AddAppointmentsInt32Int32Int32ArrayOfAppointmentBooleanDelegate AddAppointmentsInt32Int32Int32ArrayOfAppointmentBoolean;

        void IInterviewService.AddAppointments(int surveySid, int interviewId, int batchId, Appointment[] appointments, bool allowOutsideShift)
        {

            if (AddAppointmentsInt32Int32Int32ArrayOfAppointmentBoolean != null)
            {
                AddAppointmentsInt32Int32Int32ArrayOfAppointmentBoolean(surveySid, interviewId, batchId, appointments, allowOutsideShift);
            } else if (_inner != null)
            {
                ((IInterviewService)_inner).AddAppointments(surveySid, interviewId, batchId, appointments, allowOutsideShift);
            }
        }

        public delegate void DeleteRespondentsInt32ArrayOfInt32CancellationTokenDelegate(int surveySID, int[] respondentIDs, CancellationToken cancellationToken);
        public DeleteRespondentsInt32ArrayOfInt32CancellationTokenDelegate DeleteRespondentsInt32ArrayOfInt32CancellationToken;

        void IInterviewService.DeleteRespondents(int surveySID, int[] respondentIDs, CancellationToken cancellationToken)
        {

            if (DeleteRespondentsInt32ArrayOfInt32CancellationToken != null)
            {
                DeleteRespondentsInt32ArrayOfInt32CancellationToken(surveySID, respondentIDs, cancellationToken);
            } else if (_inner != null)
            {
                ((IInterviewService)_inner).DeleteRespondents(surveySID, respondentIDs, cancellationToken);
            }
        }

        public delegate BvInterviewWithOriginEntity AddRespondentBvSurveyEntityInt32Int32OperationTypeRoleNullableOfInt32Delegate(BvSurveyEntity survey, int respondentId, int its, OperationType operationType, Role role, int? personSid);
        public AddRespondentBvSurveyEntityInt32Int32OperationTypeRoleNullableOfInt32Delegate AddRespondentBvSurveyEntityInt32Int32OperationTypeRoleNullableOfInt32;

        BvInterviewWithOriginEntity IInterviewService.AddRespondent(BvSurveyEntity survey, int respondentId, int its, OperationType operationType, Role role, int? personSid)
        {


            if (AddRespondentBvSurveyEntityInt32Int32OperationTypeRoleNullableOfInt32 != null)
            {
                return AddRespondentBvSurveyEntityInt32Int32OperationTypeRoleNullableOfInt32(survey, respondentId, its, operationType, role, personSid);
            } else if (_inner != null)
            {
                return ((IInterviewService)_inner).AddRespondent(survey, respondentId, its, operationType, role, personSid);
            }

            return default(BvInterviewWithOriginEntity);
        }

        public delegate BvInterviewWithOriginEntity AddRespondentBvSurveyEntityInt32SchedulingScriptExecutionOptionsDelegate(BvSurveyEntity survey, int respondentId, SchedulingScriptExecutionOptions options);
        public AddRespondentBvSurveyEntityInt32SchedulingScriptExecutionOptionsDelegate AddRespondentBvSurveyEntityInt32SchedulingScriptExecutionOptions;

        BvInterviewWithOriginEntity IInterviewService.AddRespondent(BvSurveyEntity survey, int respondentId, SchedulingScriptExecutionOptions options)
        {


            if (AddRespondentBvSurveyEntityInt32SchedulingScriptExecutionOptions != null)
            {
                return AddRespondentBvSurveyEntityInt32SchedulingScriptExecutionOptions(survey, respondentId, options);
            } else if (_inner != null)
            {
                return ((IInterviewService)_inner).AddRespondent(survey, respondentId, options);
            }

            return default(BvInterviewWithOriginEntity);
        }

        public delegate void BindDialerIdToInterviewInt32Int32Int32Delegate(int surveyId, int interviewId, int dialerId);
        public BindDialerIdToInterviewInt32Int32Int32Delegate BindDialerIdToInterviewInt32Int32Int32;

        void IInterviewService.BindDialerIdToInterview(int surveyId, int interviewId, int dialerId)
        {

            if (BindDialerIdToInterviewInt32Int32Int32 != null)
            {
                BindDialerIdToInterviewInt32Int32Int32(surveyId, interviewId, dialerId);
            } else if (_inner != null)
            {
                ((IInterviewService)_inner).BindDialerIdToInterview(surveyId, interviewId, dialerId);
            }
        }

        public delegate void BindDialerIdToInterviewBvInterviewEntityInt32Delegate(BvInterviewEntity interview, int dialerId);
        public BindDialerIdToInterviewBvInterviewEntityInt32Delegate BindDialerIdToInterviewBvInterviewEntityInt32;

        void IInterviewService.BindDialerIdToInterview(BvInterviewEntity interview, int dialerId)
        {

            if (BindDialerIdToInterviewBvInterviewEntityInt32 != null)
            {
                BindDialerIdToInterviewBvInterviewEntityInt32(interview, dialerId);
            } else if (_inner != null)
            {
                ((IInterviewService)_inner).BindDialerIdToInterview(interview, dialerId);
            }
        }

        public delegate string GenereteSecurityKeyBvInterviewEntityDelegate(BvInterviewEntity interview);
        public GenereteSecurityKeyBvInterviewEntityDelegate GenereteSecurityKeyBvInterviewEntity;

        string IInterviewService.GenereteSecurityKey(BvInterviewEntity interview)
        {


            if (GenereteSecurityKeyBvInterviewEntity != null)
            {
                return GenereteSecurityKeyBvInterviewEntity(interview);
            } else if (_inner != null)
            {
                return ((IInterviewService)_inner).GenereteSecurityKey(interview);
            }

            return default(string);
        }

        public delegate int GetInterviewTimezoneOrDefaultBvInterviewEntityDelegate(BvInterviewEntity interview);
        public GetInterviewTimezoneOrDefaultBvInterviewEntityDelegate GetInterviewTimezoneOrDefaultBvInterviewEntity;

        int IInterviewService.GetInterviewTimezoneOrDefault(BvInterviewEntity interview)
        {


            if (GetInterviewTimezoneOrDefaultBvInterviewEntity != null)
            {
                return GetInterviewTimezoneOrDefaultBvInterviewEntity(interview);
            } else if (_inner != null)
            {
                return ((IInterviewService)_inner).GetInterviewTimezoneOrDefault(interview);
            }

            return default(int);
        }

        public delegate int GetInterviewTimezoneOrDefaultInt32Int32Delegate(int surveySid, int interviewId);
        public GetInterviewTimezoneOrDefaultInt32Int32Delegate GetInterviewTimezoneOrDefaultInt32Int32;

        int IInterviewService.GetInterviewTimezoneOrDefault(int surveySid, int interviewId)
        {


            if (GetInterviewTimezoneOrDefaultInt32Int32 != null)
            {
                return GetInterviewTimezoneOrDefaultInt32Int32(surveySid, interviewId);
            } else if (_inner != null)
            {
                return ((IInterviewService)_inner).GetInterviewTimezoneOrDefault(surveySid, interviewId);
            }

            return default(int);
        }

        public delegate int[] GetInterviewIdsWithoutRespondentsInt32Delegate(int surveyId);
        public GetInterviewIdsWithoutRespondentsInt32Delegate GetInterviewIdsWithoutRespondentsInt32;

        int[] IInterviewService.GetInterviewIdsWithoutRespondents(int surveyId)
        {


            if (GetInterviewIdsWithoutRespondentsInt32 != null)
            {
                return GetInterviewIdsWithoutRespondentsInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IInterviewService)_inner).GetInterviewIdsWithoutRespondents(surveyId);
            }

            return default(int[]);
        }

    }
}