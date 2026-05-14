using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class DateTimeRangeSelect : BaseWUC
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeRangeSelect"/> class.
        /// </summary>
        public DateTimeRangeSelect()
        {
            _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        }

        # region Properties

        /// <summary>
        /// Gets the current time in local timezone.
        /// </summary>
        protected DateTime Now
        {
            get
            {
                return _timezoneProvider.ConvertToLocalTime(DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Flag indicates, is datetime range manually changed or not
        /// </summary>
        private bool m_RangeChanged;

        /// <summary>
        /// Flag indicates, is datetime value already saved or not
        /// </summary>
        private bool m_Saved;

        /// <summary>
        /// Event occurs in case of dropdownlist's selection changes, or if one of dates changes.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Gets or sets autopostback property of the user control.
        /// </summary>
        public bool AutoPostBack
        {
            get
            {
                return ddlFilter.AutoPostBack;
            }
            set
            {
                ddlFilter.AutoPostBack = value;
                bttnOK.IsSubmit = value;
            }
        }

        /// <summary>
        /// Gets or sets begin datetime in local timezone.
        /// </summary>
        public DateTime BeginDateTime
        {
            get
            {
                if (!AutoPostBack)
                    SaveDateTime();
                return ViewState["BeginDateTime"] == null ? Now : (DateTime)ViewState["BeginDateTime"];
            }
            set
            {
                ViewState["BeginDateTime"] = value;
            }
        }

        /// <summary>
        /// Gets or sets end datetime in local timezone.
        /// </summary>
        public DateTime EndDateTime
        {
            get
            {
                if (!AutoPostBack)
                    SaveDateTime();
                return ViewState["EndDateTime"] == null ? Now : (DateTime)ViewState["EndDateTime"];
            }
            set
            {
                ViewState["EndDateTime"] = value;
            }
        }

        /// <summary>
        /// Gets or sets begin datetime in UTC.
        /// </summary>
        public DateTime BeginDateTimeUtc
        {
            get
            {
                return _timezoneProvider.ConvertToUtc(BeginDateTime);
            }
        }

        /// <summary>
        /// Gets or sets end datetime in UTC.
        /// </summary>
        public DateTime EndDateTimeUtc
        {
            get
            {
                return _timezoneProvider.ConvertToUtc(EndDateTime);
            }
        }

        /// <summary>
        /// Gets or sets 'Enabled' property of the user control.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return ddlFilter.Enabled;
            }
            set
            {
                ddlFilter.Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets allowed intervals that will be displayed in the control's dropdownlist.
        /// </summary>
        public DateTimeRange RangeIntervals
        {
            get
            {
                if (ViewState["RangeIntervals"] == null)
                {
                    return DateTimeRange.All;
                }
                else
                    return (DateTimeRange)ViewState["RangeIntervals"];
            }
            set
            {
                ViewState["RangeIntervals"] = value;
            }
        }

        /// <summary>
        /// Defines, which interval will be selected by default
        /// </summary>
        public DateTimeRange SelectedInterval
        {
            get
            {
                return ViewState["SelectedInterval"] == null ? DateTimeRange.Today : (DateTimeRange)ViewState["SelectedInterval"];
            }
            set
            {
                ViewState["SelectedInterval"] = value;
            }
        }

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                foreach (DateTimeRange interval in Enum.GetValues(typeof (DateTimeRange)))
                {
                    if ((int) (interval & RangeIntervals) != 0 && interval != DateTimeRange.All)
                    {
                        ddlFilter.Items.Add(new ListItem(StringHelper.GetStringFromEnum(interval), ((int)interval).ToString()));
                        if (interval == SelectedInterval)
                        {
                            ddlFilter.SelectedValue = ((int) interval).ToString();
                        }
                    }
                }

                ddlFilter.Items.Add(new ListItem(Strings.Range, "0"));
                SaveDateTime();
            }


            dteStart.ValueChanged +=
                delegate(object s, EventArgs ev)
                {
                    if (Changed != null)
                        m_RangeChanged = true;

                };
            dteEnd.ValueChanged +=
                delegate(object s, EventArgs ev)
                {
                    if (Changed != null)
                        m_RangeChanged = true;
                };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                m_Saved = false;
                SaveDateTime();
                FillDateTimeEdits();
            }
        }

        /// <summary>
        /// Saves datetime value of control to ViewState
        /// </summary>
        protected void SaveDateTime()
        {
            if (m_Saved)
                return;
            switch ((DateTimeRange)Convert.ToInt32(ddlFilter.SelectedValue))
            {
                case DateTimeRange.Range:
                    BeginDateTime = dteStart.DateTimeValue;
                    EndDateTime = dteEnd.DateTimeValue;
                    break;
                case DateTimeRange.Today:
                    BeginDateTime = Now.Date;
                    EndDateTime = Now.Date.AddDays(1).AddSeconds(-1);
                    break;
                case DateTimeRange.Last2Hrs:
                    BeginDateTime = Now.AddHours(-2);
                    EndDateTime = Now;
                    break;
                case DateTimeRange.Last4Hrs:
                    BeginDateTime = Now.AddHours(-4);
                    EndDateTime = Now;
                    break;
                case DateTimeRange.Last2Days:
                    BeginDateTime = Now.Date.AddDays(-1);
                    EndDateTime = Now.Date.AddDays(1).AddSeconds(-1);
                    break;
                case DateTimeRange.TodayMinus1:
                    BeginDateTime = Now.Date.AddDays(-1);
                    EndDateTime = Now.Date.AddSeconds(-1);
                    break;
                case DateTimeRange.TodayMinus2:
                    BeginDateTime = Now.Date.AddDays(-2);
                    EndDateTime = Now.Date.AddDays(-1).AddSeconds(-1);
                    break;
                case DateTimeRange.TodayMinus3:
                    BeginDateTime = Now.Date.AddDays(-3);
                    EndDateTime = Now.Date.AddDays(-2).AddSeconds(-1);
                    break;
                case DateTimeRange.TodayMinus4:
                    BeginDateTime = Now.Date.AddDays(-4);
                    EndDateTime = Now.Date.AddDays(-3).AddSeconds(-1);
                    break;
                case DateTimeRange.TodayMinus5:
                    BeginDateTime = Now.Date.AddDays(-5);
                    EndDateTime = Now.Date.AddDays(-4).AddSeconds(-1);
                    break;
                case DateTimeRange.TodayMinus6:
                    BeginDateTime = Now.Date.AddDays(-6);
                    EndDateTime = Now.Date.AddDays(-5).AddSeconds(-1);
                    break;
                case DateTimeRange.TodayMinus7:
                    BeginDateTime = Now.Date.AddDays(-7);
                    EndDateTime = Now.Date.AddDays(-6).AddSeconds(-1);
                    break;
                case DateTimeRange.ThisWeek:
                    int daysLeft = Convert.ToInt32(Now.DayOfWeek) > 0 ? 1 - Convert.ToInt32(Now.DayOfWeek) : -6;
                    BeginDateTime = Now.Date.AddDays(daysLeft);
                    EndDateTime = Now.Date.AddDays(7 + daysLeft).AddSeconds(-1);
                    break;
                case DateTimeRange.ThisMonth:
                    BeginDateTime = Now.Date.AddDays(1 - Now.Day);
                    EndDateTime = Now.Date.AddDays(1 - Now.Day).AddMonths(1).AddSeconds(-1);
                    break;
                case DateTimeRange.ThisYear:
                    BeginDateTime = new DateTime(Now.Year, 1, 1);
                    EndDateTime = (new DateTime(Now.Year + 1, 1, 1)).AddSeconds(-1);
                    break;
                default:
                    {
                        throw new NotSupportedException(string.Format(Strings.IntervalNotSupported, (DateTimeRange)Convert.ToInt32(ddlFilter.SelectedValue)));
                    }
            }
            m_Saved = true;
        }

        /// <summary>
        /// Fills datetime editors with datetime value, previously saved in viewstate
        /// </summary>
        protected void FillDateTimeEdits()
        {
            dteStart.DateTimeValue = BeginDateTime;
            dteEnd.DateTimeValue = EndDateTime;
        }

        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((DateTimeRange)Convert.ToInt32(ddlFilter.SelectedValue) != DateTimeRange.Range)
            {
                SaveDateTime();
                FillDateTimeEdits();
                if (Changed != null)
                    Changed(this, EventArgs.Empty);
            }
            else
            {
                FillDateTimeEdits();
            }
        }

        protected void bttnOK_Click(object sender, EventArgs e)
        {
            if (dteStart.DateTimeValue > dteEnd.DateTimeValue)
            {
                FillDateTimeEdits();
                Page.AddUserMessage(Strings.EndTimeLessStartTime);
                return;
            }
            if (m_RangeChanged)
            {
                SaveDateTime();
                Changed(this, EventArgs.Empty);
            }
        }

    }
}