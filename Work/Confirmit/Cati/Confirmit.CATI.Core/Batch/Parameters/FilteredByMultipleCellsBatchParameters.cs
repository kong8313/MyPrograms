using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Batch
{
    [Serializable]
    public class QuotaParameter
    {
        public string[] Fields { get; set; }
        public string[][] Cells { get; set; }

        public QuotaParameter(string[] fields, string[][] cells)
        {
            Fields = fields;
            Cells = cells;
        }
        public QuotaParameter() { }
    }

    [Serializable]
    public class FilteredByMultipleCellsBatchParameters : BatchParameters
    {
        public int SurveyId { get; set; }
        public List<QuotaParameter> QuotaParameters { get; set; }
        public FilteredByMultipleCellsBatchParameters(List<QuotaParameter> quotaParameters, int surveyId)
        {
            QuotaParameters = quotaParameters;
            SurveyId = surveyId;
        }
        public FilteredByMultipleCellsBatchParameters() { }

        public override BatchType Type { get { return BatchType.FilteredByMultipleCells; } }
    }
}
