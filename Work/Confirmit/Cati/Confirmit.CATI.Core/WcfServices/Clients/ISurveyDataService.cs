using System.Data;
using Confirmit.CATI.Core.SurveyDataService;

namespace Confirmit.CATI.Core.WcfServices.Clients
{
    public interface ISurveyDataService
    {
        TransferResult GetData(TransferDefBase transferDef, ResponseToken token);
        ErrorMessage[] UpdateData(TransferDef transferDef, DataSet ds, bool applyRules, bool inTransaction, int transactionKey);
    }
}