using Confirmit.CATI.Core.Services.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RoutineMaintenance.Actions
{
    [TestClass]
    public class DatabaseIndexesActionsTest : BaseMockedIntegrationTest
    {
        private DatabaseIndexService _databaseIndexService;

        public override void OnPostTestInitialize()
        {
            _databaseIndexService = new DatabaseIndexService(new DatabaseServerPropertiesProvider());
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void RebuildIndexes_RebuildAllIndexes_RebuildSuccess()
        {
            var allIndexes = _databaseIndexService.GetAllIndexes(null);
            foreach (var indexInfo in allIndexes)
            {
                _databaseIndexService.RebuildIndex(indexInfo.TableName, indexInfo.IndexName, indexInfo.ContainsLob);
            }
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void ReorganizeIndexes_ReorganizeAllIndexes_ReorganizeSuccess()
        {
            var allIndexes = _databaseIndexService.GetAllIndexes(null);
            foreach (var indexInfo in allIndexes)
            {
                _databaseIndexService.ReorginizeIndex(indexInfo.TableName, indexInfo.IndexName);
            }
        }
    }
}
