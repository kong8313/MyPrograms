using System;
using System.Data;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Surveys
{
    /// <summary>
    /// Page shows active calls distribution list
    /// </summary>
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class CallsSentToDialerDistribution : SurveyFormBase
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private readonly ISurveyCallDistributionService _surveyService = ServiceLocator.Resolve<ISurveyCallDistributionService>();
        private const int DatesRangeForReport = 14;
        private DateTime _startTime;
        private DateTime _endTime;

        public override string Title
        {
            get { return Strings.CallsSentToDialerDistribution; }
        }

        /// <summary>
        /// Gets current survey Id
        /// </summary>
        [StoreInViewState]
        public int SurveyId;

        /// <summary>
        /// Gets/sets selected day in string representation      
        /// </summary>
        /// <remarks>
        /// Stores selected value from dropdown with days
        /// </remarks>
        [StoreInViewState]
        public string SelectedDay;

        /// <summary>
        /// Gets/sets flag indicated is last 20 minutes option checked or not
        /// </summary>
        /// <remarks>
        /// Stores checked state of cbSetDefaultTime control
        /// </remarks>
        [StoreInViewState]
        public bool IsLast20MinutesChecked;

        /// <summary>
        /// Gets/sets time of day
        /// </summary>
        /// <remarks>
        /// Stores value of wdteTime control
        /// </remarks>
        [StoreInViewState]
        public DateTime SelectedTimeOfDay;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                InitDaysDropDown();

                SaveDateSelectionControlsState();

                if (Request["ID"] != null)
                {
                    SurveyId = Int32.Parse(Request["ID"]);
                }
            }

            m_grid.GetPage += delegate(out int totalCount)
            {
                var distributionTable =_surveyService.GetCallsSentToDialerDistribution(
                    SurveyId,
                    GetSelectedDateTime(),
                    _timezoneProvider.GetLocalTimezoneId(),
                    out totalCount
                    );
                if (distributionTable.Rows.Count > 0)
                {
                    _startTime = DateTime.Parse(distributionTable.Columns[1].ColumnName);
                    _endTime = DateTime.Parse(distributionTable.Columns[distributionTable.Columns.Count - 1].ColumnName);
                    DispositionTable.InnerText = String.Format(Strings.DispositionsTableForPeriod, _startTime.ToString("G"), _endTime.ToString("G"));
                }
                else
                {
                    DispositionTable.InnerText = "";
                    _startTime = _endTime = DateTime.MaxValue;
                }

                DialersRequests.InnerText = Strings.DialersRequests;
                TotalCallsSentToDialer.InnerText = String.Format(Strings.TotalCallsSentToDialerDuringPeriod, totalCount);
                return distributionTable;
            };

            m_gridDialerCalls.GetPage +=
                delegate(out int totalCount)
                {
                    CallsBreakdownInDialerCache.InnerText = Strings.CallsBreakdownInDialerCache;
                    var dialerCalls = _surveyService.GetDialerCallsBreakdown(SurveyId, out totalCount);
                    TotalCallsInDialerCache.InnerText = String.Format(Strings.TotalCallsInDialerCache, totalCount);
                    return dialerCalls;
                };


            m_gridIts.GetPage +=
                delegate(out int totalCount)
                {
                    var processedCalls = _surveyService.GetCallsDispositionCodes(SurveyId, _timezoneProvider.ConvertToUtc(_startTime), _timezoneProvider.ConvertToUtc(_endTime), out totalCount);
                    TotalProcessedCalls.InnerText = String.Format(Strings.TotalProcessedCalls, totalCount);
                    return processedCalls;
                };
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            lblTime.Text = _timezoneProvider.GetCurrentLocalTime().ToString("g");
            lblSelectedTime.Text = GetSelectedDateTimeText();
        }

        protected void btnSetTime_Click(object sender, EventArgs e)
        {
            SaveDateSelectionControlsState();
            RefreshGrids();
        }

        protected void btnCancelSetTime_Click(object sender, EventArgs e)
        {
            RestoreDateSelectionControlsState();
        }

        protected void Menu_UpdateClick(object sender, EventArgs e)
        {
            RefreshGrids();
        }

        private void InitDaysDropDown()
        {
            DateTime currentDate = _timezoneProvider.GetCurrentLocalTime();

            for (int i = 0; i > -DatesRangeForReport; i--)
            {
                DateTime date = currentDate.AddDays(i);
                ddlDays.Items.Add(new ListItem(date.ToShortDateString(), date.ToShortDateString()));
            }
        }

        /// <summary>
        /// Gets the selected in the toolbar (but probably not yet saved) start DateTime
        /// for call distribution period in UTC format.
        /// </summary>
        private DateTime? GetSelectedDateTime()
        {
            DateTime? startDate;

            if (IsLast20MinutesChecked)
            {
                startDate = null;
            }
            else
            {
                startDate = DateTime.Parse(SelectedDay);
                startDate = startDate.Value.Add(SelectedTimeOfDay.TimeOfDay);
                startDate = _timezoneProvider.ConvertToUtc(startDate.Value);
            }

            return startDate;
        }

        /// <summary>
        /// Gets the selected in the toolbar (but probably not yet saved) start DateTime
        /// for call distribution period in UTC format as text to show in the toolbar.
        /// </summary>
        private string GetSelectedDateTimeText()
        {
            if (IsLast20MinutesChecked)
            {
                return Strings.Last20Times;
            }

            DateTime selectedDate = DateTime.Parse(SelectedDay);
            selectedDate = selectedDate.Add(SelectedTimeOfDay.TimeOfDay);

            return String.Format(Strings.Last20TimesSinceTime, selectedDate.ToString("g"));
        }

        /// <summary>
        /// Saves state of time selection controls into corresponding properties
        /// </summary>
        private void SaveDateSelectionControlsState()
        {
            SelectedDay = ddlDays.SelectedValue;
            SelectedTimeOfDay = dteTime.Date;
            IsLast20MinutesChecked = cbSetDefaultTime.Checked;
        }

        /// <summary>
        /// Restore state of time selection controls from corresponding properties
        /// </summary>
        private void RestoreDateSelectionControlsState()
        {
            ddlDays.SelectedValue = SelectedDay;
            dteTime.Date = SelectedTimeOfDay;
            cbSetDefaultTime.Checked = IsLast20MinutesChecked;
            RefreshGrids();
        }

        private void RefreshGrids()
        {
            m_grid.RefreshData();
            m_gridIts.RefreshData();
            m_gridDialerCalls.RefreshData();
        }

        protected void m_grid_RowHeaderDataBound(object sender, GridViewRowEventArgs e)
        {
            for (int cellIndex = 2; cellIndex < e.Row.Cells.Count; cellIndex++)
            {
                var cell = (DataControlFieldCell)e.Row.Cells[cellIndex];
                cell.Text = DateTime.Parse(cell.ContainingField.HeaderText).ToString("HH:mm:ss");
            }
        }
    }
}