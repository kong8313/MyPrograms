using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Reports
{
    public enum InterviewerBreakReportEvent
    {
        NotDefined = -1,
        Break = 0,
        Login = 1
    }

    public partial class InterviewerSessionsReport : BaseForm
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private readonly ICallCenterProvider _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
        private IEnumerable<int> _selectedInterviewers;

        public override string TopTitle
        {
            get
            {
                return Resources.Strings.InterviewerBreakDetails;
            }
        }

        public IEnumerable<int> SelectedInterviewers
        {
            get
            {
                if (_selectedInterviewers == null)
                {
                    _selectedInterviewers = GetInterviewersSelectedByUser() ??
                                            PersonManager.GetPersonList().Select(x => x.Id).ToArray();
                }

                return _selectedInterviewers;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_Grid.GetPage += GetPage;
            m_Grid.HintText = Resources.Strings.InterviewerBreakDetailsAttentionText;

            BindSearchableHeaders();
            m_Grid.InitializeRow += Grid_InitializeRow;

            btnPersons.OnClientClick = InterviewersSelectionScriptProvider.Get(SourceList.InterviewerBreakReport);
        }

        void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            var dataItem = (InterviewerSessionsReportEntity)e.Row.DataItem;

            InititalizeDurationColumn(e, dataItem);
            InitializeEventColumn(e, dataItem);
        }

        private static void InitializeEventColumn(RowEventArgs e, InterviewerSessionsReportEntity dataItem)
        {
            e.Row.Items.FindItemByKey("Event").Column.Type = typeof(string);
            e.Row.Items.FindItemByKey("Event").Text = ((InterviewerBreakReportEvent)dataItem.Event.Value).ToString();

            if (dataItem.Event == (int) InterviewerBreakReportEvent.Break)
            {
                e.Row.Items.FindItemByKey("Event").Text += $@" ({dataItem.Note})";
            }
        }

        private static void InititalizeDurationColumn(RowEventArgs e, InterviewerSessionsReportEntity dataItem)
        {
            e.Row.Items.FindItemByKey("Duration").Column.Type = typeof(string);

            if (dataItem.Duration.HasValue)
            {
                var duration = TimeSpan.FromSeconds(dataItem.Duration.Value);
                e.Row.Items.FindItemByKey("Duration").Text = String.Format(
                    "{0}:{1}:{2}", ((int)duration.TotalHours).ToString("D2"), (duration.Minutes).ToString("D2"), duration.Seconds.ToString("D2"));
            }
            else
            {
                e.Row.Items.FindItemByKey("Duration").Text = dataItem.Event == (int)InterviewerBreakReportEvent.Break
                    ? "On a break now"
                    : "Logged in now";
            }
        }

        private void BindSearchableHeaders()
        {
            var startTimeColumn = m_Grid.Columns.FromKey("StartTime") as GeneralGridColumn;
            if (startTimeColumn != null)
            {
                // setting default value "Today" for date column
                startTimeColumn.SearchDefaultValue = SearchPredefinedDate.Today.ToString();
            }

            var eventColumn = m_Grid.Columns.FromKey("Event") as ISearchableField;
            if (eventColumn != null)
            {
                var items = Enum.GetValues(typeof(InterviewerBreakReportEvent))
                        .Cast<InterviewerBreakReportEvent>()
                        .Where(x => (int)x >= 0)
                        .Select(x => new ListItem(x.ToString(), ((int)x).ToString(CultureInfo.InvariantCulture)));
                eventColumn.Items.AddRange(items);
            }
        }

        private object GetPage(out int totalCount)
        {
            var searchParameterCollection = PrepareSearchParameters(m_Grid.PageArguments.SearchParameters);

            var args = new PagingArgs(
                            m_Grid.PageIndex,
                            m_Grid.PageSize,
                            m_Grid.SortedColumnKey,
                            m_Grid.SortIndicatorAsc,
                            searchParameterCollection);

            var eventType = args.SearchParameters.FirstOrDefault(x => x.ColumnName == "Event");
            if (eventType != null)
            {
                args.SearchParameters.Remove(eventType);
            }

            var parameters = new InterviewerSessionsReportParams
            {
                Persons = SelectedInterviewers,
                PagingArgs = args,
                CallCenterId = _callCenterProvider.GetCurrentId(),
                TimezoneId = _timezoneProvider.GetLocalTimezoneId(),
                EventType = (int) (eventType == null ? InterviewerBreakReportEvent.NotDefined : eventType.Value),
                CompanyId = BackendInstance.Current.CompanyId
            };

            return ReportManager.GetInterviewerSessions(parameters, out totalCount);
        }

        private IEnumerable<int> GetInterviewersSelectedByUser()
        {
            if (ReportsSessionVariables.InterviewerSessionsReportSelectedInterviewersIds != null &&
                ReportsSessionVariables.InterviewerSessionsReportSelectedInterviewersIds.Any())
            {
                return ReportsSessionVariables.InterviewerSessionsReportSelectedInterviewersIds;
            }

            return null;
        }

        private SearchParameterCollection PrepareSearchParameters(SearchParameterCollection searchParameterCollection)
        {
            var parameter = searchParameterCollection.FirstOrDefault(x => x.ColumnName == "Duration");

            if (parameter != null)
            {
                parameter.Value = ((int)(parameter.Value)) * 60;
            }

            return searchParameterCollection;
        }
    }
}
