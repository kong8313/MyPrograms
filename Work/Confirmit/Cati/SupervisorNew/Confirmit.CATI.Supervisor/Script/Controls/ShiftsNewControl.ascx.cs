using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Script.Classes;
using Infragistics.Web.UI.GridControls;
using Confirmit.CATI.Supervisor.Classes.Script;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;

namespace Confirmit.CATI.Supervisor.Script.Controls
{
    public partial class ShiftsNewControl : ScheduleControlBase
    {
        protected ShiftCollection ShiftCollection => WorkingSchedule.Shifts;

        protected ExclusionCollection ExclusionCollection => WorkingSchedule.Exclusions;

        protected ShiftTypeCollection ShiftTypeCollection => WorkingSchedule.ShiftTypes;

        protected BvTimezoneEntityCollection UsedTimeZones
        {
            get
            {
                if (ViewState["UsedTimeZones"] == null)
                {
                    ViewState["UsedTimeZones"] = GetUsedTimezones();
                }

                return (BvTimezoneEntityCollection)ViewState["UsedTimeZones"];
            }
        }

        /// <summary>
        /// Currently selected time zone.
        /// Note that value is stored into Session because it is used in ShiftOutlook control.
        /// </summary>
        private int SelectedTimeZone
        {
            get => (int)(Session[$"SelectedTimeZone_{ScheduleId}"] ?? Shift.RespondentTimezoneId);
            set => Session[$"SelectedTimeZone_{ScheduleId}"] = value;
        }

        /// <summary>
        /// Currently display state: Shifts/Exclusions/Both
        /// </summary>
        public CurrentDisplay CurrentDisplay
        {
            get => (CurrentDisplay)(Session[$"CurrentDisplay_{ScheduleId}"] ?? CurrentDisplay.Shifts);
            set => Session[$"CurrentDisplay_{ScheduleId}"] = value;
        }

        protected override string ClientControllerName => "shiftsController";

        [StoreInViewState]
        protected bool IsTimezonesBound;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            EnableCommands();

            if (!IsTimezonesBound)
            {
                BindTimeZones();
                IsTimezonesBound = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SelectedTimeZone = (String.IsNullOrEmpty(ddlUsedTimeZones.SelectedValue) == false) ?
                Int32.Parse(ddlUsedTimeZones.SelectedValue) :
                Shift.RespondentTimezoneId;

            if (ddlShowShifts.SelectedIndex == 0)
            {
                CurrentDisplay = CurrentDisplay.Shifts;
                m_grid.GetPage += Shifts_GetPage;
            }
            else if (ddlShowShifts.SelectedIndex == 1)
            {
                CurrentDisplay = CurrentDisplay.Exclusions;
                m_grid.GetPage += Exclusions_GetPage;
            }
            else
            {
                CurrentDisplay = CurrentDisplay.Both;
                m_grid.GetPage += Both_GetPage;
                m_grid.OnDblClickCommand = string.Empty;
            }

            StateChecker.AddSaveButton(btnSave);
            m_grid.InitializeRow += m_grid_InitializeRow;

            if (IsPostBack && !string.IsNullOrWhiteSpace(viewToggleValue.Value) && viewToggleValue.Value != "false")
            {
                Page.RegisterStartupScript("toggleViews();");
            }
        }

        protected void OnChange(object sender, EventArgs eventArgs)
        {
            Page.RegisterStartupScript("Common.fireGlobalEvent('ScriptViewScheduleShiftChanged');");
            ScheduleChangedHandler(this, eventArgs);
        }

        protected void SetDefault(object sender, EventArgs e)
        {
            if (SelectedTimeZone != Shift.RespondentTimezoneId)
            {
                var id = m_grid.SelectedKeysInt.First();

                if (CurrentDisplay == CurrentDisplay.Shifts)
                {
                    //delete info for current selected TZ
                    var shift = ShiftCollection.GetItemById(id);

                    if (shift.HasTimezone(Shift.RespondentTimezoneId))
                    {
                        shift.RemoveDataForTimezone(SelectedTimeZone);
                    }
                    else
                    {
                        NotifyUser(Strings.SelectedShiftHasntRespondentTimeZone);
                    }
                }
                else if (CurrentDisplay == CurrentDisplay.Exclusions)
                {
                    //delete info for current selected TZ
                    var exclusion = ExclusionCollection.GetItemById(id);

                    if (exclusion.HasTimezone(Exclusion.RespondentTimezoneId))
                    {
                        exclusion.RemoveDataForTimezone(SelectedTimeZone);
                    }
                    else
                    {
                        NotifyUser(Strings.SelectedShiftHasntRespondentTimeZone);
                    }
                }
            }
            OnChange(sender, e);
        }

        protected void Delete(object sender, EventArgs e)
        {
            var id = m_grid.SelectedKeysInt.First();
            ErrorCollection errors = null;

            switch (CurrentDisplay)
            {
                case CurrentDisplay.Shifts:
                    ShiftCollection.RemoveById(id, out errors);
                    break;
                case CurrentDisplay.Exclusions:
                    ExclusionCollection.RemoveById(id, out errors);
                    break;
            }

            if (errors != null && errors.Count > 0)
            {
                NotifyUser(errors);
            }

            OnChange(sender, e);
        }

        protected void AddNewTimeZone(object sender, EventArgs e)
        {
            foreach (ListItem item in ddlAvailableTimeZones.Items)
            {
                if (item.Selected)
                {
                    UsedTimeZones.Add(TimezoneManager.GetTimezoneByID(Int32.Parse(item.Value)));
                }
            }
            BindTimeZones();
        }

        private BvTimezoneEntityCollection GetUsedTimezones()
        {
            var usedTimezones = new BvTimezoneEntityCollection();
            var allTimeZones = TimezoneManager.ActiveTimezonesList;
            var usedTimezoneIds = WorkingSchedule.GetUsedTimezoneIds();

            foreach (var timeZone in allTimeZones)
            {
                if (Array.IndexOf(usedTimezoneIds, timeZone.ID) >= 0)
                {
                    usedTimezones.Add(timeZone);
                }
            }
            return usedTimezones;
        }

        /// <summary>
        /// Tune toolbar according selected TZ
        /// </summary>
        private void EnableCommands()
        {
            bool disabled = (CurrentDisplay == CurrentDisplay.Both);
            if (disabled)
            {
                m_grid.DisableCommand("New");
                m_grid.DisableCommand("Edit");
                m_grid.DisableCommand("Delete");
                m_grid.DisableCommand("SetDefault");
            }

            if (CurrentDisplay != CurrentDisplay.Both)
            {
                disabled = (SelectedTimeZone == Shift.RespondentTimezoneId);
                if (disabled)
                    m_grid.DisableCommand("SetDefault");
            }
        }

        private object Shifts_GetPage(out int totalCount)
        {
            return ScheduleManager.GetShiftsByTimezone(SelectedTimeZone, ShiftCollection, out totalCount);
        }

        private object Exclusions_GetPage(out int totalCount)
        {
            return ScheduleManager.GetExclusionsByTimezone(SelectedTimeZone, ExclusionCollection, out totalCount);
        }

        private object Both_GetPage(out int totalCount)
        {
            totalCount = 0;
            return ScheduleManager.GetBothByTimezone(SelectedTimeZone, ShiftCollection, ExclusionCollection);
        }

        /// <summary>
        /// Bind time zones for drop-down list
        /// </summary>
        private void BindTimeZones()
        {
            ddlUsedTimeZones.Items.Clear();
            ddlAvailableTimeZones.Items.Clear();
            //need clear previous value
            SelectedTimeZone = Shift.RespondentTimezoneId;

            var allTimeZones = TimezoneManager.ActiveTimezonesList.OrderBy(x => x.Name);
            ddlUsedTimeZones.Items.Add(new ListItem(GetResString("Respondent timezone"), Shift.RespondentTimezoneId.ToString()));

            foreach (var timeZone in allTimeZones)
            {
                if (UsedTimeZones.Contains(timeZone))
                {
                    ddlUsedTimeZones.Items.Add(new ListItem(timeZone.Name, timeZone.ID.ToString()));
                }
                else
                {
                    ddlAvailableTimeZones.Items.Add(new ListItem(timeZone.Name, timeZone.ID.ToString()));
                }
            }
        }

        void m_grid_InitializeRow(object sender, RowEventArgs e)
        {
            var shiftTypeId = (int)e.Row.Items.FindItemByKey("ShiftTypeId").Value;

            var shiftType = ShiftTypeCollection.GetItemById(shiftTypeId);

            if (shiftType != null)
            {
                e.Row.Items.FindItemByKey("ShiftTypeName").Value = shiftType.Name;
            }
        }

        /// <summary>
        /// Adds confirmation while Launch button click.
        /// </summary>
        public override void AddConfirmationWhileLaunch()
        {
            m_grid.Commands.First(x => x.Key == "Launch").Confirmation =
                Strings.LaunchScriptConfirmation;
        }
    }
}
