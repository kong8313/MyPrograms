using System;

using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
    /// <summary>
    /// Represents single row of data for appointment activity view.
    /// </summary>
    public class AppointmentActivityInfo
    {
        #region Fileds

        private AlertStatus m_alert = AlertStatus.Ok;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets alert status.
        /// </summary>
		public AlertStatus Alert
		{
			get { return m_alert; }
			set { m_alert = value; }
		}

        /// <summary>
        /// Gets or sets BvFEE sid of survey.
        /// </summary>
        public int SurveySID { get; set; }

        /// <summary>
        /// Gets or sets interview ID.
        /// </summary>
        public int InterviewID { get; set; }

        /// <summary>
        /// Gets or sets confirmit project ID.
        /// </summary>
        public string ProjectID { get; set; }

        /// <summary>
        /// Gets or sets confirmit project name.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets BvFEE name of interviewer.
        /// </summary>
        public string InterviewerName { get; set; }

        /// <summary>
        /// Gets or sets appointment time.
        /// </summary>
        public DateTime AppointmentTime { get; set; }

        /// <summary>
        /// Gets or sets timezone name.
        /// </summary>
        public string TimezoneName { get; set; }

        /// <summary>
        /// Gets or sets timezone ID.
        /// </summary>
        public int TimezoneID { get; set; }

        /// <summary>
        /// Gets or sets the ID of tha call associated with current appointment.
        /// </summary>
        public int CallID { get; set; }

        /// <summary>
        /// ITS or Extended Status to know soft or hard appointment
        /// </summary>
        public int ExtendedStatus { get; set; }

        public string ExtendedStatusName { get; set; }

        #endregion
    }
}