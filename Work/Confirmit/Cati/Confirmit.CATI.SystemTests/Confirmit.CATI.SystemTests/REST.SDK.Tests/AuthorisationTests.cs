using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Constants;
using Confirmit.CATI.REST.SDK.Exceptions;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Services;
using Confirmit.SystemTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests.REST.SDK.Tests
{
    [TestClass]
    public class AuthorisationTests : BaseSystemTests
    {
        private IRestClient _client;

        private ICallHistoryService _callHistoryService;


        [TestInitialize]
        public void Initialize()
        {
            TestsGroupName = "Rest.Sdk";

            TestInitialize();

            _client = Confirmit.Cati.RestClient;
            _callHistoryService = new CallHistoryService(_client);
        }


        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public async Task NoAuthKeySpecifiedForbiddenThrowed()
        {
            _client.HttpClient.DefaultRequestHeaders.Remove(Constants.XConfirmitApiKeyHeader);
            await _callHistoryService.GetAsync("");
        }
    }
}