using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.ServiceLocatorRegistry;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Persons;

namespace Confirmit.CATI.IntegrationTests.Framework.Wrappers
{
    public class CallCenterWrapper
    {
        private readonly BackendTools _backendTools;

        public BvCallCenterEntity Entity { get; private set; }
        
        public ICallCenterService Service = ServiceLocator.Resolve<ICallCenterService>();

        public static CallCenterWrapper Create(string name, BackendTools backendTools)
        {
            var repository = ServiceLocator.Resolve<ICallCenterRepository>();
            var entity = new BvCallCenterEntity
                             {
                                 Name = name,
                                 Description = name,
                                 LocalTimezoneId = 1
                             };
            repository.Insert(entity);

            return new CallCenterWrapper(entity, backendTools);
        }

        public CallCenterWrapper(BvCallCenterEntity entity, BackendTools backendTools)
        {
            Entity = entity;
            _backendTools = backendTools;
        }

        public int CreateAndAssignSurvey(string name)
        {
            var surveyId = _backendTools.CreateSurvey(name);
            
            Service.AssignSurvey(Entity.ID, surveyId);
            
            return surveyId;
        }

        public PersonWrapper CreatePerson(string name)
        {
            var personId = PersonTools.CreatePerson(name, name, ConfirmitDialerInterface.AgentTaskChoiceMode.Manual, null, Entity.ID);
            return new PersonWrapper(personId);
        }

        public PersonWrapper CreatePerson(string name, string pwd, ConfirmitDialerInterface.AgentTaskChoiceMode personMode)
        {
            var personId = PersonTools.CreatePerson(name, pwd, personMode, null, Entity.ID);
            return new PersonWrapper(personId);
        }

        public PersonWrapper CreatePerson(string name, int[] parentGroup )
        {
            var personId = PersonTools.CreatePerson(name, name, ConfirmitDialerInterface.AgentTaskChoiceMode.Manual, parentGroup, Entity.ID);
            return new PersonWrapper(personId);
        }

        public void AssignResourceToSurvey(int surveyId, int personId)
        {
            AssignmentService.AssignResourceToSurvey(surveyId, personId, Entity.ID);
        }

        public List<BvSpGetPersonsListPageEntity> GetPersonsListPage(PagingArgs args)
        {
            int totalCount;

            return DoCall( Entity.ID, () => PersonManager.GetPersonsListPage(args, out totalCount));
        }

        public static T DoCall<T>( int callCenterId, Func<T> action )
        {
            var ccp = (TestCallCenterProvider) ServiceLocator.Resolve<ICallCenterProvider>();
            var old = ccp.GetCurrentId();

            ccp.CurrentId = callCenterId;
            
            var result = action();
            
            ccp.CurrentId = old;

            return result;
        }

        public List<BvSpAssignment_ListEntity> GetSurveyAssignment(int surveyId)
        {
            return BvSpAssignment_ListAdapter.ExecuteEntityList(surveyId, Entity.ID);
        }

        public void DeassignResourceToSurvey(int surveyId, int personId)
        {
            AssignmentService.DeassignResourceFromSurvey(surveyId, personId, Entity.ID);
        }
    }
}
