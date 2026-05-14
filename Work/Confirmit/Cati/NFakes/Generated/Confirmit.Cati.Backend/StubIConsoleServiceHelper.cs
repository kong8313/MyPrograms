using System;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService.Fakes
{
    public class StubIConsoleServiceHelper : IConsoleServiceHelper 
    {
        private IConsoleServiceHelper _inner;

        public StubIConsoleServiceHelper()
        {
            _inner = null;
        }

        public IConsoleServiceHelper Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Timezone GetTimeZoneInt32Delegate(int timezoneId);
        public GetTimeZoneInt32Delegate GetTimeZoneInt32;

        Timezone IConsoleServiceHelper.GetTimeZone(int timezoneId)
        {


            if (GetTimeZoneInt32 != null)
            {
                return GetTimeZoneInt32(timezoneId);
            } else if (_inner != null)
            {
                return ((IConsoleServiceHelper)_inner).GetTimeZone(timezoneId);
            }

            return default(Timezone);
        }

        public delegate void LogoutProcessInt32StringLoginStateBooleanStringInt32Delegate(int personId, string company, LoginState loggedInToDialerState, bool isLoginRcToDialer, string projectId, int dialerId);
        public LogoutProcessInt32StringLoginStateBooleanStringInt32Delegate LogoutProcessInt32StringLoginStateBooleanStringInt32;

        void IConsoleServiceHelper.LogoutProcess(int personId, string company, LoginState loggedInToDialerState, bool isLoginRcToDialer, string projectId, int dialerId)
        {

            if (LogoutProcessInt32StringLoginStateBooleanStringInt32 != null)
            {
                LogoutProcessInt32StringLoginStateBooleanStringInt32(personId, company, loggedInToDialerState, isLoginRcToDialer, projectId, dialerId);
            } else if (_inner != null)
            {
                ((IConsoleServiceHelper)_inner).LogoutProcess(personId, company, loggedInToDialerState, isLoginRcToDialer, projectId, dialerId);
            }
        }

        public delegate bool SetPendingBreakStatusBvTasksEntityBvPersonEntityPendingBreakStatusNullableOfInt32Delegate(BvTasksEntity task, BvPersonEntity person, PendingBreakStatus status, int? breakTypeId);
        public SetPendingBreakStatusBvTasksEntityBvPersonEntityPendingBreakStatusNullableOfInt32Delegate SetPendingBreakStatusBvTasksEntityBvPersonEntityPendingBreakStatusNullableOfInt32;

        bool IConsoleServiceHelper.SetPendingBreakStatus(BvTasksEntity task, BvPersonEntity person, PendingBreakStatus status, int? breakTypeId)
        {


            if (SetPendingBreakStatusBvTasksEntityBvPersonEntityPendingBreakStatusNullableOfInt32 != null)
            {
                return SetPendingBreakStatusBvTasksEntityBvPersonEntityPendingBreakStatusNullableOfInt32(task, person, status, breakTypeId);
            } else if (_inner != null)
            {
                return ((IConsoleServiceHelper)_inner).SetPendingBreakStatus(task, person, status, breakTypeId);
            }

            return default(bool);
        }

        public delegate void ContinueWorkAfterBreakBvTasksEntityInt32Delegate(BvTasksEntity task, int attemptNumber);
        public ContinueWorkAfterBreakBvTasksEntityInt32Delegate ContinueWorkAfterBreakBvTasksEntityInt32;

        void IConsoleServiceHelper.ContinueWorkAfterBreak(BvTasksEntity task, int attemptNumber)
        {

            if (ContinueWorkAfterBreakBvTasksEntityInt32 != null)
            {
                ContinueWorkAfterBreakBvTasksEntityInt32(task, attemptNumber);
            } else if (_inner != null)
            {
                ((IConsoleServiceHelper)_inner).ContinueWorkAfterBreak(task, attemptNumber);
            }
        }

        public delegate void SwitchSurveyIfNeededBvTasksEntityDelegate(BvTasksEntity task);
        public SwitchSurveyIfNeededBvTasksEntityDelegate SwitchSurveyIfNeededBvTasksEntity;

        void IConsoleServiceHelper.SwitchSurveyIfNeeded(BvTasksEntity task)
        {

            if (SwitchSurveyIfNeededBvTasksEntity != null)
            {
                SwitchSurveyIfNeededBvTasksEntity(task);
            } else if (_inner != null)
            {
                ((IConsoleServiceHelper)_inner).SwitchSurveyIfNeeded(task);
            }
        }

    }
}