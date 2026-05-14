using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Backend.WcfServices.Internal.ManagementService
{
    public class BvInterviewTimings
    {
        public DateTime? TimeCallDelivered { get; set; }
        public int InterviewDuriationTime { get; set; }
        public int WaitingTime { get; set; }
        public int OpenEndReviewDurationTime { get; set; }
        public int CallCenterID { get; set; }
    }
}
