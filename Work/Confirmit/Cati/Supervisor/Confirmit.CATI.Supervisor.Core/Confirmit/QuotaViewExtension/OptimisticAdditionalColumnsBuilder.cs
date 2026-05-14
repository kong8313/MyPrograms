using System;
using System.Data;

using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension
{
    public class OptimisticAdditionalColumnsBuilder : IQuotaViewAdditionalColumnsBuilder
    {
        public void AddColumns(DataColumnCollection columns)
        {
            columns.Add(QuotaManager.InProgress, typeof(int));
            columns.Add(QuotaManager.OptimisticTotalLimit, typeof(int));
        }

        public void FillRow(DataRow row, QuotaList quota, QuotaRow cell)
        {
            row[QuotaManager.InProgress] = cell.LiveCounter == -1 ? (object)DBNull.Value : cell.LiveCounter;
            row[QuotaManager.OptimisticTotalLimit] = cell.LiveTarget == -1 ? (object)DBNull.Value : cell.LiveTarget;
        }
        public string GetSummaryInfo()
        {
            return null;
        }
    }
}