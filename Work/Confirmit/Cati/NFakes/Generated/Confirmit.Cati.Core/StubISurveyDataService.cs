using System;
using Confirmit.CATI.Core.SurveyDataService;
using Confirmit.CATI.Core.WcfServices.Clients;
using System.Data;

namespace Confirmit.CATI.Core.WcfServices.Clients.Fakes
{
    public class StubISurveyDataService : ISurveyDataService 
    {
        private ISurveyDataService _inner;

        public StubISurveyDataService()
        {
            _inner = null;
        }

        public ISurveyDataService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate TransferResult GetDataTransferDefBaseResponseTokenDelegate(TransferDefBase transferDef, ResponseToken token);
        public GetDataTransferDefBaseResponseTokenDelegate GetDataTransferDefBaseResponseToken;

        TransferResult ISurveyDataService.GetData(TransferDefBase transferDef, ResponseToken token)
        {


            if (GetDataTransferDefBaseResponseToken != null)
            {
                return GetDataTransferDefBaseResponseToken(transferDef, token);
            } else if (_inner != null)
            {
                return ((ISurveyDataService)_inner).GetData(transferDef, token);
            }

            return default(TransferResult);
        }

        public delegate ErrorMessage[] UpdateDataTransferDefDataSetBooleanBooleanInt32Delegate(TransferDef transferDef, DataSet ds, bool applyRules, bool inTransaction, int transactionKey);
        public UpdateDataTransferDefDataSetBooleanBooleanInt32Delegate UpdateDataTransferDefDataSetBooleanBooleanInt32;

        ErrorMessage[] ISurveyDataService.UpdateData(TransferDef transferDef, DataSet ds, bool applyRules, bool inTransaction, int transactionKey)
        {


            if (UpdateDataTransferDefDataSetBooleanBooleanInt32 != null)
            {
                return UpdateDataTransferDefDataSetBooleanBooleanInt32(transferDef, ds, applyRules, inTransaction, transactionKey);
            } else if (_inner != null)
            {
                return ((ISurveyDataService)_inner).UpdateData(transferDef, ds, applyRules, inTransaction, transactionKey);
            }

            return default(ErrorMessage[]);
        }

    }
}