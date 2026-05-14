using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.IntegrationTests.Framework.Wrappers
{
    public class PersonWrapper
    {
        public BvPersonEntity Entity { get; private set; }

        public PersonWrapper(int personId)
        {
            Entity = PersonRepository.GetById(personId);
        }

        public void Delete()
        {
            ServiceLocator.Resolve<IPersonRepository>().Delete(Entity.SID);
        }

        public void Assign(int surveyId)
        {
            AssignmentService.AssignResourceToSurvey(surveyId, Entity.SID, Entity.CallCenterID);
        }

        public List<BvSpPerson_GetAssignedSurveyListEntity> GetSurveyAssignemnts(string userName)
        {
            return BvSpPerson_GetAssignedSurveyListAdapter.ExecuteEntityList(Entity.SID, userName, Entity.CallCenterID);
        }
    }
}