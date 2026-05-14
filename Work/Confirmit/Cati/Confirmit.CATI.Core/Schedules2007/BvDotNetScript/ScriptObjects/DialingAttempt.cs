using System;
using System.Collections.Generic;
using Confirmit.CATI.Common;

namespace BvDotNetScript.ScriptObjects
{
    public class DialingAttempt
    {
        public long DialId { get; set; }
        
        public string TelephoneNumber { get; set; }
        
        public string DialerCallerId { get; set; }
        
        public int? RingTime { get; set; }
        
        public int? DialerCallOutcome { get; set; }
        
        private Dictionary<string, string> CallOutcomeMetadata { get; set; }
        
        public DialingAttempt(long dialId, string dialerCallerId, int? ringTime, int? dialerCallOutcome, Dictionary<string, string> callOutcomeMetadata, string telephoneNumber)
        {
            DialId = dialId;
            TelephoneNumber = telephoneNumber;
            DialerCallerId = dialerCallerId;
            RingTime = ringTime;
            DialerCallOutcome = dialerCallOutcome;
            CallOutcomeMetadata = callOutcomeMetadata ?? new Dictionary<string, string>();
        }

        public string GetMetadata(string key)
        {
            return CallOutcomeMetadata.GetValueOrDefault(key, null);
        }
    }
}