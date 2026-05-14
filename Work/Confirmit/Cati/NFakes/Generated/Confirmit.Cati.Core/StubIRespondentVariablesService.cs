using System;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes
{
    public class StubIRespondentVariablesService : IRespondentVariablesService 
    {
        private IRespondentVariablesService _inner;

        public StubIRespondentVariablesService()
        {
            _inner = null;
        }

        public IRespondentVariablesService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Dictionary<string, Object> GetVariablesToSendInt32Int32Delegate(int surveyId, int respId);
        public GetVariablesToSendInt32Int32Delegate GetVariablesToSendInt32Int32;

        Dictionary<string, Object> IRespondentVariablesService.GetVariablesToSend(int surveyId, int respId)
        {


            if (GetVariablesToSendInt32Int32 != null)
            {
                return GetVariablesToSendInt32Int32(surveyId, respId);
            } else if (_inner != null)
            {
                return ((IRespondentVariablesService)_inner).GetVariablesToSend(surveyId, respId);
            }

            return default(Dictionary<string, Object>);
        }

        public delegate Dictionary<int, Dictionary<string, Object>> GetVariablesToSendInt32ListOfInt32Delegate(int surveyId, List<int> respIds);
        public GetVariablesToSendInt32ListOfInt32Delegate GetVariablesToSendInt32ListOfInt32;

        Dictionary<int, Dictionary<string, Object>> IRespondentVariablesService.GetVariablesToSend(int surveyId, List<int> respIds)
        {


            if (GetVariablesToSendInt32ListOfInt32 != null)
            {
                return GetVariablesToSendInt32ListOfInt32(surveyId, respIds);
            } else if (_inner != null)
            {
                return ((IRespondentVariablesService)_inner).GetVariablesToSend(surveyId, respIds);
            }

            return default(Dictionary<int, Dictionary<string, Object>>);
        }

    }
}