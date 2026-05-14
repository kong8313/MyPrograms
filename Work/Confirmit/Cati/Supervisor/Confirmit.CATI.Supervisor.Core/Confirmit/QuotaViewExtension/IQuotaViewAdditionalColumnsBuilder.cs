using System.Data;

using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension
{
    public interface IQuotaViewAdditionalColumnsBuilder
    {

        void AddColumns(DataColumnCollection columns);

        void FillRow(DataRow row, QuotaList quotaList, QuotaRow cell);

        string GetSummaryInfo();
    }
}