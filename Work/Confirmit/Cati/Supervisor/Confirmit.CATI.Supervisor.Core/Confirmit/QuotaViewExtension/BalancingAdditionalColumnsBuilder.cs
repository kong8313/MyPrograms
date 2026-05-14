using System;
using System.Data;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension
{
    public class BalancingAdditionalColumnsBuilder : IQuotaViewAdditionalColumnsBuilder
    {
        public void AddColumns(DataColumnCollection columns)
        {
            columns.Add(QuotaManager.Priority, typeof(string));
        }

        public void FillRow(DataRow row, QuotaList quota, QuotaRow cell)
        {
            row[QuotaManager.Priority] = GetStringPriority(cell.Priority); ;
        }

        public string GetSummaryInfo()
        {
            return null;
        }

        private static string GetStringPriority(QuotaLimitPriority? priority)
        {
            priority = priority ?? QuotaLimitPriority.Medium;
            switch (priority)
            {
                case QuotaLimitPriority.Disabled:
                    return Supervisor.Resources.Strings.NoBalancing;
                case QuotaLimitPriority.Low:
                    return Supervisor.Resources.Strings.Low;
                case QuotaLimitPriority.Medium:
                    return Supervisor.Resources.Strings.Medium;
                case QuotaLimitPriority.High:
                    return Supervisor.Resources.Strings.High;
                default:
                    throw new Exception("Unexpected priority value");
            }

        }
    }
}