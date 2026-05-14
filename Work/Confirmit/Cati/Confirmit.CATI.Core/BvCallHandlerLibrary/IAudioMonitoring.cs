using ConfirmitDialerInterface;

namespace BvCallHandlerLibrary
{
    public interface IAudioMonitoring
    {
        void StartAudioMonitor(string supervisorName, int interviewerId, string telephoneNumber);

        void StopAudioMonitor(string supervisorName, int interviewerId);

        void SetMonitorMode(string supervisorName, int interviewerId, MonitorMode monitorMode);
    }
}