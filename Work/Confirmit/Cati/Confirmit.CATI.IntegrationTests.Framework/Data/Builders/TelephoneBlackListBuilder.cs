using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class TelephoneBlackListBuilder : BaseObjectBuilder<string[]>
    {
        public TelephoneBlackListBuilder(TestDataContext context, string[] data, DataGenerator dataGenerator)
            : base(context, data, dataGenerator)
        {
        }

        public override void Create()
        {
            var repository = ServiceLocator.Resolve<ITelephoneBlacklistRepository>();
            foreach (var number in Data)
            {
                var entity = new BvTelephoneBlacklistEntity()
                {
                    DisplayPattern = number
                };
                repository.Insert(entity);
            }
        }
    }
}