using System;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.SurveyVoiceXml.Service.Client;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIInternalVoiceXmlApiFactory : IInternalVoiceXmlApiFactory 
    {
        private IInternalVoiceXmlApiFactory _inner;

        public StubIInternalVoiceXmlApiFactory()
        {
            _inner = null;
        }

        public IInternalVoiceXmlApiFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IInternalSurveyVoiceXmlAPI CreateApiClientDelegate();
        public CreateApiClientDelegate CreateApiClient;

        IInternalSurveyVoiceXmlAPI IInternalVoiceXmlApiFactory.CreateApiClient()
        {


            if (CreateApiClient != null)
            {
                return CreateApiClient();
            } else if (_inner != null)
            {
                return ((IInternalVoiceXmlApiFactory)_inner).CreateApiClient();
            }

            return default(IInternalSurveyVoiceXmlAPI);
        }

    }
}