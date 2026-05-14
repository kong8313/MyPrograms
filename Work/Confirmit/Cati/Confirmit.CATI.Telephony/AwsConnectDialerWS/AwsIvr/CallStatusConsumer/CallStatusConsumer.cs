using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr.Contact;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr.CallStatusConsumer
{
    public class CallStatusConsumer : IDisposable
    {
        private readonly string _sqsUrl;
        private readonly ILogger _logger;
        private readonly AmazonSQSClient _client;
        private readonly ContactDetailsProvider _contactDetailsProvider;
        private readonly CancellationTokenSource _cts;

        public CallStatusConsumer(AwsAccessOptions options, string sqsUrl, ILogger logger)
        {
            var awsCredentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
            _client = new AmazonSQSClient(awsCredentials, RegionEndpoint.GetBySystemName(options.Region));
            _sqsUrl = sqsUrl;
            _logger = logger;
            
            _contactDetailsProvider = new ContactDetailsProvider(options);
            _cts = new CancellationTokenSource();
        }

        public void StartCallStatusConsumer(Func<string, bool> isValidRespondent,
            Action<OnCallDisconnectedEventArgs> onCallDisconnected)
        {
            Task.Run(async () =>
                await RunPolling(isValidRespondent, onCallDisconnected, _cts.Token));
        }
        
        private async Task RunPolling(Func<string, bool> isValidRespondent,
            Action<OnCallDisconnectedEventArgs> onCallDisconnected, CancellationToken cancellationToken)
        {
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = _sqsUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            };
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var receiveMessageResponse = await _client.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);

                foreach (var message in receiveMessageResponse.Messages)
                {
                    try
                    {
                        await ProcessMessage(isValidRespondent, onCallDisconnected, message);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(nameof(CallStatusConsumer),
                            () => $"Error processing message {message.Body}: {ex.Message}");
                    }
                    finally
                    {
                        await DeleteMessage(message, cancellationToken);
                    }
                }

                await Task.Delay(1000, cancellationToken);
            }
        }

        private async Task ProcessMessage(Func<string, bool> isValidRespondent,
            Action<OnCallDisconnectedEventArgs> onCallDisconnected, Message message)
        {
            _logger.Verbose(nameof(CallStatusConsumer), $"Got message: {message.MessageId}");

            var data = JsonConvert.DeserializeObject<CallStatusEvent>(message.Body);

            if (data.Detail.Channel == "VOICE" && data.Detail.InitiationMethod == "API" && data.Detail.EventType == "DISCONNECTED")
            {
                var connectInstanceId = data.Detail.Tags["aws:connect:instanceId"];
                var contact = await _contactDetailsProvider.GetContactDetails(connectInstanceId, data.Detail.ContactId);
                contact.Attributes.TryGetValue("contextId", out var respondentContext);

                if (!isValidRespondent(respondentContext))
                    return;
                
                try
                {
                    onCallDisconnected(new OnCallDisconnectedEventArgs
                    {
                        ContextId = respondentContext,
                        AnsweringMachineDetectionStatus = data.Detail.AnsweringMachineDetectionStatus
                    });
                }
                catch (Exception ex)
                {
                    _logger.Error(nameof(CallStatusConsumer), () => $"Error in disconnect callback: {ex.Message}");
                }
            }
        }

        private async Task DeleteMessage(Message message, CancellationToken cancellationToken)
        {
            await _client.DeleteMessageAsync(new DeleteMessageRequest
            {
                QueueUrl = _sqsUrl,
                ReceiptHandle = message.ReceiptHandle
            }, cancellationToken);
            
            _logger.Verbose(nameof(CallStatusConsumer), $"Message deleted: {message.MessageId}");
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
