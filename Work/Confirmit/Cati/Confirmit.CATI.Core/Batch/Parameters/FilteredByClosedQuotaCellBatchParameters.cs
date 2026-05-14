using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Batch
{
    [Serializable]
    public class FilteredByClosedQuotaCellBatchParameters : BatchParameters
    {
        public int SurveyId { get; set; }
        public int QuotaId { get; set; }
        public int CellId { get; set; }
        public List<int> CellsIds { get; set; }

        public FilteredByClosedQuotaCellBatchParameters() { }

        public FilteredByClosedQuotaCellBatchParameters(int surveyId, int quotaId, List<int> cellsIds)
        {
            SurveyId = surveyId;
            QuotaId = quotaId;
            CellsIds = cellsIds;
        }

        public override BatchType Type { get { return BatchType.FilteredByClosedQuotaCell; } }
    }
}