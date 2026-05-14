using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.CallCenters;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [ManagementEvent(ManagementEvent.CreateCallCenter)]
    public class CreateCallCenterEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public CreateCallCenterEvent(int callCenterId, string callCenterName):
            base(ManagementEventCategory.CallCenter, ManagementEvent.CreateCallCenter)
        {
            ObjectId = callCenterId;
            ObjectName = callCenterName;
        }
    }

    [Serializable]
    public class UpdateCallCenterEventParameters : ManagementActivityEventDetails
    {
        public UpdateCallCenterEventParameters() { }

        public UpdateCallCenterEventParameters(IEnumerable<int> assignedDialerIds)
        {
            if (assignedDialerIds != null)
            {
                AssignedDialersList = string.Join(", ", assignedDialerIds);
            }
        }

        public string AssignedDialersList = string.Empty;
    }
    
    [ManagementEvent(ManagementEvent.UpdateCallCenter)]
    public class UpdateCallCenterEvent : ManagementActivityEvent<UpdateCallCenterEventParameters>
    {
        public UpdateCallCenterEvent(int callCenterId, string callCenterName, IEnumerable<int> assignedDialerIds):
            base(ManagementEventCategory.CallCenter, ManagementEvent.UpdateCallCenter)
        {
            ObjectId = callCenterId;
            ObjectName = callCenterName;
            Details = new UpdateCallCenterEventParameters(assignedDialerIds);
        }
    }

    [Serializable]
    public class DeleteCallCenterEventParameters : ManagementActivityEventDetails
    {
        public DeleteCallCenterEventParameters() { }

        public DeleteCallCenterEventParameters(int moveToCallCenterId,
                                               InterviewerActionOnCallCenterDelete interviewerAction)
        {
            MoveExistingDataToCallCenter = moveToCallCenterId;
            ProcessExisitingInterviewers = interviewerAction;
        }

        public int MoveExistingDataToCallCenter;
        public InterviewerActionOnCallCenterDelete ProcessExisitingInterviewers;
    }

    [ManagementEvent(ManagementEvent.DeleteCallCenter)]
    public class DeleteCallCenterEvent : ManagementActivityEvent<DeleteCallCenterEventParameters>
    {
        public DeleteCallCenterEvent(int callCenterId, string callCenterName, int moveDataCallCenterId, InterviewerActionOnCallCenterDelete interviewerAction):
            base(ManagementEventCategory.CallCenter, ManagementEvent.DeleteCallCenter)
        {
            ObjectId = callCenterId;
            ObjectName = callCenterName;
            Details = new DeleteCallCenterEventParameters(moveDataCallCenterId, interviewerAction);
        }
    }

    [Serializable]
    public class AssignSupervisorsToCallCenterEventParameters : ManagementActivityEventDetails
    {
        public string Supervisors = string.Empty;

        public AssignSupervisorsToCallCenterEventParameters() { }

        public AssignSupervisorsToCallCenterEventParameters(IEnumerable<string> names)
        {
            Supervisors = string.Join(",", names);
        }
    }

    [ManagementEvent(ManagementEvent.AssignSupervisorsToCallCenter)]
    public class AssignSupervisorsToCallCenterEvent : ManagementActivityEvent<AssignSupervisorsToCallCenterEventParameters>
    {
        public AssignSupervisorsToCallCenterEvent(int callCenterId, IEnumerable<string> names):
            base(ManagementEventCategory.CallCenter, ManagementEvent.AssignSupervisorsToCallCenter)
        {
            ObjectId = callCenterId;
            Details = new AssignSupervisorsToCallCenterEventParameters(names);
        }
    }

    [Serializable]
    public class AssignSurveysToCallCentersEventParameters : ManagementActivityEventDetails
    {
        public int[] CallCenterIds;
        public int[] SurveyIds;
        public bool ReplaceExistingAssignments;

        public AssignSurveysToCallCentersEventParameters() { }

        public AssignSurveysToCallCentersEventParameters(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds,
                                                         bool replaceExisting)
        {
            CallCenterIds = callCenterIds.ToArray();
            SurveyIds = surveyIds.ToArray();
            ReplaceExistingAssignments = replaceExisting;
        }
    }

    [ManagementEvent(ManagementEvent.AssignSurveysToCallCenters)]
    public class AssignSurveysToCallCentersEvent : ManagementActivityEvent<AssignSurveysToCallCentersEventParameters>
    {
        public AssignSurveysToCallCentersEvent(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds, bool replaceExisting = false):
            base(ManagementEventCategory.CallCenter, ManagementEvent.AssignSurveysToCallCenters)
        {
            Details = new AssignSurveysToCallCentersEventParameters(callCenterIds, surveyIds, replaceExisting);
        }
    }
}
