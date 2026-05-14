using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Script;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Script
{
    public partial class ScriptShiftProperties : BaseForm
    {
        private readonly ISchedulingObjectValidator _validator;

        public ScriptShiftProperties()
        {
            _validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
        }

        public CurrentDisplay DisplayType
        {
            get { return (CurrentDisplay)(Session[$"CurrentDisplay_{ScheduleId}"] ?? CurrentDisplay.Shifts); }
        }

        public Schedule WorkingSchedule
        {
            get { return (Schedule)Session[$"WorkingSchedule_{ScheduleId}"]; }
        }

        public ShiftCollection ShiftCollection
        {
            get { return WorkingSchedule.Shifts; }
        }

        public ExclusionCollection ExclusionCollection
        {
            get { return WorkingSchedule.Exclusions; }
        }

        protected ShiftTypeCollection ShiftTypeCollection
        {
            get { return WorkingSchedule.ShiftTypes; }
        }

        public int SelectedTimeZoneId
        {
            get
            {
                object obj = Session[$"SelectedTimeZone_{ScheduleId}"];
                if (obj != null)
                {
                    return (int)obj;
                }
                return Shift.RespondentTimezoneId;
            }
        }

        protected bool IsNew
        {
            get { return !ShiftId.HasValue; }
        }        

        private const string TimeFormatForDateTime = "HH\\:mm";

        private const string TimeFormatForTimeSpan = "hh\\:mm";

        [StoreInViewState]
        protected int? ShiftId;

        [StoreInViewState]
        protected int? ScheduleId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["ID"] != null)
            {
                ScheduleId = int.Parse(Request["ID"]);
            }

            if (SelectedTimeZoneId != Shift.RespondentTimezoneId)
            {
                hfConfimOverride.Value = GetResString("DoYouWantOverrideShift");
                hfConfimNewNotDefault.Value = Strings.DoYouWantAddShiftOnlyCurrentTimezone;
            }
            else
            {
                hfConfimOverride.Value = String.Empty;
                hfConfimNewNotDefault.Value = String.Empty;
            }

            hfTemplateType.Value = ((int)DisplayType).ToString();


            if (IsPostBack == false)
            {
                BindShiftTypeIdsList();

                if (Request["ShiftId"] != null)
                {
                    ShiftId = Int32.Parse(Request["ShiftId"]);                    
                }

                if (IsNew == false)
                {
                    BindData();
                }
            }

            dialog.OKButton.Text = IsNew ? "Add" : "Save";
        }

        private void BindData()
        {
            try
            {
                hfRowId.Value = ShiftId.ToString();

                if (DisplayType == CurrentDisplay.Shifts)
                {
                    var shift = ShiftCollection.GetItemById(ShiftId.Value);
                    ddlShiftType.SelectedValue = shift.ShiftTypeId.ToString();

                    ShiftData data;
                    shift.TryGetDataForTimezone(SelectedTimeZoneId, out data);

                    ddlStartDay.SelectedValue = data.StartDayOfWeek.Value.ToString();
                    ddlEndDay.SelectedValue = data.EndDayOfWeek.Value.ToString();
                    tbStartTime.Text = data.StartTime.Value.ToString(TimeFormatForTimeSpan);
                    tbEndTime.Text = data.EndTime.Value.ToString(TimeFormatForTimeSpan);
                    
                    hfHasRespondentTimeZone.Value = shift.HasTimezone(Shift.RespondentTimezoneId).ToString();
                }
                else if (DisplayType == CurrentDisplay.Exclusions)
                {
                    var exclusion = ExclusionCollection.GetItemById(ShiftId.Value);
                    ddlShiftType.SelectedValue = exclusion.ShiftTypeId.ToString();

                    ExclusionData data;
                    exclusion.TryGetDataForTimezone(SelectedTimeZoneId, out data);

                    wdteStartDate.Date = data.StartDate.Value;
                    wdteEndDate.Date = data.EndDate.Value;
                    tbStartTime.Text = data.StartDate.Value.ToString(TimeFormatForDateTime);
                    tbEndTime.Text = data.EndDate.Value.ToString(TimeFormatForDateTime);

                    hfHasRespondentTimeZone.Value = exclusion.HasTimezone(Shift.RespondentTimezoneId).ToString();
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (DisplayType == CurrentDisplay.Shifts)
            {
                phShiftsStartDay.Visible = true;
                phShiftsEndDay.Visible = true;
                phExclusiveStartDay.Visible = false;
                phExclusiveEndDay.Visible = false;
            }
            else if (DisplayType == CurrentDisplay.Exclusions)
            {
                phExclusiveStartDay.Visible = true;
                phExclusiveEndDay.Visible = true;
                phShiftsStartDay.Visible = false;
                phShiftsEndDay.Visible = false;
            }
        }

        private void BindShiftTypeIdsList()
        {
            ddlShiftType.Items.Clear();

            foreach (var shiftType in ShiftTypeCollection)
            {
                if ((shiftType.IsExclusionType && DisplayType == CurrentDisplay.Exclusions) ||
                    (shiftType.IsExclusionType == false && DisplayType == CurrentDisplay.Shifts))
                {
                    var li = new ListItem(shiftType.Name, shiftType.Id.Value.ToString());
                    ddlShiftType.Items.Add(li);
                }
            }
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            if (DisplayType == CurrentDisplay.Shifts)
            {
                SaveShift();
            }
            else if (DisplayType == CurrentDisplay.Exclusions)
            {
                SaveExclusion();
            }
        }

        private void SaveShift()
        {
            try
            {
                var shiftData = new ShiftData();

                DayOfWeek day;
                DayOfWeek.TryParse(ddlStartDay.SelectedValue, out day);
                shiftData.StartDayOfWeek = day;

                DayOfWeek.TryParse(ddlEndDay.SelectedValue, out day);
                shiftData.EndDayOfWeek = day;

                shiftData.StartTime = TimeSpan.Parse(tbStartTime.Text);
                shiftData.EndTime = TimeSpan.Parse(tbEndTime.Text);

                Shift shift;
                if (IsNew == false)
                {
                    shift = (Shift)ShiftCollection.GetItemById(ShiftId.Value).Clone();
                }
                else
                {
                    shift = new Shift { Id = ShiftCollection.GetNewId() };
                }

                shift.ShiftTypeId = int.Parse(ddlShiftType.SelectedValue);
                shift.SetDataForTimezone(SelectedTimeZoneId, shiftData);

                ErrorCollection errors;
                ErrorCollection collectionBasedErrors = null;
                if (_validator.Validate(shift, out errors) && _validator.ValidateWithCollection(ShiftCollection,shift,out collectionBasedErrors))
                {
                    if (IsNew)
                    {
                        ShiftCollection.Add(shift);
                    }
                    else
                    {
                        int index = ShiftCollection.IndexOf(ShiftCollection.GetItemById(shift.Id.Value));
                        ShiftCollection[index] = shift;
                    }

                    CloseOverlay(true);
                }
                else
                {
                    if (collectionBasedErrors != null)
                    {
                        errors.AddRange(collectionBasedErrors);    
                    }

                    ShowClientMessage(errors[0].Message);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void SaveExclusion()
        {
            try
            {
                Exclusion exclusion;
                ErrorCollection errors;
                ErrorCollection collectionRelatedErrors = null;

                var exclusionData =
                    new ExclusionData(new DateTime(wdteStartDate.Date.Ticks + TimeSpan.Parse(tbStartTime.Text).Ticks),
                        new DateTime(wdteEndDate.Date.Ticks + TimeSpan.Parse(tbEndTime.Text).Ticks));

                if (IsNew == false)
                {
                    exclusion = (Exclusion)ExclusionCollection.GetItemById(ShiftId.Value).Clone();
                }
                else
                {
                    exclusion = new Exclusion { Id = ExclusionCollection.GetNewId() };
                }

                exclusion.ShiftTypeId = int.Parse(ddlShiftType.SelectedValue);
                exclusion.SetDataForTimezone(SelectedTimeZoneId, exclusionData);

                if (_validator.Validate(exclusion, out errors) && _validator.ValidateWithCollection(ExclusionCollection,exclusion, out collectionRelatedErrors))
                {
                    if (IsNew)
                    {
                        ExclusionCollection.Add(exclusion);
                    }
                    else
                    {
                        int index = ExclusionCollection.IndexOf(ExclusionCollection.GetItemById(exclusion.Id.Value));
                        ExclusionCollection[index] = exclusion;
                    }

                    CloseOverlay(true);
                }
                else
                {
                    if (collectionRelatedErrors != null)
                    {
                        errors.AddRange(collectionRelatedErrors);    
                    }

                    ShowClientMessage(errors[0].Message);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}