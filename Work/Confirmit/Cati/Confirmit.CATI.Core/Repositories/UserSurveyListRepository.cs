using System.Collections;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.CallCenters;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class UserSurveyListRepository : IUserSurveyListRepository
    {
        private readonly ISupervisorNameProvider _supervisorNameProvider;
        private readonly ICallCenterService _callCenterService;

        public UserSurveyListRepository(
            ISupervisorNameProvider supervisorNameProvider,
            ICallCenterService callCenterService)
        {
            _supervisorNameProvider = supervisorNameProvider;
            _callCenterService = callCenterService;
        }

        public IEnumerable<BvSpUserSurveyList_GetEntity> GetList(UserSurveyListType listType)
        {
            var userName = _supervisorNameProvider.Name;
            var callCenterId = _callCenterService.GetSupervisorCallCenter(userName).ID;
            
            return BvSpUserSurveyList_GetAdapter.ExecuteEntityList(userName, (byte) listType, callCenterId);
        }

        public void Insert(UserSurveyListType listType, int surveyId)
        {
            var userName = _supervisorNameProvider.Name;

            BvSpUserSurveyList_InsertAdapter.ExecuteNonQuery(userName, (byte)listType, surveyId);
        }
    }
}