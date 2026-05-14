using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Batch
{
    [Serializable]
    public class FilteredByOpenedQuotaCellBatchParameters : BatchParameters
    {
        public int SurveyId { get; set; }
        public int QuotaId { get; set; }
        public int CellId { get; set; }
        public List<int> CellsIds { get; set; }

        public FilteredByOpenedQuotaCellBatchParameters() { }

        public FilteredByOpenedQuotaCellBatchParameters(int surveyId, int quotaId, List<int> cellsIds)
        {
            SurveyId = surveyId;
            QuotaId = quotaId;
            CellsIds = cellsIds;
        }

        public override BatchType Type { get { return BatchType.FilteredByOpenedQuotaCell; } }
    }
}