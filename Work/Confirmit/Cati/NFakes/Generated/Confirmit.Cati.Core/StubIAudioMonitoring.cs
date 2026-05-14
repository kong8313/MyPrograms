using System;
using BvCallHandlerLibrary;
using ConfirmitDialerInterface;

namespace BvCallHandlerLibrary.Fakes
{
    public class StubIAudioMonitoring : IAudioMonitoring 
    {
        private IAudioMonitoring _inner;

        public StubIAudioMonitoring()
        {
            _inner = null;
        }

        public IAudioMonitoring Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void StartAudioMonitorStringInt32StringDelegate(string supervisorName, int interviewerId, string telephoneNumber);
        public StartAudioMonitorStringInt32StringDelegate StartAudioMonitorStringInt32String;

        void IAudioMonitoring.StartAudioMonitor(string supervisorName, int interviewerId, string telephoneNumber)
        {

            if (StartAudioMonitorStringInt32String != null)
            {
                StartAudioMonitorStringInt32String(supervisorName, interviewerId, telephoneNumber);
            } else if (_inner != null)
            {
                ((IAudioMonitoring)_inner).StartAudioMonitor(supervisorName, interviewerId, telephoneNumber);
            }
        }

        public delegate void StopAudioMonitorStringInt32Delegate(string supervisorName, int interviewerId);
        public StopAudioMonitorStringInt32Delegate StopAudioMonitorStringInt32;

        void IAudioMonitoring.StopAudioMonitor(string supervisorName, int interviewerId)
        {

            if (StopAudioMonitorStringInt32 != null)
            {
                StopAudioMonitorStringInt32(supervisorName, interviewerId);
            } else if (_inner != null)
            {
                ((IAudioMonitoring)_inner).StopAudioMonitor(supervisorName, interviewerId);
            }
        }

        public delegate void SetMonitorModeStringInt32MonitorModeDelegate(string supervisorName, int interviewerId, MonitorMode monitorMode);
        public SetMonitorModeStringInt32MonitorModeDelegate SetMonitorModeStringInt32MonitorMode;

        void IAudioMonitoring.SetMonitorMode(string supervisorName, int interviewerId, MonitorMode monitorMode)
        {

            if (SetMonitorModeStringInt32MonitorMode != null)
            {
                SetMonitorModeStringInt32MonitorMode(supervisorName, interviewerId, monitorMode);
            } else if (_inner != null)
            {
                ((IAudioMonitoring)_inner).SetMonitorMode(supervisorName, interviewerId, monitorMode);
            }
        }

    }
}