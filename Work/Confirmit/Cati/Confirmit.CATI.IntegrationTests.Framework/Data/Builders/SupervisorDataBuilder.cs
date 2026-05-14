using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    class SupervisorDataBuilder : BaseObjectBuilder<SupervisorData>
    {
        public SupervisorDataBuilder(TestDataContext context, SupervisorData data, DataGenerator dataGenerator) 
            : base(context, data, dataGenerator)
        {
        }

        public override void Create()
        {
            CheckAndInitData();

            Context.Supervisors.Add(new SupervisorController(Data.Tag, Data.Name, Context));
        }
        
        private void CheckAndInitData()
        {
            if (Data.Name == null)
            {
                Data.Name = "Supervisor " + Data.Tag;
            }
        }
        public override void Setup()
        {
            if (Data.CurrentCallCenter != null)
            {
                var currentCallCenter = Context.GetCallCenter(Data.CurrentCallCenter);

                BvSupervisorAssignmentAdapter.Insert(new BvSupervisorAssignmentEntity()
                {
                    CallCenterId = currentCallCenter.Id,
                    Name = Data.Name
                });
            }

            if (Data.Surveys != null)
            {
                foreach (var survey in Context.GetSurveys(Data.Surveys))
                {
                    var entity = new BvUserSurveyPermissionEntity()
                    {
                        UserName = Data.Name,
                        SurveySID = survey.Id
                    };
                    BvUserSurveyPermissionAdapter.Insert(entity);
                }
            }
        }
    }
}