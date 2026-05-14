using System;

namespace BvDotNetScript.ScriptObjects
{
    public class CallAttempt
    {
        public int AttemptNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime StartTimeRespondent { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public DateTime EndTimeRespondent { get; set; }
        public int? InterviwerId { get; set; }
        public ExtendedStatus ExtendedStatus { get; set; }
        public string AaporCode { get; set; }
        public int Duration { get; set; }
        public int OpenEndReviewDuration { get; set; }
        public int PreviewTime { get; set; }
        public int ConnectedTime { get; set; }
        public int WrapTime { get; set; }
        public int WaitingTime { get; set; }
        public int CallCenterId { get; set; }
    }
}