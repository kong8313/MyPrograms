using System;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Monitoring;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIMonitoringService : IMonitoringService 
    {
        private IMonitoringService _inner;

        public StubIMonitoringService()
        {
            _inner = null;
        }

        public IMonitoringService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate long StartMonitoringInt32StringStringStringBooleanDelegate(int interviewerId, string supervisorName, string projectId, string telephoneNumber, bool isWebMonitoring);
        public StartMonitoringInt32StringStringStringBooleanDelegate StartMonitoringInt32StringStringStringBoolean;

        long IMonitoringService.StartMonitoring(int interviewerId, string supervisorName, string projectId, string telephoneNumber, bool isWebMonitoring)
        {


            if (StartMonitoringInt32StringStringStringBoolean != null)
            {
                return StartMonitoringInt32StringStringStringBoolean(interviewerId, supervisorName, projectId, telephoneNumber, isWebMonitoring);
            } else if (_inner != null)
            {
                return ((IMonitoringService)_inner).StartMonitoring(interviewerId, supervisorName, projectId, telephoneNumber, isWebMonitoring);
            }

            return default(long);
        }

        public delegate void StopMonitoringInt32Int64StringDelegate(int interviewerId, long monitoringSessionId, string supervisorName);
        public StopMonitoringInt32Int64StringDelegate StopMonitoringInt32Int64String;

        void IMonitoringService.StopMonitoring(int interviewerId, long monitoringSessionId, string supervisorName)
        {

            if (StopMonitoringInt32Int64String != null)
            {
                StopMonitoringInt32Int64String(interviewerId, monitoringSessionId, supervisorName);
            } else if (_inner != null)
            {
                ((IMonitoringService)_inner).StopMonitoring(interviewerId, monitoringSessionId, supervisorName);
            }
        }

        public delegate FusionMonitoringDescription GetActiveMonitoringInt32Delegate(int interviewerId);
        public GetActiveMonitoringInt32Delegate GetActiveMonitoringInt32;

        FusionMonitoringDescription IMonitoringService.GetActiveMonitoring(int interviewerId)
        {


            if (GetActiveMonitoringInt32 != null)
            {
                return GetActiveMonitoringInt32(interviewerId);
            } else if (_inner != null)
            {
                return ((IMonitoringService)_inner).GetActiveMonitoring(interviewerId);
            }

            return default(FusionMonitoringDescription);
        }

        public delegate bool IsMonitoredInt32Delegate(int interviewerId);
        public IsMonitoredInt32Delegate IsMonitoredInt32;

        bool IMonitoringService.IsMonitored(int interviewerId)
        {


            if (IsMonitoredInt32 != null)
            {
                return IsMonitoredInt32(interviewerId);
            } else if (_inner != null)
            {
                return ((IMonitoringService)_inner).IsMonitored(interviewerId);
            }

            return default(bool);
        }

        public delegate bool IsActiveMonitoringSessionInt32Int64Delegate(int interviewerId, long monitoringSessionId);
        public IsActiveMonitoringSessionInt32Int64Delegate IsActiveMonitoringSessionInt32Int64;

        bool IMonitoringService.IsActiveMonitoringSession(int interviewerId, long monitoringSessionId)
        {


            if (IsActiveMonitoringSessionInt32Int64 != null)
            {
                return IsActiveMonitoringSessionInt32Int64(interviewerId, monitoringSessionId);
            } else if (_inner != null)
            {
                return ((IMonitoringService)_inner).IsActiveMonitoringSession(interviewerId, monitoringSessionId);
            }

            return default(bool);
        }

        public delegate bool IsAudioMonitoringSessionStartedStringDelegate(string supervisorName);
        public IsAudioMonitoringSessionStartedStringDelegate IsAudioMonitoringSessionStartedString;

        bool IMonitoringService.IsAudioMonitoringSessionStarted(string supervisorName)
        {


            if (IsAudioMonitoringSessionStartedString != null)
            {
                return IsAudioMonitoringSessionStartedString(supervisorName);
            } else if (_inner != null)
            {
                return ((IMonitoringService)_inner).IsAudioMonitoringSessionStarted(supervisorName);
            }

            return default(bool);
        }

        public delegate bool IsLiveMonitoringEnabledInt32Delegate(int interviewerId);
        public IsLiveMonitoringEnabledInt32Delegate IsLiveMonitoringEnabledInt32;

        bool IMonitoringService.IsLiveMonitoringEnabled(int interviewerId)
        {


            if (IsLiveMonitoringEnabledInt32 != null)
            {
                return IsLiveMonitoringEnabledInt32(interviewerId);
            } else if (_inner != null)
            {
                return ((IMonitoringService)_inner).IsLiveMonitoringEnabled(interviewerId);
            }

            return default(bool);
        }

        public delegate void SetLiveMonitoringStateInt32BooleanDelegate(int personId, bool isLiveMonitoringEnabled);
        public SetLiveMonitoringStateInt32BooleanDelegate SetLiveMonitoringStateInt32Boolean;

        void IMonitoringService.SetLiveMonitoringState(int personId, bool isLiveMonitoringEnabled)
        {

            if (SetLiveMonitoringStateInt32Boolean != null)
            {
                SetLiveMonitoringStateInt32Boolean(personId, isLiveMonitoringEnabled);
            } else if (_inner != null)
            {
                ((IMonitoringService)_inner).SetLiveMonitoringState(personId, isLiveMonitoringEnabled);
            }
        }

    }
}