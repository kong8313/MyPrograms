using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.Services.CallDelivery.Interfaces
{
    public class CallRequestResult
    {
        public int CallId { get; set; }
        public int SurveyId { get; set; }
        public int InterviewId { get; set; }
        public BvActiveDialEntity ActiveDial { get; set; }
    }
    internal interface ICallRequest
    {
        string Description { get; }

        CallRequestResult Execute();
    }
}
