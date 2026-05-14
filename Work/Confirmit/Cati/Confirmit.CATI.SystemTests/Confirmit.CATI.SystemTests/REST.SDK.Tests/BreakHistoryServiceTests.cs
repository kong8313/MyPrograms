using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Services;
using Confirmit.SystemTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests.REST.SDK.Tests
{
    [TestClass]
    public class BreakHistoryServiceTests : BaseSystemTests
    {
        private IBreakHistoryService _breakHistoryService;


        [TestInitialize]
        public void Initialize()
        {
            TestsGroupName = "Rest.Sdk";

            TestInitialize();

            _breakHistoryService = new BreakHistoryService(Confirmit.Cati.RestClient);
        }

        [TestMethod]
        public async Task BreakHistoryGet()
        {
            await _breakHistoryService.GetAsync("");
        }

        [TestMethod]
        public async Task BreakHistoryGetWithOrderAndTop()
        {
            await _breakHistoryService.GetAsync("?$orderby=Time&$top=10");
        }
    }
}
