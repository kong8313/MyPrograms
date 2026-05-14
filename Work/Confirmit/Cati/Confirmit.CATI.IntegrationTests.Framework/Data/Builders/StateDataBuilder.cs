using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using System;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class StateDataBuilder : BaseObjectBuilder<StateData[]>
    {
        public StateDataBuilder(TestDataContext context, StateData[] data, DataGenerator dataGenerator)
           : base(context, data, dataGenerator)
        {
        }

        public override void Create()
        {
            var defaultGroup = StateGroupRepository.GetDefault();
            foreach (var state in Data)
            {
                var entity = StateRepository.GetByItsAndStateGroupId(state.StateID, defaultGroup.ID);
                entity.Name = state.Name;

                StateRepository.Update(entity);
            }
        }
    }
}
