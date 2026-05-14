using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confirmit.TelephonyProblemStates.ProblemState;
using ConfirmitDialerInterface;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    public class CatiProblemStateFactory
    {
        private const int CatiInterviewerBackendErrorId = 300;

        private static IDictionary<int, Func<ICatiProblemStateInfo, CatiProblemState>> _data;
        private readonly ICatiProblemStateInfo _additionalInfo;

        static CatiProblemStateFactory()
        {
            _data = new Dictionary<int, Func<ICatiProblemStateInfo, CatiProblemState>>
                {
                    {(int)DialerErrorCode.UnknownAgent, info=> new UnknownAgentState((int)DialerErrorCode.UnknownAgent)},
                    {(int)DialerErrorCode.WrongAgentState, info=> new WrongAgentStateState((int)DialerErrorCode.WrongAgentState)},
                    {(int)DialerErrorCode.AgentAlreadyLoggedIn, info=> new ExistsAlreadyState((int)DialerErrorCode.AgentAlreadyLoggedIn)},
                    {(int)DialerErrorCode.Success, info=> new SuccessState((int)DialerErrorCode.Success)},
                    {(int)DialerErrorCode.NotAvailable, info=> new DialerIsNotAvailable((int)DialerErrorCode.NotAvailable)},
                    {(int)DialerErrorCode.Exception, info=> new DialerCallExceptionState((int)DialerErrorCode.Exception)},
                    {(int)DialerErrorCode.NotSupported, info=> new MethodNotSupportedState((int)DialerErrorCode.NotSupported)},
                    {(int)DialerErrorCode.InvalidParameter, info=> new InvalidParamterState((int)DialerErrorCode.InvalidParameter)},
                    {(int)DialerErrorCode.InvalidExtension, info=> new WrongExtensionState((int)DialerErrorCode.InvalidExtension)},
                    {(int)DialerErrorCode.ResourceAlreadyInUse, info=> new ResourceAlreadyInUseState((int)DialerErrorCode.ResourceAlreadyInUse, info.StationId)},
                    {(int)DialerErrorCode.ResourceNotFound, info=> new ResourceNotFoundState((int)DialerErrorCode.ResourceNotFound, info.StationId)},
                    {(int)DialerErrorCode.NoMoreLicences, info=> new NoMoreLicencesState((int)DialerErrorCode.NoMoreLicences)},
                    {(int)DialerErrorCode.InvalidPhoneNumber, info=> new WrongNumberState((int)DialerErrorCode.InvalidPhoneNumber)},
                    {(int)DialerErrorCode.NoMoreConferenceResources, info=> new NoMoreConferenceResourcesState((int)DialerErrorCode.NoMoreConferenceResources)},
                    {(int)DialerErrorCode.NoMoreFreeChannels, info=> new NoMoreFreeChannelsState((int)DialerErrorCode.NoMoreFreeChannels)},
                    {(int)DialerErrorCode.UnknownError, info=> new LowLevelFaultState((int)DialerErrorCode.UnknownError)},
                    {(int)DialerErrorCode.WrongStateDialingInProgress, info=> new WrongStateDialingInProgressState((int)DialerErrorCode.WrongStateDialingInProgress)},
                    {(int)DialerErrorCode.WrongStatePaused, info=> new WrongStatePausedState((int)DialerErrorCode.WrongStatePaused)},
                    {(int)DialerErrorCode.WrongStateResourceIsBusy, info=> new WrongStateResourceIsBusyState((int)DialerErrorCode.WrongStateResourceIsBusy, info.StationId)},
                    {(int)DialerErrorCode.Forbidden, info=> new ForbiddenState((int)DialerErrorCode.Forbidden)},
                    {(int)DialerErrorCode.UnknownSupervisor, info=> new UnknownSupervisorState((int)DialerErrorCode.UnknownSupervisor)},
                    {(int)DialerErrorCode.AgentIsNotLoggedin, info=> new AgentIsNotLoggedState((int)DialerErrorCode.AgentIsNotLoggedin)},
                    {(int)DialerErrorCode.AgentAlreadyBeingMonitored, info=> new AgentAlreadyBeingMonitoredState((int)DialerErrorCode.AgentAlreadyBeingMonitored)},
                    {(int)DialerErrorCode.InvalidDialingMode, info=> new InvalidDialingModeState((int)DialerErrorCode.InvalidDialingMode)},
                    {(int)DialerErrorCode.Restarted, info=> new DialerRestartedState((int)DialerErrorCode.Restarted)},
                    {(int)DialerErrorCode.NoMoreSupervisorResources, info=> new NoMoreSupervisorResourcesState((int)DialerErrorCode.NoMoreSupervisorResources)},
                    {(int)DialerErrorCode.PhoneNumberAlreadyInUse, info=> new PhoneNumberAlreadyInUseState((int)DialerErrorCode.PhoneNumberAlreadyInUse)},
                    {(int)DialerErrorCode.MonitoringIsAlreadyStarted, info=> new MonitoringIsAlreadyStartedState((int)DialerErrorCode.MonitoringIsAlreadyStarted)},
                    {CatiInterviewerBackendErrorId, info=> new CatiInterviewerErrorState(CatiInterviewerBackendErrorId)},
                };
        }

        public CatiProblemStateFactory(ICatiProblemStateInfo additionalInfo)
        {
            if (additionalInfo == null)
            {
                throw new ArgumentNullException("additionalInfo");
            }

            _additionalInfo = additionalInfo;
        }

        public CatiProblemState GetState(DialerErrorCode dialerErrorCode)
        {
            return GetState((int) dialerErrorCode);
        }

        public CatiProblemState GetState(int errorCode)
        {
            Func<ICatiProblemStateInfo, CatiProblemState> stateActivator;
            if (_data.TryGetValue(errorCode, out stateActivator) == false)
            {
                Trace.TraceError("Unknown dialer error code {0} has been detected", errorCode);
                return new UnknownErrorState(errorCode);
            }

            return stateActivator(_additionalInfo);
        }
    }
}
