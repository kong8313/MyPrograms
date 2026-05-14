using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IUserSurveyListRepository
    {
        IEnumerable<BvSpUserSurveyList_GetEntity> GetList(UserSurveyListType listType);
        void Insert(UserSurveyListType listType, int surveyId);
    }
}