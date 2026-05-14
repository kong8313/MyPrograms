using System;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class CallGroupDataBuilder : BaseObjectBuilder<CallGroupData>
    {
        private readonly ICallGroupRepository _callGroupRepository = ServiceLocator.Resolve<ICallGroupRepository>();
        private readonly ICallGroupService _callGroupService = ServiceLocator.Resolve<ICallGroupService>();

        public CallGroupDataBuilder(
            TestDataContext context, 
            CallGroupData data, 
            DataGenerator dataGenerator) 
            : base(context, data, dataGenerator)
        {
        }

        public override void Create()
        {
            CheckAndInitData();

            var model = new BvCallGroupEntity
            {
                Name = Data.Name,
                Description = Data.Description
            };

            _callGroupRepository.Insert(model);

            var controller = new CallGroupController(Data.Tag, model.Id, Context);
            Context.CallGroups.Add(controller);

            _callGroupService.AddConditions(model.Id, Data.ITS.Select(x => new BvCallGroupConditionEntity { ConditionValue = (int)x, ConditionPriority = 1 }));
        }

        private void CheckAndInitData()
        {
            if (String.IsNullOrWhiteSpace(Data.Name))
            {
                Data.Name = DataGenerator.NewName(Data.Tag ?? "Call group name");
            }
            
            if (String.IsNullOrWhiteSpace(Data.Description))
            {
                Data.Description = DataGenerator.NewName("Call group description");
            }
        }
    }
}