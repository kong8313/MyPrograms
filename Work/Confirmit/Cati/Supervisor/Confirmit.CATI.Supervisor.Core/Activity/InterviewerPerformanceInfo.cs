using System;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
	/// <summary>
	/// Represents single row of data for task activity view.
	/// </summary>
	public class InterviewerPerformanceInfo
    {     
        #region Properties

        public int InterviewerId { get; set; }

	    public string InterviewerName { get; set; }

        public int SurveyId { get; set; }

        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public TimeSpan InterviewingTime { get; set; }

        public int TotalInterviewCount { get; set; }

        public int CompletedInterviewCount { get; set; }

	    public int CompletedInLastHourCount { get; set; }

        public float StrikeRateAverage { get; set; }	    

	    #endregion
	}
}