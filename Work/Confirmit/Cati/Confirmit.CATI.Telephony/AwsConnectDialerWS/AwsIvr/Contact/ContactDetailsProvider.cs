using System.Threading.Tasks;
using Amazon;
using Amazon.Connect;
using Amazon.Connect.Model;
using Amazon.Runtime;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr.Contact
{
    public class ContactDetailsProvider
    {
        private readonly AmazonConnectClient _client;

        public ContactDetailsProvider(AwsAccessOptions options)
        {
            var awsCredentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
            _client = new AmazonConnectClient(awsCredentials, RegionEndpoint.GetBySystemName(options.Region));
        }

        public async Task<GetContactAttributesResponse> GetContactDetails(string instanceId, string contactId)
        {
            var response = await _client.GetContactAttributesAsync(new GetContactAttributesRequest
            {
                InstanceId = instanceId,
                InitialContactId = contactId
            });

            return response;
        }
    }
}
