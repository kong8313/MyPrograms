using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.SurveyVoiceXml.Service.Client;
using Confirmit.SurveyVoiceXml.Service.Client.Fakes;
using Confirmit.SurveyVoiceXml.Service.Client.Models;
using ConfirmitDialerInterface;
using Microsoft.Rest;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers.Mocks
{
    public class VoiceXmlServiceController : IMain
    {
        public static VoiceXmlPageModel TransferPage(TransferType type, string resource)
        {
            return new VoiceXmlPageModel
            {
                TransferConfiguration = new CatiCallTransferConfiguration { Type = type.ToString(), TransferTarget = resource }
            };
        }

        public VoiceXmlPageModel[] Scenario;

        private Dictionary<int, int> InterviewerId2ScenarioIndex = new Dictionary<int, int>();
        private IntegrationTestingFramework _testingFramework;

        public VoiceXmlServiceController(IntegrationTestingFramework testingFramework, VoiceXmlPageModel[] scenario)
        {
            _testingFramework = testingFramework;
            Scenario = scenario.ToArray();

            var stubInternalVoiceXmlApiFactory = _testingFramework.RegistryStub<IInternalVoiceXmlApiFactory, StubIInternalVoiceXmlApiFactory>();

            var stubIInternalSurveyVoiceXmlApi = new StubIInternalSurveyVoiceXmlAPI();
            stubInternalVoiceXmlApiFactory.CreateApiClient = () => stubIInternalSurveyVoiceXmlApi;

            stubIInternalSurveyVoiceXmlApi.MainGet = () => this;
        }

        public int InitialPageCallCount = 0;
        public int NextPageCallCount = 0;

        public Task<HttpOperationResponse<VoiceXmlPageModel>> InitialPageWithHttpMessagesAsync(VoiceXmlPagePostModel model, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InitialPageCallCount++;
            return Task.Factory.StartNew(() => new HttpOperationResponse<VoiceXmlPageModel>
            {
                Body = GenerateVoiceXmlPageModel(model)
            });
        }

        public Task<HttpOperationResponse<VoiceXmlPageModel>> NextPageWithHttpMessagesAsync(VoiceXmlPagePostModel model, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            NextPageCallCount++;
            return Task.Factory.StartNew(() => new HttpOperationResponse<VoiceXmlPageModel>
            {
                Body = GenerateVoiceXmlPageModel(model)
            });
        }

        private VoiceXmlPageModel GenerateVoiceXmlPageModel(VoiceXmlPagePostModel model)
        {
            var index = 0;
            if (InterviewerId2ScenarioIndex.ContainsKey((int)model.InterviewerId))
            {
                index = ++InterviewerId2ScenarioIndex[(int)model.InterviewerId];
            }

            InterviewerId2ScenarioIndex[(int)model.InterviewerId] = index;

            var voiceXml = $"<vxml><form><var name=\"catiinterviewid__\" expr=\"{model.InterviewId}\"/></form></vxml>";

            if (index < Scenario.Length)
            {
                return new VoiceXmlPageModel
                {
                    IsLastPage = Scenario[index].IsLastPage ?? false,
                    VoiceXml = voiceXml,
                    Its = Scenario[index].Its,
                    Status = Scenario[index].Status,
                    TransferConfiguration = Scenario[index].TransferConfiguration
                };
            }
            else
            {
                InterviewerId2ScenarioIndex.Remove((int)model.InterviewerId);

                return new VoiceXmlPageModel
                {
                    IsLastPage = true,
                    Its = "13",
                    Status = "completed",
                    VoiceXml = voiceXml
                };
            }
        }
    }

}
