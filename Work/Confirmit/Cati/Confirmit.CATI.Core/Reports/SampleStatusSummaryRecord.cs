using System;
using System.Collections.Generic;
using System.ComponentModel;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;

namespace Confirmit.CATI.Core.Reports
{  
    public class SssTwoColumnWrapper
    {
        public int Index { get; set; }
        public SampleStatusSummaryRecord Data { get; set; }
    }
    /// <summary>
    /// Represents a single row of Sample Status Summary report
    /// </summary>
    [Serializable]
    public class SampleStatusSummaryRecord
    {        
        public SampleStatusSummaryRecord() { }

        /// <summary>
        /// Initializes a new instance of SampleStatusSummaryRecord and fills it with given data.
        /// </summary>
        /// <param name="entity">BvSpReportSampleStatusSummary entity object.</param>
        public SampleStatusSummaryRecord(BvSpReportSampleStatusSummaryEntity entity)
        {
            StateID = entity.StateID.Value;
            StateName = entity.StateName;
            Count = entity.Count.Value;
            SurveyName = entity.SurveyName;
            SampleSize = entity.SampleSize.Value;
            Calls = entity.Calls.Value;
            Person = entity.Person;
            Index = entity.Index.Value;
        }        

        /// <summary>
        /// State's ID
        /// </summary>
        public int StateID { get; set; }

        /// <summary>
        /// State's name
        /// </summary>
        public string StateName { get; set; }

        /// <summary>
        /// Interview count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Survey's name
        /// </summary>
        public string SurveyName { get; set; }

        /// <summary>
        /// Sample size
        /// </summary>
        public int SampleSize { get; set; }

        /// <summary>
        /// Calls count
        /// </summary>
        public int Calls { get; set; }

        /// <summary>
        /// Peron's name
        /// </summary>
        public string Person { get; set; }

        public long Index { get; set; }
    }
    
    [Serializable]
    public class SampleStatusSummaryRecordList : List<SssTwoColumnWrapper>
    {
        public SampleStatusSummaryRecordList() {}

        public SampleStatusSummaryRecordList(IEnumerable<SssTwoColumnWrapper> items) : base(items) { }
    }    
}
