using Confirmit.CATI.Core.Telephony.IVR.Interfaces;
using Confirmit.SurveyVoiceXml.Service.Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Confirmit.CATI.Core.Telephony.IVR
{
    public class IvrVariablesProvider : IIvrVariablesProvider
    {
        private const string PersonSidVar = "__catiinterviewerid";
        private const string InterviewIdVar = "catiinterviewid__";

        public IList<VoiceXmlVariableModel> ConvertToIvrVariables(KeyValuePair<string, string>[] variables)
        {
            return variables.Select(x => new VoiceXmlVariableModel
            {
                Name = x.Key,
                Value = x.Value
            }).ToList();
        }

        public int? GetPersonSid(KeyValuePair<string, string>[] variables)
        {
            return GetIntValueFromVariables(variables, PersonSidVar);
        }

        public int? GetInterviewId(KeyValuePair<string, string>[] variables)
        {
            return GetIntValueFromVariables(variables, InterviewIdVar);
        }

        private int? GetIntValueFromVariables(KeyValuePair<string, string>[] variables, string key)
        {
            var variable = variables.FirstOrDefault(x => x.Key == key).Value;
            if (variable != null && int.TryParse(variable.Trim('\''), out int value))
            {
                return value;
            }

            Trace.TraceWarning($"Value [{variable}] can't be cast to numeric");
            return null;
        }
    }
}
