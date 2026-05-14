using Confirmit.CATI.IntegrationTests.Framework.Data.Builders;

namespace Confirmit.CATI.IntegrationTests.Framework.Data
{
    public class TestData : BaseTestData
    {
        public TestDataContext Create()
        {
            var context = new TestDataBuilder().Create(this);

            new TestDataMocker().Mock(context);

            return context;
        }
    }
}
