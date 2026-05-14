using System;

namespace Confirmit.CATI.Core.Services
{
    public class BvInterviewTimings
    {
        public DateTime? TimeCallDelivered { get; set; }
        public int InterviewDurationTime { get; set; }
        public int WaitingTime { get; set; }
        public int OpenEndReviewDurationTime { get; set; }
        public int ConnectedTime { get; set; }
        public int WrapTime { get; set; }
        public int PreviewTime { get; set; }
        public int CallCenterID { get; set; }
    }
}