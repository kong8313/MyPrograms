using System;
using Confirmit.SurveyVoiceXml.Service.Client.Models;
using System.Collections.Generic;
using System.Threading;
using Confirmit.SurveyVoiceXml.Service.Client;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace Confirmit.SurveyVoiceXml.Service.Client.Fakes
{
    public class StubIMain : IMain 
    {
        private IMain _inner;

        public StubIMain()
        {
            _inner = null;
        }

        public IMain Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Task<HttpOperationResponse<VoiceXmlPageModel>> InitialPageWithHttpMessagesAsyncVoiceXmlPagePostModelDictionaryOfStringListOfStringCancellationTokenDelegate(VoiceXmlPagePostModel model, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken);
        public InitialPageWithHttpMessagesAsyncVoiceXmlPagePostModelDictionaryOfStringListOfStringCancellationTokenDelegate InitialPageWithHttpMessagesAsyncVoiceXmlPagePostModelDictionaryOfStringListOfStringCancellationToken;

        Task<HttpOperationResponse<VoiceXmlPageModel>> IMain.InitialPageWithHttpMessagesAsync(VoiceXmlPagePostModel model, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {


            if (InitialPageWithHttpMessagesAsyncVoiceXmlPagePostModelDictionaryOfStringListOfStringCancellationToken != null)
            {
                return InitialPageWithHttpMessagesAsyncVoiceXmlPagePostModelDictionaryOfStringListOfStringCancellationToken(model, customHeaders, cancellationToken);
            } else if (_inner != null)
            {
                return ((IMain)_inner).InitialPageWithHttpMessagesAsync(model, customHeaders, cancellationToken);
            }

            return default(Task<HttpOperationResponse<VoiceXmlPageModel>>);
        }

        public delegate Task<HttpOperationResponse<VoiceXmlPageModel>> NextPageWithHttpMessagesAsyncVoiceXmlPagePostModelDictionaryOfStringListOfStringCancellationTokenDelegate(VoiceXmlPagePostModel model, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken);
        public NextPageWithHttpMessagesAsyncVoiceXmlPagePostModelDictionaryOfStringListOfStringCancellationTokenDelegate NextPageWithHttpMessagesAsyncVoiceXmlPagePostModelDictionaryOfStringListOfStringCancellationToken;

        Task<HttpOperationResponse<VoiceXmlPageModel>> IMain.NextPageWithHttpMessagesAsync(VoiceXmlPagePostModel model, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {


            if (NextPageWithHttpMessagesAsyncVoiceXmlPagePostModelDictionaryOfStringListOfStringCancellationToken != null)
            {
                return NextPageWithHttpMessagesAsyncVoiceXmlPagePostModelDictionaryOfStringListOfStringCancellationToken(model, customHeaders, cancellationToken);
            } else if (_inner != null)
            {
                return ((IMain)_inner).NextPageWithHttpMessagesAsync(model, customHeaders, cancellationToken);
            }

            return default(Task<HttpOperationResponse<VoiceXmlPageModel>>);
        }

    }
}