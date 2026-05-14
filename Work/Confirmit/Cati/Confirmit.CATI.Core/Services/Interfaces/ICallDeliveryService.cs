using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface ICallDeliveryService
    {
        List<CallInfo> LookupCalls(int surveySid, 
            int dialerId,
            int? groupId, CallsSelectionAlgorithm callsSelectionAlgorithm, int count,
            bool isRecording, out List<GroupInfo> aggregatedGroupsInfo);

        CallRequestResult LookupCall(
            int personId,
            int surveyId,
            int interviewId,
            GetCallEvent activityEvent);

        void WrapupCall(BvCallEntity call);
    }
}
