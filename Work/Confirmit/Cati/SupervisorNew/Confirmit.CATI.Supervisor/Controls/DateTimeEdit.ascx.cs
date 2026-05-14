using System;
using System.Web.Script.Serialization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Timezone;

namespace Confirmit.CATI.Supervisor.Controls
{
    /// <summary>
    ///	Summary description for DateTimeEdit.
    /// </summary>
    public partial class DateTimeEdit: BaseWUC
    {


        private readonly ICachedLocalTimezoneManager _timezoneProvider;

        public DateTimeEdit()
        {
            _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        }
        /// <summary>
        /// Defines, if date part of control is shown or not.
        /// </summary>
        public bool ShowDate
        {
            get { return wdteDate.Visible; }
            set { wdteDate.Visible = value; }
        }

        /// <summary>
        /// Defines, if time part of control is shown or not.
        /// </summary>
        public bool ShowTime
        {
            get { return wdteTime.Visible; }
            set { wdteTime.Visible = value; }
        }

        /// <summary>
        /// Gets or sets datetime value according to 'ShowDate' and 'ShowTime' properties.
        /// </summary>
        public DateTime DateTimeValue
        {
            get { return ((DateTime)wdteDate.Value).Date.Add(wdteTime.Date.TimeOfDay); }
            set
            {
                wdteDate.Value = value;
                wdteTime.Value = value;
            }
        }

        public DateTime DateTimeValueUtc
        {
            get { return _timezoneProvider.ConvertToUtc(DateTimeValue); }
            set { DateTimeValue = _timezoneProvider.ConvertToLocalTime(value); }
        }

        /// <summary>
        /// Gets or sets 'Enabled' property of the user control.
        /// </summary>
        public bool Enabled
        {
            get { return wdteDate.Enabled && wdteTime.Enabled; }
            set
            {
                wdteDate.Enabled = value;
                wdteTime.Enabled = value;
            }
        }

        /// <summary>
        /// Event occurs if date or time changes
        /// </summary>
        public event EventHandler ValueChanged;

        protected void Page_Init(object sender, EventArgs e)
        {
            wdteDate.ValueChanged +=
                delegate
                {
                    if (ValueChanged != null)
                        ValueChanged(this, EventArgs.Empty);
                };
            wdteTime.ValueChanged +=
                delegate
                {
                    if (ValueChanged != null)
                        ValueChanged(this, EventArgs.Empty);
                };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* We have to synchronize Enabled properties of controls,
             * because WebDateTimeEdit doesn't persist Enabled property 
             * to server. It means if you change this property on client side,
             * server side property remains the same. We can't do it on Page_init
             * because wdteDate.Enabled as old value at taht moment.
             */
            wdteTime.Enabled = wdteDate.Enabled;            
        }

        public string ClientControllerName
        {
            get { return ClientID + "_controller"; }
        }

        public bool CalendarExpanded
        {
            set
            {
                wdteDate.OpenCalendarOnFocus = value;
                wdteDate.FocusOnInitialization = value;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var script = string.Format("var {0} = new DateTimeEdit({1});", ClientControllerName, GetClientSettings());

            Page.ClientScript.RegisterStartupScript(GetType(), ClientID + "ControllerInit", script, true);
        }

        private string GetClientSettings()
        {
            var settings = new
                {
                    DateControlId = wdteDate.ClientID,
                    TimeControlId = wdteTime.ClientID,
                };

            return new JavaScriptSerializer().Serialize(settings);
        }
    }
}