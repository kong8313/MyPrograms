using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Core.Activity.CustomizableColumns;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.ActivityViews;
using Confirmit.CATI.Supervisor.Resources;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class SurveyActivityViewCustomizableColumnsService : ICustomizableColumnsService
    {
        private readonly ISupervisorSettingsRepository _settingsRepository;
        private readonly IActivityManager _activityManager;

        private List<StatusAlertInfo> _itsList;
        private List<ColumnDescription> _mergedColumnDescriptions;
        private List<ColumnDescription> _savedColumnDescriptions;

        public SurveyActivityViewCustomizableColumnsService(ISupervisorSettingsRepository settingsRepository, IActivityManager activityManager)
        {
            _settingsRepository = settingsRepository;
            _activityManager = activityManager;
            Init();
        }

        private void Init()
        {
            _savedColumnDescriptions = _settingsRepository.ReadSurveyActivityViewColumnSettings();
            _itsList = _activityManager.GetStatusAlertsList(true);
            _mergedColumnDescriptions = MergeColumnDescriptions(_defaultColumnDescriptions, _savedColumnDescriptions);
        }

        private IReadOnlyCollection<ColumnDescription> _defaultColumnDescriptions = new ReadOnlyCollection<ColumnDescription>(new List<ColumnDescription>
        {
            new ColumnDescription("LoggedCount", Strings.hdr_LoggedCount, true),
            new ColumnDescription("AssignedCount", Strings.hdr_AssignedCount, true),
            new ColumnDescription("Target", Strings.Target, true),
            new ColumnDescription("CustomIts1", null, true, (int)CallOutcome.FreshSample),
            new ColumnDescription("CustomIts2", null, true, (int)CallOutcome.Completed),
            new ColumnDescription("CustomIts3", null, false),
            new ColumnDescription("CustomIts4", null, false),
            new ColumnDescription("CustomIts5", null, false),
            new ColumnDescription("TotalTimeToday", Strings.hdr_TimeSpentToday, true),
            new ColumnDescription("TotalTime", Strings.hdr_TimeSpentTotal, true),
            new ColumnDescription("NextAppointment", Strings.hdr_NextAppointment, true),
            new ColumnDescription("ScheduledCallsCount", Strings.hdr_ScheduledCallsCount, true),
            new ColumnDescription("SuspendedCallsCount", Strings.hdr_SuspendedCallsCount, true),
            new ColumnDescription("StrikeRate", Strings.hdr_StrikeRate, true),
            new ColumnDescription("StrikeRate1h", Strings.hdr_StrikeRate1h, false),
            new ColumnDescription("CountCalls", Strings.hdr_CountCalls, true),
            new ColumnDescription("CountCalls1h", Strings.hdr_CountCalls1h, false),
            new ColumnDescription("InterviewDuration", Strings.hdr_InterviewDuration, true),
            new ColumnDescription("InterviewDuration1h", Strings.hdr_InterviewDuration1h, false)
        });

        private string[] _volatileColumns = {"CustomIts1", "CustomIts2", "CustomIts3", "CustomIts4", "CustomIts5" };

        public List<BoundField> GetGridFields()
        {
            var fields = new List<BoundField>();

            foreach (var columnDescription in _mergedColumnDescriptions)
            {
                fields.Add(new BoundField
                {
                    DataField = columnDescription.Key,
                    HeaderText = columnDescription.ColumnText,
                    SortExpression = columnDescription.Key,
                    Visible = columnDescription.IsVisible
                });
            }

            return fields;
        }
        
        public List<GridColumnSetting> GetColumnSettings()
        {
            return _mergedColumnDescriptions.Select(x =>
            {
                Control control;

                if (_volatileColumns.Contains(x.Key))
                {
                    var dropDown = InitItsDropdown();
                    var itsObj = _itsList.FirstOrDefault(its => its.StatusId == x.Value);
                    dropDown.SelectedIndex = itsObj != null ? _itsList.IndexOf(itsObj) : -1;
                    control = dropDown;
                }
                else
                {
                    var labelControl = new Label();
                    labelControl.Text = x.ColumnText;
                    control = labelControl;
                }
                
                return new GridColumnSetting
                {
                    Active = x.IsVisible,
                    SettingControl = control,
                    Key = x.Key
                };
            }).ToList();
        }

        public void SaveColumnSettings(List<GridColumnSetting> settings)
        {
            List<ColumnDescription> savedColumnDescriptions = new List<ColumnDescription>();
            foreach (var gridColumnSetting in settings)
            {
                // columnText will be set during merge, so put null there
                var columnDescription = new ColumnDescription(gridColumnSetting.Key, null, gridColumnSetting.Active);

                if (_volatileColumns.Contains(gridColumnSetting.Key))
                {
                    columnDescription.Value = int.Parse(((ServerControls.DropDownList)gridColumnSetting.SettingControl).SelectedValue);
                }

                savedColumnDescriptions.Add(columnDescription);
            }

            _settingsRepository.WriteSurveyActivityViewColumnSettings(savedColumnDescriptions);
            _savedColumnDescriptions = savedColumnDescriptions;
            _mergedColumnDescriptions = MergeColumnDescriptions(_defaultColumnDescriptions, savedColumnDescriptions);
        }

        public object GetGridData(params object[] searchParams)
        {
            return _activityManager.GetSurveyActivityData(
                (string)searchParams[0],
                (bool)searchParams[1],
                (bool)searchParams[2],
                (IEnumerable<int>)searchParams[3],
                (bool)searchParams[4],
                GetItsSearchParam("CustomIts1"), 
                GetItsSearchParam("CustomIts2"), 
                GetItsSearchParam("CustomIts3"), 
                GetItsSearchParam("CustomIts4"), 
                GetItsSearchParam("CustomIts5"));
        }

        private int GetItsSearchParam(string itsName)
        {
            var itsColumnSetting = _mergedColumnDescriptions.FirstOrDefault(x => x.Key == itsName);
            if (itsColumnSetting != null && itsColumnSetting.IsVisible) return itsColumnSetting.Value;

            return 0;
        }

        private ServerControls.DropDownList InitItsDropdown()
        {
            var dropdown = new ServerControls.DropDownList();
            dropdown.DataSource = _itsList;
            dropdown.DataValueField = "StatusId";
            dropdown.DataTextField = "StatusName";
            dropdown.DataBind();

            return dropdown;
        }

        private List<ColumnDescription> MergeColumnDescriptions(IReadOnlyCollection<ColumnDescription> defaultDescriptions, List<ColumnDescription> overrideDescriptions)
        {
            List<ColumnDescription> mergedColumnDescriptions = new List<ColumnDescription>();

            foreach (var defaultColumnDescription in defaultDescriptions)
            {
                var mergedDescription = new ColumnDescription (defaultColumnDescription.Key, defaultColumnDescription.ColumnText, defaultColumnDescription.IsVisible, defaultColumnDescription.Value);
                var overrideDescription = overrideDescriptions != null ? overrideDescriptions.FirstOrDefault(x => x.Key == defaultColumnDescription.Key) : null;

                // initial column names
                if (defaultColumnDescription.Key == "CustomIts1")
                {
                    mergedDescription.Value = (int)CallOutcome.FreshSample;
                }

                if (defaultColumnDescription.Key == "CustomIts2")
                {
                    mergedDescription.Value = (int)CallOutcome.Completed;
                }

                if (overrideDescription != null)
                {
                    mergedDescription.IsVisible = overrideDescription.IsVisible;
                    mergedDescription.Value = overrideDescription.Value;
                }

                if (_volatileColumns.Contains(mergedDescription.Key))
                {
                    var statusAlertInfo = _itsList.FirstOrDefault(x => x.StatusId == mergedDescription.Value);
                    mergedDescription.ColumnText = statusAlertInfo != null ? statusAlertInfo.StatusName : null;
                }

                mergedColumnDescriptions.Add(mergedDescription);
            }

            return mergedColumnDescriptions;
        }
    }
}