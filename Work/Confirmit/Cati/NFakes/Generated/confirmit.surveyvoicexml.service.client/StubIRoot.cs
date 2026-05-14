using System;
using System.Collections.Generic;
using System.Threading;
using Confirmit.SurveyVoiceXml.Service.Client;
using System.Threading.Tasks;
using Microsoft.Rest;
using Confirmit.SurveyVoiceXml.Service.Client.Models;

namespace Confirmit.SurveyVoiceXml.Service.Client.Fakes
{
    public class StubIRoot : IRoot 
    {
        private IRoot _inner;

        public StubIRoot()
        {
            _inner = null;
        }

        public IRoot Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Task<HttpOperationResponse<ServiceInfoModel>> ServiceInfoWithHttpMessagesAsyncDictionaryOfStringListOfStringCancellationTokenDelegate(Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken);
        public ServiceInfoWithHttpMessagesAsyncDictionaryOfStringListOfStringCancellationTokenDelegate ServiceInfoWithHttpMessagesAsyncDictionaryOfStringListOfStringCancellationToken;

        Task<HttpOperationResponse<ServiceInfoModel>> IRoot.ServiceInfoWithHttpMessagesAsync(Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {


            if (ServiceInfoWithHttpMessagesAsyncDictionaryOfStringListOfStringCancellationToken != null)
            {
                return ServiceInfoWithHttpMessagesAsyncDictionaryOfStringListOfStringCancellationToken(customHeaders, cancellationToken);
            } else if (_inner != null)
            {
                return ((IRoot)_inner).ServiceInfoWithHttpMessagesAsync(customHeaders, cancellationToken);
            }

            return default(Task<HttpOperationResponse<ServiceInfoModel>>);
        }

    }
}