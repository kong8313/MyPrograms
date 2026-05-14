using System;
using System.Collections.Generic;

using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Core.Activity
{

	/// <summary>
	/// Represents single row of data for CATI activity view.
	/// </summary>
	public class SurveyActivityInfo
	{
		private Dictionary<string, AlertStatus> m_AlertStatuses = new Dictionary<string, AlertStatus>();
		/// <summary>
		/// Gets or sets alert statuses for columns.
		/// Note: string value should match some property of SurveyActivityInfo.
		/// </summary>
		public Dictionary<string, AlertStatus> AlertStatuses
		{
			get { return m_AlertStatuses; }
			set { m_AlertStatuses = value; }
		}
		
		/// <summary>
		/// Gets summary for alert statuses.
		/// </summary>
		public AlertStatus Alert
		{
			get 
			{ //return m_Alert; 
				if (m_AlertStatuses.ContainsValue(AlertStatus.Error))
					return AlertStatus.Error;
				if (m_AlertStatuses.ContainsValue(AlertStatus.Warning))
					return AlertStatus.Warning;
				return AlertStatus.Ok;
			}
		}

		private int m_SID;
		/// <summary>
		/// Gets or sets BvFEE sid of survey.
		/// </summary>
		public int SID
		{
			get { return m_SID; }
			set { m_SID = value; }
		}
		
		private string m_Id;
		/// <summary>
		/// Gets or sets confirmit project id.
		/// </summary>
		public string Id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}

		private string m_Name;
		/// <summary>
		/// Gets or sets confirmit project name.
		/// </summary>
		public string Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}

		private int m_LoggedCount;
		/// <summary>
		/// Gets or sets number of logged interviewers.
		/// </summary>
		public int LoggedCount
		{
			get { return m_LoggedCount; }
			set { m_LoggedCount = value; }
		}

		private int m_AssignedCount;
		/// <summary>
		/// Gets or sets number of interviewers, assigned to survey.
		/// </summary>
		public int AssignedCount
		{
			get { return m_AssignedCount; }
			set { m_AssignedCount = value; }
		}

        public int? Target { get; set; }


        private int m_SampleSize;
		/// <summary>
		/// Gets or sets sample size for survey.
		/// </summary>
		public int SampleSize
		{
			get { return m_SampleSize; }
			set { m_SampleSize = value; }
		}

	    public int? CustomIts1 { get; set; }

	    public int? CustomIts2 { get; set; }

	    public int? CustomIts3 { get; set; }

	    public int? CustomIts4 { get; set; }

	    public int? CustomIts5 { get; set; }

        private TimeSpan m_TotalTime;
		/// <summary>
		/// Gets or sets total time spent for survey.
		/// </summary>
		public TimeSpan TotalTime
        {
			get { return m_TotalTime; }
			set { m_TotalTime = value; }
		}

        private TimeSpan m_TotalTimeToday;
		/// <summary>
		/// Gets or sets time spent for survey today only.
		/// </summary>
		public TimeSpan TotalTimeToday
		{
			get { return m_TotalTimeToday; }
			set { m_TotalTimeToday = value; }
		}

		private DateTime? m_NextAppointment;
		/// <summary>
		/// Gets or sets next appointment time for survey.
		/// </summary>
		public DateTime? NextAppointment
		{
			get { return m_NextAppointment; }
			set { m_NextAppointment = value; }
		}

		private int m_ScheduledCallsCount;
		/// <summary>
		/// Gets or sets number of scheduled calls.
		/// </summary>
		public int ScheduledCallsCount
		{
			get { return m_ScheduledCallsCount; }
			set { m_ScheduledCallsCount = value; }
		}

		private int m_SuspendedCallsCount;
		/// <summary>
		/// Gets or sets number of suspended calls.
		/// </summary>
		public int SuspendedCallsCount
		{
			get { return m_SuspendedCallsCount; }
			set { m_SuspendedCallsCount = value; }
		}
		
		private int m_StrikeRate;
		/// <summary>
		/// Gets or sets strike rate for survey.
		/// </summary>
		public int StrikeRate
		{
			get { return m_StrikeRate; }
			set { m_StrikeRate = value; }
		}

	    private int m_StrikeRate1h;
        /// <summary>
		/// Gets or sets strike rate calculated per hour for survey.
		/// </summary>
		public int StrikeRate1h
        {
			get { return m_StrikeRate1h; }
			set { m_StrikeRate1h = value; }
		}

		private int m_CountCalls;
		/// <summary>
		/// Count call made during last hour computed by 15min intervals.
		/// </summary>
		public int CountCalls
		{
			get { return m_CountCalls; }
			set { m_CountCalls = value; }
		}

        private int m_CountCalls1h;
		/// <summary>
		/// Count call made during last hour computed by 1h intervals.
		/// </summary>
		public int CountCalls1h
		{
			get { return m_CountCalls1h; }
			set { m_CountCalls1h = value; }
		}


		private TimeSpan m_InterviewDuration;
        /// <summary>
        /// Gets or sets average interview duration for survey computed by 15min intervals.
        /// </summary>
        public TimeSpan InterviewDuration
		{
			get { return m_InterviewDuration; }
			set { m_InterviewDuration = value; }
		}

	    private TimeSpan m_InterviewDuration1h;
        /// <summary>
		/// Gets or sets average interview duration for survey computed by 1h intervals.
		/// </summary>
		public TimeSpan InterviewDuration1h
		{
			get { return m_InterviewDuration1h; }
			set { m_InterviewDuration1h = value; }
		}

		/// <summary>
		/// Default empty constructor.
		/// </summary>
		public SurveyActivityInfo()
		{
		}
	}
}
