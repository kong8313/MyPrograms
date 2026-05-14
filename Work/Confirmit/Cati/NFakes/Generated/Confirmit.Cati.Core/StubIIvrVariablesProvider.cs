using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Telephony.IVR.Interfaces;
using Confirmit.SurveyVoiceXml.Service.Client.Models;

namespace Confirmit.CATI.Core.Telephony.IVR.Interfaces.Fakes
{
    public class StubIIvrVariablesProvider : IIvrVariablesProvider 
    {
        private IIvrVariablesProvider _inner;

        public StubIIvrVariablesProvider()
        {
            _inner = null;
        }

        public IIvrVariablesProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IList<VoiceXmlVariableModel> ConvertToIvrVariablesArrayOfKeyValuePairOfStringStringDelegate(KeyValuePair<string, string>[] variables);
        public ConvertToIvrVariablesArrayOfKeyValuePairOfStringStringDelegate ConvertToIvrVariablesArrayOfKeyValuePairOfStringString;

        IList<VoiceXmlVariableModel> IIvrVariablesProvider.ConvertToIvrVariables(KeyValuePair<string, string>[] variables)
        {


            if (ConvertToIvrVariablesArrayOfKeyValuePairOfStringString != null)
            {
                return ConvertToIvrVariablesArrayOfKeyValuePairOfStringString(variables);
            } else if (_inner != null)
            {
                return ((IIvrVariablesProvider)_inner).ConvertToIvrVariables(variables);
            }

            return default(IList<VoiceXmlVariableModel>);
        }

        public delegate int? GetPersonSidArrayOfKeyValuePairOfStringStringDelegate(KeyValuePair<string, string>[] variables);
        public GetPersonSidArrayOfKeyValuePairOfStringStringDelegate GetPersonSidArrayOfKeyValuePairOfStringString;

        int? IIvrVariablesProvider.GetPersonSid(KeyValuePair<string, string>[] variables)
        {


            if (GetPersonSidArrayOfKeyValuePairOfStringString != null)
            {
                return GetPersonSidArrayOfKeyValuePairOfStringString(variables);
            } else if (_inner != null)
            {
                return ((IIvrVariablesProvider)_inner).GetPersonSid(variables);
            }

            return default(int?);
        }

        public delegate int? GetInterviewIdArrayOfKeyValuePairOfStringStringDelegate(KeyValuePair<string, string>[] variables);
        public GetInterviewIdArrayOfKeyValuePairOfStringStringDelegate GetInterviewIdArrayOfKeyValuePairOfStringString;

        int? IIvrVariablesProvider.GetInterviewId(KeyValuePair<string, string>[] variables)
        {


            if (GetInterviewIdArrayOfKeyValuePairOfStringString != null)
            {
                return GetInterviewIdArrayOfKeyValuePairOfStringString(variables);
            } else if (_inner != null)
            {
                return ((IIvrVariablesProvider)_inner).GetInterviewId(variables);
            }

            return default(int?);
        }

    }
}