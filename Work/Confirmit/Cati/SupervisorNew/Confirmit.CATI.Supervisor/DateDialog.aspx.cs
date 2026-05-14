using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Timezone;

namespace Confirmit.CATI.Supervisor
{
    public partial class DateDialog : BaseForm
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider =
            ServiceLocator.Resolve<ICachedLocalTimezoneManager>();

        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime dateTime;
            try
            {
                string val = Request.Params["date"];
                if (val != null)
                {
                    int year = Int32.Parse(val.Substring(0, 4));
                    int month = Int32.Parse(val.Substring(5, 2));
                    int day = Int32.Parse(val.Substring(8, 2));
                    int hour = Int32.Parse(val.Substring(11, 2));
                    int minute = Int32.Parse(val.Substring(14, 2));
                    int second = Int32.Parse(val.Substring(17, 2));
                    dateTime = new DateTime(year, month, day, hour, minute, second);
                }
                else
                {
                    dateTime = _timezoneProvider.GetCurrentLocalTime();
                }
            }
            catch (Exception)
            {
                dateTime = _timezoneProvider.GetCurrentLocalTime();
            }

            dteCalendar.DateTimeValue = dateTime;
        }
    }
}