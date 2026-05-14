namespace Confirmit.CATI.Supervisor.Core.Activity
{
    public class SystemWideInfo
    {
        private int m_loggedInterviewersCount;
        private int m_openSurveysCount;
        private int m_callsCount;

        public int LoggedIvrAgentsCount { get; set; }

        /// <summary>
        /// Gets or sets count of interviewers logged in the system.
        /// </summary>
        public int LoggedInterviewersCount
        {
            get { return m_loggedInterviewersCount; }
            set { m_loggedInterviewersCount = value; }
        }

        public int TotalInterviewersCount
        {
            get; 
            set; 
        }

        public int TotalInterviewersWorkedTodayCount
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int OpenSurveysCount
        {
            get { return m_openSurveysCount; }
            set { m_openSurveysCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CallsCount
        {
            get { return m_callsCount; }
            set { m_callsCount = value; }
        }

        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public SystemWideInfo()
        {
        }
    }
}