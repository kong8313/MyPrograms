using System;
using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Core.Batch
{
    [Serializable]
    public class FilteredByCellsBatchParameters : BatchParameters
    {
        public int SurveyId { get; set; }
        public string[] Fields { get; set; }
        public string[][] Cells { get; set; }

        public FilteredByCellsBatchParameters(){}

        public FilteredByCellsBatchParameters(int surveyId, string[] fields, IEnumerable<string[]> cells)
        {
            SurveyId = surveyId;
            Fields = fields;
            Cells = cells.ToArray();
        }

        public override BatchType Type { get { return BatchType.FilteredByCells; } }
    }
}