using System;
using System.Collections.Generic;
using System.Text;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
    /// <summary>
    /// Represents single row of data for survey appointment counts.
    /// </summary>
    public class SurveyAppointmentCountInfo
    {
        #region Fields
        private int m_shortIntervalCount;
        private int m_longIntervalCount;
        private int m_surveySID;
        private string m_surveyName;
        private string m_projectId;
        private bool m_isTotalCount = false;
        #endregion

        #region Properties
        /// <summary>
        /// Count of appointments of short interval.
        /// </summary>
        public int ShortIntervalCount
        {
            get { return m_shortIntervalCount; }
            set { m_shortIntervalCount = value; }
        }

        /// <summary>
        /// Count of appointments of long interval.
        /// </summary>
        public int LongIntervalCount
        {
            get { return m_longIntervalCount; }
            set { m_longIntervalCount = value; }
        }

        /// <summary>
        /// BvFEE survey SID.
        /// </summary>
        public int SurveySID
        {
            get { return m_surveySID; }
            set { m_surveySID = value; }
        }

        /// <summary>
        /// Confirmit project name.
        /// </summary>
        public string ProjectName
        {
            get { return m_surveyName; }
            set { m_surveyName = value; }
        }

        /// <summary>
        /// Gets or sets confirmit project id.
        /// </summary>
        public string ProjectId
        {
            get { return m_projectId; }
            set { m_projectId = value; }
        }

        /// <summary>
        /// Determins if current row of data is 'Total' row.
        /// </summary>
        public bool IsTotalCount
        {
            get { return m_isTotalCount; }
            set { m_isTotalCount = value; }
        }
        #endregion

        #region Methodes
        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public SurveyAppointmentCountInfo()
        {
        }
        #endregion
    }
}