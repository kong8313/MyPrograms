using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.Connect;
using Amazon.Connect.Model;
using Amazon.Runtime;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr.OutboundCall
{
    public class OutboundCaller
    {
        private readonly AmazonConnectClient _client;

        public OutboundCaller(AwsAccessOptions options)
        {
            var awsCredentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
            _client = new AmazonConnectClient(awsCredentials, RegionEndpoint.GetBySystemName(options.Region));
        }

        public async Task<string> StartOutboundVoiceContact(OutboundCallPayload payload)
        {
            var splittedUrl = payload.SurveyUrl.Split('?');

            var contactAttributes = new Dictionary<string, string>
            {
                { "contextId", payload.ContextId },
                { "surveyId", payload.SurveyId },
                { "surveyUrl", splittedUrl[0] },
                { "surveyQuerystring", splittedUrl.Length > 1 ? "?" + splittedUrl[1] : string.Empty }
            };

            var response = await _client.StartOutboundVoiceContactAsync(new StartOutboundVoiceContactRequest
            {
                InstanceId = payload.InstanceId,
                ContactFlowId = payload.ContactFlowId,
                DestinationPhoneNumber = payload.DestinationPhoneNumber,
                SourcePhoneNumber = payload.SourcePhoneNumber,
                QueueId = payload.QueueId,
                CampaignId = payload.CampaignId,
                TrafficType = TrafficType.CAMPAIGN,
                AnswerMachineDetectionConfig = new AnswerMachineDetectionConfig
                {
                    AwaitAnswerMachinePrompt = payload.EnableAnswerMachineDetection,
                    EnableAnswerMachineDetection = payload.EnableAnswerMachineDetection
                },
                Attributes = contactAttributes
            });

            return response.ContactId;
        }
    }
}
