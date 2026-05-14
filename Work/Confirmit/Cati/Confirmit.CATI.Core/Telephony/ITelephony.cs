using System;

namespace Confirmit.CATI.Core.Telephony
{
    public interface ITelephony : ITelephonyCore, ITelephonyRecording, ITelephonyPlayback, ITelephonyFacilities
    {
        void SendGoReady(int dialerId, long campaignId, long agentId, Func<string> logInfoFunc);
        void SendGoNotReady(int dialerId, long campaignId, string agentId, string breakName, Func<string> logInfoFunc);
        void SendSetGroups(int dialerId, long campaignId, long agentId, int[] userGroups);
        void UpdateDialersCollection();
    }
}
