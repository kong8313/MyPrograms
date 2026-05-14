using Confirmit.SurveyVoiceXml.Service.Client;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IInternalVoiceXmlApiFactory
    {
        IInternalSurveyVoiceXmlAPI CreateApiClient();
    }
}