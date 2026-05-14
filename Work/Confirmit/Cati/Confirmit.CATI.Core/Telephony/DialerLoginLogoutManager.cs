using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using BvCallHandlerLibrary;

using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    /// <summary>
    /// Class to manage all Login/Logout operations
    /// If we have things to check all of these things must be checked here
    /// NOTE: Not completely implemented yet.
    /// </summary>
    public class DialerLoginLogoutManager : IDialerLoginLogoutManager
    {
        private readonly ITelephony _telephony;
        private readonly IAudioMonitoring _audioMonitoring;

        public DialerLoginLogoutManager(
            ITelephony telephony,
            IAudioMonitoring audioMonitoring)
        {
            _telephony = telephony;
            _audioMonitoring = audioMonitoring;
        }

        /// <summary>
        /// We should stop audio monitoring if some before logging out from dialer
        /// </summary>        
        public DialerErrorCode Logout(int dialerId, long campaignId, bool isPredictive, int agentId)
        {
            try
            {
                // check first if interviewer being monitored
                string supervisorName = GetSupervisorName(agentId);

                if (!string.IsNullOrEmpty(supervisorName))
                {
                    _audioMonitoring.StopAudioMonitor(supervisorName, agentId);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                        "DialerLoginLogoutManager.Logout dialerId= {0}, campaignId = {1}, agentId= {2}. Monitor stopping is failed: \r\n{3} /// {4}",
                        dialerId,
                        campaignId,
                        agentId,
                        ex,
                        new StackTrace(true));
            }

            return _telephony.Logout(dialerId, campaignId, isPredictive, agentId.ToString());
        }

        private static string GetSupervisorName(int interviewerId)
        {
            var entity = AudioMonitoringAdapter.GetByCondition(
                 "[InterviewerSID] = @InterviewerId",
                 new SqlParameter("@InterviewerId", interviewerId)).FirstOrDefault();

            return (entity != null) ? entity.SupervisorName : null;
        }
    }
}