using Confirmit.SurveyVoiceXml.Service.Client.Models;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Telephony.IVR.Interfaces
{
    public interface IIvrVariablesProvider
    {
        IList<VoiceXmlVariableModel> ConvertToIvrVariables(KeyValuePair<string, string>[] variables);

        int? GetPersonSid(KeyValuePair<string, string>[] variables);

        int? GetInterviewId(KeyValuePair<string, string>[] variables);
    }
}
