using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Confirmit.CATI.Core.Services.Survey.Quota
{
    public class QuotaMatcher
    {
        List<QuotaMatchInfo> _quotas;
        public QuotaMatcher()
        {
            _quotas = new List<QuotaMatchInfo>();
        }

        public void AddQuota(List<string> fields,
            List<BvSurveyQuotaCellEntity> quotaCells,
            Dictionary<string, HashSet<string>> availableValues)
        {
            var cellsDict = new Dictionary<string, BvSurveyQuotaCellEntity>();

            foreach (var quotaCellData in quotaCells)
            {
                var cellAnswers = string.Join(";", quotaCellData.Data.FieldValues.Select(x => x.Value));
                cellsDict.Add(cellAnswers, quotaCellData);
            }

            _quotas.Add(new QuotaMatchInfo(fields, cellsDict, availableValues));
        }

        public List<BvInterviewQuotaCellEntity> GetInterviewQuotaCells(DataRow interview)
        {
            var matchedCells = new List<BvInterviewQuotaCellEntity>();

            foreach (var quota in _quotas)
            {
                //any cell as defalt
                var quotaCell = quota.Cells[string.Join(";", quota.Fields.Select(x => ""))];

                var answerStr = string.Join(";", quota.Fields.Select(x => quota.AvailableValues[x].Contains(interview[x].ToString()) ? interview[x] : ""));
                if (quota.Cells.ContainsKey(answerStr))
                {
                    quotaCell = quota.Cells[answerStr];
                }

                var cell = new BvInterviewQuotaCellEntity
                {
                    SurveyID = quotaCell.SurveyID,
                    QuotaID = quotaCell.QuotaID,
                    CellID = quotaCell.CellID,
                    InterviewId = (int)interview["respid"]
                };

                matchedCells.Add(cell);
            }

            return matchedCells;
        }

        private class QuotaMatchInfo
        {
            public readonly Dictionary<string, BvSurveyQuotaCellEntity> Cells;
            public readonly List<string> Fields;
            public readonly Dictionary<string, HashSet<string>> AvailableValues;

            public QuotaMatchInfo(List<string> fields,
                Dictionary<string, BvSurveyQuotaCellEntity> cells,
                Dictionary<string, HashSet<string>> availableValues)
            {
                Fields = fields;
                Cells = cells;
                AvailableValues = availableValues;
            }
        }
    }
}
