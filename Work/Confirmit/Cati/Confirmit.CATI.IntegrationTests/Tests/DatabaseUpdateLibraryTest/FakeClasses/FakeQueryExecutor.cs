using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.Tools;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.FakeClasses
{
    public class FakeQueryExecutor : QueryExecutor
    {
        public FakeQueryExecutor(ILogger logger, IConfiguration configuration)
            : base(logger, configuration)
        {
        }

        public override T ExecuteScalar<T>(string databaseName, string query)
        {
            if (query.Contains(string.Format("SELECT COUNT(*) FROM [{0}].master.sys.databases WHERE name = ", DBUpdateLibraryTestHelper.TestLinkedServerName)))
            {
                return default(T);
            }

            return ExecuteScalar<T>(databaseName, query, false);
        }
    }
}
