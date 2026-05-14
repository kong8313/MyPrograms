using System;
using System.Collections.Generic;
using System.Threading;
using Confirmit.SurveyVoiceXml.Service.Client;
using System.Threading.Tasks;
using Microsoft.Rest;
using Confirmit.SurveyVoiceXml.Service.Client.Models;

namespace Confirmit.SurveyVoiceXml.Service.Client.Fakes
{
    public class StubIAbout : IAbout 
    {
        private IAbout _inner;

        public StubIAbout()
        {
            _inner = null;
        }

        public IAbout Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Task<HttpOperationResponse<ExtensionDetails>> AboutMethodWithHttpMessagesAsyncDictionaryOfStringListOfStringCancellationTokenDelegate(Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken);
        public AboutMethodWithHttpMessagesAsyncDictionaryOfStringListOfStringCancellationTokenDelegate AboutMethodWithHttpMessagesAsyncDictionaryOfStringListOfStringCancellationToken;

        Task<HttpOperationResponse<ExtensionDetails>> IAbout.AboutMethodWithHttpMessagesAsync(Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {


            if (AboutMethodWithHttpMessagesAsyncDictionaryOfStringListOfStringCancellationToken != null)
            {
                return AboutMethodWithHttpMessagesAsyncDictionaryOfStringListOfStringCancellationToken(customHeaders, cancellationToken);
            } else if (_inner != null)
            {
                return ((IAbout)_inner).AboutMethodWithHttpMessagesAsync(customHeaders, cancellationToken);
            }

            return default(Task<HttpOperationResponse<ExtensionDetails>>);
        }

    }
}