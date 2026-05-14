using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement
{
    public enum CallManagementColumnKey
    {
        InterviewID,
        TelephoneNumber,
        RespondentName,
        DialingMode,
        DialTypeId,
        TimeText,
        Priority,
        StateName,
        Resource,
        TimezoneName,
        AttemptNumber,
        ShiftType,
        QuestionColumnsPosition, // this is not a real column key. it is collect information about order of all additional question variable columns among all columns
        ExpireTimeText,
        LastCallTimeText,
        LastInterviewerName,
        ApptTimeText,
        ExpTimeText,
        CallState,
        ReviewStatus
    }

    [Serializable]
    public class CallManagementViews
    {
        public List<CallManagementView> Views { get; set; }
    }

    [Serializable]
    public class CallManagementView
    {
        public string Name;

        public List<CallManagementColumn> Columns;

        public bool IsDefault;
    }

    [Serializable]
    public class CallManagementColumn
    {
        public CallManagementColumnKey ColumnKey;

        public bool IsVisible;
    }
}