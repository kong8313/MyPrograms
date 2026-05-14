using System;
using Confirmit.CATI.Core.SurveyDataService;
using System.Data;

namespace Confirmit.CATI.Core.SurveyDataService.Fakes
{
    public class StubFusionSurveyDataSoap : FusionSurveyDataSoap 
    {
        private FusionSurveyDataSoap _inner;

        public StubFusionSurveyDataSoap()
        {
            _inner = null;
        }

        public FusionSurveyDataSoap Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate TransferResult GetDataStringTransferDefBaseResponseTokenDelegate(string key, TransferDefBase transferDef, ResponseToken token);
        public GetDataStringTransferDefBaseResponseTokenDelegate GetDataStringTransferDefBaseResponseToken;

        TransferResult FusionSurveyDataSoap.GetData(string key, TransferDefBase transferDef, ResponseToken token)
        {


            if (GetDataStringTransferDefBaseResponseToken != null)
            {
                return GetDataStringTransferDefBaseResponseToken(key, transferDef, token);
            } else if (_inner != null)
            {
                return ((FusionSurveyDataSoap)_inner).GetData(key, transferDef, token);
            }

            return default(TransferResult);
        }

        public delegate ErrorMessage[] UpdateDataStringTransferDefDataSetBooleanBooleanInt32Delegate(string key, TransferDef transferDef, DataSet ds, bool applyRules, bool inTransaction, int transactionKey);
        public UpdateDataStringTransferDefDataSetBooleanBooleanInt32Delegate UpdateDataStringTransferDefDataSetBooleanBooleanInt32;

        ErrorMessage[] FusionSurveyDataSoap.UpdateData(string key, TransferDef transferDef, DataSet ds, bool applyRules, bool inTransaction, int transactionKey)
        {


            if (UpdateDataStringTransferDefDataSetBooleanBooleanInt32 != null)
            {
                return UpdateDataStringTransferDefDataSetBooleanBooleanInt32(key, transferDef, ds, applyRules, inTransaction, transactionKey);
            } else if (_inner != null)
            {
                return ((FusionSurveyDataSoap)_inner).UpdateData(key, transferDef, ds, applyRules, inTransaction, transactionKey);
            }

            return default(ErrorMessage[]);
        }

    }
}