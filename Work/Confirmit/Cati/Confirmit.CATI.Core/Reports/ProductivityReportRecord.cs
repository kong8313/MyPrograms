using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;

namespace Confirmit.CATI.Core.Reports
{
	/// <summary>
	/// Represents single record of productivity reports
	/// </summary>
	[Serializable]
    public class ProductivityReportRecord
	{
		public ProductivityReportRecord()
		{ 
			PersonSID = 0;
			PersonCode = "-";
			PersonName = "-";
			TotalInterviewCount = 0;

			SurveySID = 0;
			SurveyCode = "-";
			SurveyName = "-";

			StateID = 0;
			StateName = "-";

			InterviewCount = 0;
			InterviewTime = 0;
			InterviewTimePercentage = 0;
		}

        /// <summary>
        /// Initialiazes a new instance of ProductivityReportRecord class and fills it with given data.
        /// </summary>
        /// <param name="entity">BvSpRptProdByInterEx2 entity object.</param>
        public ProductivityReportRecord(BvSpSurveyProductivityReportEntity entity)
        {
            PersonSID = entity.PersonSID.Value;
            PersonCode = entity.PersonCode;
            PersonName = entity.PersonName;
            TotalInterviewCount = entity.TotalInterviewCount.Value;

            SurveySID = entity.SurveySID.Value;
            SurveyCode = entity.SurveyCode;
            SurveyName = entity.SurveyName;

            StateID = entity.StateID.Value;
            StateName = entity.StateName;

            InterviewCount = entity.InterviewCount.Value;
            InterviewTime = entity.InterviewTime.Value;
			InterviewTimePercentage = entity.InterviewTimePercentage;
		}

	    /// <summary>
	    /// Person SID (BvFEE SID).
	    /// </summary>
	    public int PersonSID { get; set; }

	    /// <summary>
	    /// Person code (Confirmit code).
	    /// </summary>
	    public string PersonCode { get; set; }

	    /// <summary>
	    /// Person name (Confirmit name).
	    /// </summary>
	    public string PersonName { get; set; }

	    /// <summary>
	    /// Total inteview count for selected person/survey.
	    /// </summary>
	    public int TotalInterviewCount { get; set; }

	    /// <summary>
	    /// Survey SID (BvFEE SID).
	    /// </summary>
	    public int SurveySID { get; set; }

	    /// <summary>
	    /// Survey code (Confirmit code).
	    /// </summary>
	    public string SurveyCode { get; set; }

	    /// <summary>
	    /// Survey name (Confirmit name).
	    /// </summary>
	    public string SurveyName { get; set; }

	    /// <summary>
	    /// State ID (BvFEE ID).
	    /// </summary>
	    public int StateID { get; set; }

	    /// <summary>
	    /// State name (BvFEE name).
	    /// </summary>
	    public string StateName { get; set; }

	    /// <summary>
	    /// Number of interviews for selected state.
	    /// </summary>
	    public int InterviewCount { get; set; }

	    /// <summary>
	    /// Pure interviewing for selected state.
	    /// </summary>
	    public int InterviewTime { get; set; }

		/// <summary>
		/// Percentage of pure interviewing for selected state.
		/// </summary>
		public decimal InterviewTimePercentage { get; set; }
	}

    [Serializable]
    public class ProductivityReportRecordList : List<ProductivityReportRecord>
    {
        public ProductivityReportRecordList() { }

        public ProductivityReportRecordList(IEnumerable<ProductivityReportRecord> items) : base(items) { }
    }
}
