using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Core.Timezone;

namespace Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider
{
    /// <summary>
    /// Represents record provider for certain record in Appointment activity view
    /// </summary>
    public class AppointmentActivityRecordExportProvider : ObjectExportRecordProvider
    {
        private bool m_IsRespondentTZ = false;
        private readonly int _localTimezoneId;

        /// <summary>
        /// Initializes new instance of SurveyActivityRecordExportProvider class and fills it with given data.
        /// </summary>
        /// <param name="appointmentActivityInfo"></param>
        /// <param name="isRespondentTZ"></param>
        /// <param name="localTimezoneId"></param>
        /// <param name="surveyInfo">object of type SurveyActivityInfo</param>
        public AppointmentActivityRecordExportProvider(AppointmentActivityInfo appointmentActivityInfo, bool isRespondentTZ, int localTimezoneId)
            : base(appointmentActivityInfo)
        {
            m_IsRespondentTZ = isRespondentTZ;
            _localTimezoneId = localTimezoneId;
        }

        public override object this[string name]
        {
            get
            {
                AppointmentActivityInfo info = (AppointmentActivityInfo)m_Object;

                switch (name)
                {
                    case "AppointmentTime":
                        int tzID = m_IsRespondentTZ ? info.TimezoneID : _localTimezoneId;
                        return TimezoneManager.ConvertToTzLocalTime(tzID, info.AppointmentTime);
                }

                return base[name];                                
            }
        }

        #region IExportRecordProvider Members

        /// <summary>
        /// Gets descendant records for this record.
        /// </summary>
        public override IExportRecordProvider Descendants
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}
