using System;
using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class CreateFilterEventParameters : ManagementActivityEventDetails
    {
        public int SurveySid { get; set; }
        public bool IsHidden { get; set; }
        public int[] FieldIds { get; set; }
        public string[] Columns { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.CreateFilter)]
    public class CreateFilterEvent : ManagementActivityEvent<CreateFilterEventParameters>
    {
        public CreateFilterEvent(
            int filterSid,
            string filterName,
            int surveySid,
            bool isHidden,
            IEnumerable<int> fieldIds,
            IEnumerable<string> columns):
            base(ManagementEventCategory.CallManagementUI, ManagementEvent.CreateFilter)
        {
            ObjectId = filterSid;
            ObjectName = filterName;
            Details = new CreateFilterEventParameters { SurveySid = surveySid, IsHidden = isHidden, FieldIds = fieldIds.ToArray(), Columns = columns.ToArray() };
        }
    }

    [Serializable]
    public class UpdateFilterEventParameters : ManagementActivityEventDetails
    {
        public int SurveySid { get; set; }
        public bool IsHidden { get; set; }
        public int[] FieldIds { get; set; }
        public string[] Columns { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.UpdateFilter)]
    public class UpdateFilterEvent : ManagementActivityEvent<UpdateFilterEventParameters>
    {
        public UpdateFilterEvent(
            int filterSid,
            string filterName,
            int surveySid,
            bool isHidden,
            IEnumerable<int> fieldIds,
            IEnumerable<string> columns):
            base(ManagementEventCategory.CallManagementUI, ManagementEvent.UpdateFilter)
        {
            ObjectId = filterSid;
            ObjectName = filterName;
            Details = new UpdateFilterEventParameters { SurveySid = surveySid, IsHidden = isHidden, FieldIds = fieldIds.ToArray(), Columns = columns.ToArray() };
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteFilter)]
    public class DeleteFilterEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public DeleteFilterEvent(int filterSid, string filterName):
            base(ManagementEventCategory.CallManagementUI, ManagementEvent.DeleteFilter)
        {
            ObjectId = filterSid;
            ObjectName = filterName;
        }
    }

    [Serializable]
    public class MoveOrCopySurveySpecificFiltersToSurveyEventParameters : ManagementActivityEventDetails
    {
        public int SourceSurveySid { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.MoveSurveySpecificFiltersToSurvey)]
    public class MoveSurveySpecificFiltersToSurveyEvent : ManagementActivityEvent<MoveOrCopySurveySpecificFiltersToSurveyEventParameters>
    {
        public MoveSurveySpecificFiltersToSurveyEvent(int sourceSurveySid, int targetSurveySid, string targetProjectId):
            base(ManagementEventCategory.CallManagementUI, ManagementEvent.MoveSurveySpecificFiltersToSurvey)
        {
            ObjectId = targetSurveySid;
            ObjectName = targetProjectId;
            Details = new MoveOrCopySurveySpecificFiltersToSurveyEventParameters { SourceSurveySid = sourceSurveySid };
        }
    }

    [ManagementEventAttribute(ManagementEvent.CopySurveySpecificFiltersToSurvey)]
    public class CopySurveySpecificFiltersToSurveyEvent : ManagementActivityEvent<MoveOrCopySurveySpecificFiltersToSurveyEventParameters>
    {
        public CopySurveySpecificFiltersToSurveyEvent(int sourceSurveySid, int targetSurveySid, string targetProjectId):
            base(ManagementEventCategory.CallManagementUI, ManagementEvent.CopySurveySpecificFiltersToSurvey)
        {
            ObjectId = targetSurveySid;
            ObjectName = targetProjectId;
            Details = new MoveOrCopySurveySpecificFiltersToSurveyEventParameters { SourceSurveySid = sourceSurveySid };
        }
    }
}