using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;

using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Script.Classes;
using Infragistics.WebUI.WebSchedule;
using Infragistics.WebUI.Shared;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Script;

namespace Confirmit.CATI.Supervisor.Script.ShiftOutlook
{
    public partial class ShiftOutlook : BaseWUC
    {
        protected override void OnLoad(EventArgs e)
        {
            Page.RegisterStyleSheet("styles/WebCalendar.css");

            base.OnLoad(e);
            WebdayView1.NextButtonImage.Url = "~/svgimages/arrow_forward.svg";
            WebdayView1.PrevButtonImage.Url = "~/svgimages/arrow_back.svg";
            WebdayView1.NextButtonImage.AlternateText = Strings.NextWeek;
            WebdayView1.PrevButtonImage.AlternateText = Strings.PrevWeek;
        }

        public int SelectedTimeZone
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

        public CurrentDisplay CurrentDisplay
        {
            get
            {
                object obj = Session[$"CurrentDisplay_{ScheduleId}"];
                if (obj != null)
                {
                    return (CurrentDisplay)obj;
                }
                return CurrentDisplay.Both;
            }
        }

        public Schedule WorkingSchedule
        {
            get
            {
                return (Schedule)Session[$"WorkingSchedule_{ScheduleId}"];
            }
        }

        protected ShiftCollection ShiftCollection
        {
            get
            {
                return WorkingSchedule.Shifts;
            }
        }

        protected ExclusionCollection ExclusionCollection
        {
            get
            {
                return WorkingSchedule.Exclusions;
            }
        }

        protected ShiftTypeCollection ShiftTypeCollection
        {
            get
            {
                return WorkingSchedule.ShiftTypes;
            }
        }

        protected DateTime FirstWeekDay
        {
            get
            {
                return WebScheduleInfo1.ConvertUtcToTimeZoneTime(WebScheduleInfo1.ActiveDayUtc).Value;
            }
        }

        [StoreInViewState]
        protected int ScheduleId;

        public string CssClass { get; set; }

        protected void Page_Load( object sender, EventArgs e )
        {
            if (Request["ID"] != null)
            {
                ScheduleId = int.Parse(Request["ID"]);
            }

            if (IsPostBack == false)
            {
                DateTime firstWeekDay = DateTime.Now.AddDays((int)DateTime.Now.DayOfWeek * (-1) + 1);
                firstWeekDay = firstWeekDay.AddTicks(firstWeekDay.TimeOfDay.Ticks * (-1));

                WebScheduleInfo1.FirstDayOfWeek = FirstDayOfWeek.Monday;
                WebScheduleInfo1.ActiveDayUtc = WebScheduleInfo1.ConvertTimeZoneTimeToUtc(new SmartDate(firstWeekDay));
            }
        }

        protected void Page_PreRender( object sender, EventArgs e )
        {
            this.InitView();
        }

        #region InitView
        private void InitView()
        {
            int totalCount;

            var theCustomProvider = new OutlookProvider(
                ScheduleManager.GetShiftsByTimezone(SelectedTimeZone, ShiftCollection, out totalCount),
                ScheduleManager.GetExclusionsByTimezone(SelectedTimeZone, ExclusionCollection, out totalCount),
                ShiftTypeCollection,
                FirstWeekDay,
                DayOfWeek.Monday,
                CurrentDisplay
                ) {WebScheduleInfo = WebScheduleInfo1};

            var dayViewOnTab1 = WebdayView1;
            dayViewOnTab1.WebScheduleInfoID = WebScheduleInfo1.ID;

            WebdayView1.ClientEvents.Initialize = "resizeScript";            

            ApplyAppointmentsColors(theCustomProvider.GetAppointments());            
        }

        /// <summary>
        /// Makes appointments's backcolors to be displayed correctly in case Outlook control is located inside UpdatePanel.        
        /// </summary>
        private void ApplyAppointmentsColors(IEnumerable<Appointment> appointments)
        {
            var stringBuilder = new StringBuilder();

            foreach (var appointment in appointments)
            {
                var color = appointment.Style.BackColor;
                var colorString = string.Format("rgb({0}, {1}, {2})", color.R, color.G, color.B);
                var className = String.Format("backcolor_{0}", appointment.Key);
                var generatedClass = string.Format(".{0}{{background-color: {1} !important;}}", className, colorString);
                stringBuilder.Append(generatedClass);
                appointment.Style.CssClass = className;
            }

            Page.RegisterStartupScript(string.Format(@"try{{
                                                       var styleElement = document.createElement('style');
                                                       styleElement.setAttribute(""type"", ""text/css"");
                                                       styleElement.styleSheet.cssText = '{0}';
                                                       var headElement = document.getElementsByTagName('head')[0];
                                                       headElement.appendChild(styleElement);
                                                          }}catch(ex){{}};", 
                                                     stringBuilder));
        }

        #endregion
    }
}