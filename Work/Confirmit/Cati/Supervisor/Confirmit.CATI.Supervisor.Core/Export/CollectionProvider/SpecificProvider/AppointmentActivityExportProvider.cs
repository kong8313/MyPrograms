using System;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Activity;

namespace Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider
{
    /// <summary>
    /// Represents export provider for Appointment Activity view
    /// </summary>    
    public class AppointmentActivityExportProvider : CollectionExportProvider
    {
        #region Fields

        private bool m_IsRespondentTZ = false;
        private readonly int _localTimezoneId;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes new instance of SurveyActivityRecordExportProvider class and fills it with given data.
        /// </summary>
        /// <param name="appointmentActivityInfos"></param>
        /// <param name="isRespondentTZ"></param>
        /// <param name="localTimezoneId"></param>
        /// <param name="surveyInfo">object of type SurveyActivityInfo</param>
        public AppointmentActivityExportProvider(IEnumerable<AppointmentActivityInfo> appointmentActivityInfos, bool isRespondentTZ, int localTimezoneId)
            : base(appointmentActivityInfos)
        {
            m_IsRespondentTZ = isRespondentTZ;
            _localTimezoneId = localTimezoneId;
        }

        #endregion

        #region IEnumerable<IExportRecordProvider> Members

        public override IEnumerator<IExportRecordProvider> GetEnumerator()
        {
            foreach (object obj in m_Collection)
            {
                yield return new AppointmentActivityRecordExportProvider((AppointmentActivityInfo)obj, m_IsRespondentTZ, _localTimezoneId);
            }
        }

        #endregion
    }
}
