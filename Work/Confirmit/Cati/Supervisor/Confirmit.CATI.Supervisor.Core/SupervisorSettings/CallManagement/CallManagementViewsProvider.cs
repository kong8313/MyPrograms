using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement
{
    public class CallManagementViewsProvider : ICallManagementViewsProvider
    {
        public const string ScheduledViewName = "Scheduled";
        private const string HighPriorityViewName = "High priority";
        public const string SuspendedViewName = "Not Scheduled";
        public const string AllViewName = "All";
        private const string SentToDialerViewName = "Sent to dialer";

        public CallManagementViews MergeViews(CallManagementViews defaultViews, CallManagementViews customViews)
        {
            foreach (var customView in customViews.Views)
            {
                RemoveWrongColumns(customView);

                AddNewColumns(customView);

                defaultViews.Views.Add(customView);
            }

            SetDefaultViewIfNeeded(defaultViews);

            return defaultViews;
        }

        public CallManagementViews GetDefaultViews()
        {
            var resultView = new CallManagementViews { Views = new List<CallManagementView>() };

            resultView.Views.Add(new CallManagementView { Name = ScheduledViewName, IsDefault = false, Columns = GetDefaultColumns(ScheduledColumnKeys) });
            resultView.Views.Add(new CallManagementView { Name = HighPriorityViewName, IsDefault = false, Columns = GetDefaultColumns(HighPriorityColumnKeys) });
            resultView.Views.Add(new CallManagementView { Name = SuspendedViewName, IsDefault = false, Columns = GetDefaultColumns(SuspendedColumnKeys) });
            resultView.Views.Add(new CallManagementView { Name = AllViewName, IsDefault = false, Columns = GetDefaultColumns(AllColumnKeys) });
            resultView.Views.Add(new CallManagementView { Name = SentToDialerViewName, IsDefault = false, Columns = GetDefaultColumns(SentToDialerColumnKeys) });

            return resultView;
        }

        public CallManagementViews RemoveDefaultViews(CallManagementViews callManagementViews)
        {
            var resultView = new CallManagementViews { Views = new List<CallManagementView>() };

            for (int i = 5; i < callManagementViews.Views.Count; i++)
            {
                resultView.Views.Add(callManagementViews.Views[i]);
            }

            return resultView;
        }

        public int GetViewNameIndex(string name, int customViewIndex)
        {
            switch (name)
            {
                case ScheduledViewName:
                    return (int)CallStates.Scheduled;
                case HighPriorityViewName:
                    return (int)CallStates.HighPriority;
                case SuspendedViewName:
                    return (int)CallStates.Suspended;
                case AllViewName:
                    return (int)CallStates.All;
                case SentToDialerViewName:
                    return (int)CallStates.SentToDialer;
                default:
                    return customViewIndex;
            }
        }

        public string GetTranslation(CallManagementColumnKey callManagementColumnKey)
        {
            switch (callManagementColumnKey)
            {
                case CallManagementColumnKey.InterviewID:
                    return Strings.ResourceManager.GetString("InterviewId");
                case CallManagementColumnKey.TelephoneNumber:
                    return Strings.ResourceManager.GetString("TelNumber");
                case CallManagementColumnKey.RespondentName:
                    return Strings.ResourceManager.GetString("RespondentName");
                case CallManagementColumnKey.DialingMode:
                    return Strings.ResourceManager.GetString("DialingModeColumnText");
                case CallManagementColumnKey.DialTypeId:
                    return Strings.ResourceManager.GetString("DialTypeName");
                case CallManagementColumnKey.TimeText:
                    return Strings.ResourceManager.GetString("TimeInShift");
                case CallManagementColumnKey.Priority:
                    return Strings.ResourceManager.GetString("CallPriority");
                case CallManagementColumnKey.StateName:
                    return Strings.ResourceManager.GetString("ExtendedStatus");
                case CallManagementColumnKey.Resource:
                    return Strings.ResourceManager.GetString("AssignedTo");
                case CallManagementColumnKey.TimezoneName:
                    return Strings.ResourceManager.GetString("Timezone");
                case CallManagementColumnKey.AttemptNumber:
                    return Strings.ResourceManager.GetString("CallAttempts");
                case CallManagementColumnKey.ShiftType:
                    return Strings.ResourceManager.GetString("ShiftTypeName");
                case CallManagementColumnKey.QuestionColumnsPosition:
                    return Strings.ResourceManager.GetString("QuestionColumnsPosition");
                case CallManagementColumnKey.ExpireTimeText:
                    return Strings.ResourceManager.GetString("ExpireTime");
                case CallManagementColumnKey.LastCallTimeText:
                    return Strings.ResourceManager.GetString("LastCallTime");
                case CallManagementColumnKey.LastInterviewerName:
                    return Strings.ResourceManager.GetString("LastInterviewerName");
                case CallManagementColumnKey.ApptTimeText:
                    return Strings.ResourceManager.GetString("AppointmentTime");
                case CallManagementColumnKey.ExpTimeText:
                    return Strings.ResourceManager.GetString("AppointmentExpTime");
                case CallManagementColumnKey.CallState:
                    return Strings.ResourceManager.GetString("StateName");
                case CallManagementColumnKey.ReviewStatus:
                    return Strings.ResourceManager.GetString("ReviewStatus");
                default:
                    return callManagementColumnKey.ToString();
            }
        }

        private void SetDefaultViewIfNeeded(CallManagementViews callManagementViews)
        {
            if (callManagementViews.Views.All(x => !x.IsDefault))
            {
                callManagementViews.Views[0].IsDefault = true;
            }
        }

        private void RemoveWrongColumns(CallManagementView customView)
        {
            int index = 0;
            while (index < customView.Columns.Count)
            {
                if (!ScheduledColumnKeys.Contains(customView.Columns[index].ColumnKey))
                {
                    customView.Columns.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        private void AddNewColumns(CallManagementView customView)
        {
            foreach (var scheduledColumnKey in ScheduledColumnKeys)
            {
                if (customView.Columns.All(x => x.ColumnKey != scheduledColumnKey))
                {
                    customView.Columns.Add(new CallManagementColumn { ColumnKey = scheduledColumnKey, IsVisible = false });
                }
            }
        }

        private List<CallManagementColumn> GetDefaultColumns(List<CallManagementColumnKey> columnKeys)
        {
            return columnKeys.Select(columnKey => new CallManagementColumn { ColumnKey = columnKey, IsVisible = true }).ToList();
        }

        public List<CallManagementColumnKey> ScheduledColumnKeys
        {
            get
            {
                return new List<CallManagementColumnKey>
                {
                    CallManagementColumnKey.InterviewID,
                    CallManagementColumnKey.TelephoneNumber,
                    CallManagementColumnKey.RespondentName,
                    CallManagementColumnKey.DialingMode,
                    CallManagementColumnKey.DialTypeId,
                    CallManagementColumnKey.TimeText,
                    CallManagementColumnKey.Priority,
                    CallManagementColumnKey.StateName,
                    CallManagementColumnKey.Resource,
                    CallManagementColumnKey.TimezoneName,
                    CallManagementColumnKey.AttemptNumber,
                    CallManagementColumnKey.ShiftType,
                    CallManagementColumnKey.QuestionColumnsPosition,
                    CallManagementColumnKey.ExpireTimeText,
                    CallManagementColumnKey.LastCallTimeText,
                    CallManagementColumnKey.ApptTimeText,
                    CallManagementColumnKey.ExpTimeText,
                    CallManagementColumnKey.CallState
                };
            }
        }

        private List<CallManagementColumnKey> HighPriorityColumnKeys
        {
            get
            {
                return new List<CallManagementColumnKey>
                {
                    CallManagementColumnKey.InterviewID,
                    CallManagementColumnKey.TelephoneNumber,
                    CallManagementColumnKey.RespondentName,
                    CallManagementColumnKey.DialingMode,
                    CallManagementColumnKey.DialTypeId,
                    CallManagementColumnKey.TimeText,
                    CallManagementColumnKey.Priority,
                    CallManagementColumnKey.StateName,
                    CallManagementColumnKey.Resource,
                    CallManagementColumnKey.TimezoneName,
                    CallManagementColumnKey.AttemptNumber,
                    CallManagementColumnKey.ShiftType,
                    CallManagementColumnKey.QuestionColumnsPosition,
                    CallManagementColumnKey.ExpireTimeText,
                    CallManagementColumnKey.LastCallTimeText,
                    CallManagementColumnKey.ApptTimeText,
                    CallManagementColumnKey.ExpTimeText
                };
            }
        }

        private List<CallManagementColumnKey> SuspendedColumnKeys
        {
            get
            {
                return new List<CallManagementColumnKey>
                {
                    CallManagementColumnKey.InterviewID,
                    CallManagementColumnKey.TelephoneNumber,
                    CallManagementColumnKey.RespondentName,
                    CallManagementColumnKey.DialingMode,
                    CallManagementColumnKey.DialTypeId,
                    CallManagementColumnKey.StateName,
                    CallManagementColumnKey.TimezoneName,
                    CallManagementColumnKey.AttemptNumber,
                    CallManagementColumnKey.QuestionColumnsPosition,
                    CallManagementColumnKey.LastCallTimeText
                };
            }
        }

        private List<CallManagementColumnKey> AllColumnKeys
        {
            get
            {
                return new List<CallManagementColumnKey>
                {
                    CallManagementColumnKey.InterviewID,
                    CallManagementColumnKey.TelephoneNumber,
                    CallManagementColumnKey.RespondentName,
                    CallManagementColumnKey.TimeText,
                    CallManagementColumnKey.DialingMode,
                    CallManagementColumnKey.DialTypeId,
                    CallManagementColumnKey.StateName,
                    CallManagementColumnKey.TimezoneName,
                    CallManagementColumnKey.AttemptNumber,
                    CallManagementColumnKey.QuestionColumnsPosition,
                    CallManagementColumnKey.LastCallTimeText,
                    CallManagementColumnKey.LastInterviewerName,
                    CallManagementColumnKey.ApptTimeText,
                    CallManagementColumnKey.ExpTimeText,
                    CallManagementColumnKey.ReviewStatus
                };
            }
        }

        private List<CallManagementColumnKey> SentToDialerColumnKeys
        {
            get
            {
                return new List<CallManagementColumnKey>
                {
                    CallManagementColumnKey.InterviewID,
                    CallManagementColumnKey.TelephoneNumber,
                    CallManagementColumnKey.RespondentName,
                    CallManagementColumnKey.DialingMode,
                    CallManagementColumnKey.DialTypeId,
                    CallManagementColumnKey.TimeText,
                    CallManagementColumnKey.Priority,
                    CallManagementColumnKey.StateName,
                    CallManagementColumnKey.Resource,
                    CallManagementColumnKey.TimezoneName,
                    CallManagementColumnKey.AttemptNumber,
                    CallManagementColumnKey.ShiftType,
                    CallManagementColumnKey.QuestionColumnsPosition,
                    CallManagementColumnKey.ExpireTimeText,
                    CallManagementColumnKey.LastCallTimeText,
                    CallManagementColumnKey.ApptTimeText,
                    CallManagementColumnKey.ExpTimeText
                };
            }
        }
    }
}