using System;
using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class AssignResourcesToSurveyEventParameters : ManagementActivityEventDetails
    {
        public int[] ResourceSids { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.AssignResourcesToSurvey)]
    public class AssignResourcesToSurveyEvent : ManagementActivityEvent<AssignResourcesToSurveyEventParameters>
    {
        public AssignResourcesToSurveyEvent(int surveySid, string projectId, IEnumerable<int> resourceSids):
            base(ManagementEventCategory.Assignment, ManagementEvent.AssignResourcesToSurvey)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
            Details = new AssignResourcesToSurveyEventParameters { ResourceSids = resourceSids.ToArray() };
        }
    }

    [Serializable]
    public class AssignSurveysToResourceEventParameters : ManagementActivityEventDetails
    {
        public int[] SurveyIds { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.AssignResourcesToSurveyUsingSurveyAssignmentsDialog)]
    public class AssignResourcesToSurveyUsingSurveyAssignmentsDialogEvent : ManagementActivityEvent<AssignResourcesToSurveyEventParameters>
    {
        public AssignResourcesToSurveyUsingSurveyAssignmentsDialogEvent(int surveySid, string projectId, IEnumerable<int> resourceSids):
            base(ManagementEventCategory.Assignment, ManagementEvent.AssignResourcesToSurveyUsingSurveyAssignmentsDialog)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
            Details = new AssignResourcesToSurveyEventParameters { ResourceSids = resourceSids.ToArray() };
        }
    }

    [ManagementEventAttribute(ManagementEvent.AssignSurveysToResource)]
    public class AssignSurveysToResourceEvent : ManagementActivityEvent<AssignSurveysToResourceEventParameters>
    {
        public AssignSurveysToResourceEvent(int interviewerId, string interviewerName, IEnumerable<int> surveyIds):
            base(ManagementEventCategory.Assignment, ManagementEvent.AssignSurveysToResource)
        {
            ObjectId = interviewerId;
            ObjectName = interviewerName;
            Details = new AssignSurveysToResourceEventParameters { SurveyIds = surveyIds.ToArray() };
        }
    }

    [Serializable]
    public class DeassignResourcesFromSurveyEventParameters : ManagementActivityEventDetails
    {
        public int[] ResourceSids { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.DeassignResourcesFromSurvey)]
    public class DeassignResourcesFromSurveyEvent : ManagementActivityEvent<DeassignResourcesFromSurveyEventParameters>
    {
        public DeassignResourcesFromSurveyEvent(int surveySid, string projectId, IEnumerable<int> resourceSids):
            base(ManagementEventCategory.Assignment, ManagementEvent.DeassignResourcesFromSurvey)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
            Details = new DeassignResourcesFromSurveyEventParameters { ResourceSids = resourceSids.ToArray() };
        }
    }

    [Serializable]
    public class DeassignResourcesFromSurveyCallsEventParameters : ManagementActivityEventDetails
    {
        public int[] ResourceSids { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.DeassignResourcesFromSurveyUsingSurveyAssignmentsDialog)]
    public class DeassignResourcesFromSurveyUsingSurveyAssignmentsDialogEvent : ManagementActivityEvent<DeassignResourcesFromSurveyEventParameters>
    {
        public DeassignResourcesFromSurveyUsingSurveyAssignmentsDialogEvent(int surveySid, string projectId, IEnumerable<int> resourceSids):
            base(ManagementEventCategory.Assignment, ManagementEvent.DeassignResourcesFromSurveyUsingSurveyAssignmentsDialog)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
            Details = new DeassignResourcesFromSurveyEventParameters { ResourceSids = resourceSids.ToArray() };
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeassignResourcesFromSurveyCalls)]
    public class DeassignResourcesFromSurveyCallsEvent : ManagementActivityEvent<DeassignResourcesFromSurveyCallsEventParameters>
    {
        public DeassignResourcesFromSurveyCallsEvent(int surveySid, string projectId, IEnumerable<int> resourceSids):
            base(ManagementEventCategory.Assignment, ManagementEvent.DeassignResourcesFromSurveyCalls)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
            Details = new DeassignResourcesFromSurveyCallsEventParameters { ResourceSids = resourceSids.ToArray() };
        }
    }

    [Serializable]
    public class ReplacePersonSurveyAssignmentEventParameters : ManagementActivityEventDetails
    {
        public int[] SelectedSurveysIDs { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ReplacePersonSurveyAssignment)]
    public class ReplacePersonSurveyAssignmentEvent : ManagementActivityEvent<ReplacePersonSurveyAssignmentEventParameters>
    {
        public ReplacePersonSurveyAssignmentEvent(int interviewerOrGroupId, string interviewerName, IEnumerable<int> selectedSurveysIDs):
            base(ManagementEventCategory.Assignment, ManagementEvent.ReplacePersonSurveyAssignment)
        {
            ObjectId = interviewerOrGroupId;
            ObjectName = interviewerName;
            Details = new ReplacePersonSurveyAssignmentEventParameters { SelectedSurveysIDs = selectedSurveysIDs.ToArray() };
        }
    }

    [Serializable]
    public class ReplaceSurveyPersonAssignmentEventParameters : ManagementActivityEventDetails
    {
        public int[] SelectedInterviewerOrGroupIDs { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ReplaceSurveyPersonAssignment)]
    public class ReplaceSurveyPersonAssignmentEvent : ManagementActivityEvent<ReplaceSurveyPersonAssignmentEventParameters>
    {
        public ReplaceSurveyPersonAssignmentEvent(int surveyId, string surveyName, IEnumerable<int> selectedInterviewerOrGroupIDs):
            base(ManagementEventCategory.Assignment, ManagementEvent.ReplaceSurveyPersonAssignment)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
            Details = new ReplaceSurveyPersonAssignmentEventParameters { SelectedInterviewerOrGroupIDs = selectedInterviewerOrGroupIDs.ToArray() };
        }
    }
}