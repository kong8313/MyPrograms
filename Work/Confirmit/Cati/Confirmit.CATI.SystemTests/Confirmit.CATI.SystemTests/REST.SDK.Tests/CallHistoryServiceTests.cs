using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Services;
using Confirmit.SystemTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests.REST.SDK.Tests
{
    [TestClass]
    public class CallHistoryServiceTests : BaseSystemTests
    {
        private ICallHistoryService _callHistoryService;


        [TestInitialize]
        public void Initialize()
        {
            TestsGroupName = "Rest.Sdk";

            TestInitialize();

            _callHistoryService = new CallHistoryService(Confirmit.Cati.RestClient);
        }

        [TestMethod]
        public async Task CallHistoryGet()
        {
            await _callHistoryService.GetAsync("");
        }

        [TestMethod]
        public async Task CallHistoryGetWithOrderAndTop()
        {
            await _callHistoryService.GetAsync("?$orderby=Time&$top=10");
        }
    }
}
