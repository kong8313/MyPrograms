using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IUserSurveyPermissionRepository
    {
        List<BvSpUserSurveyPermission_GetEntity> GetListByUserName(string userName);

        void Insert(string userName, string projectId);

        void Delete(string userName, string projectId);

        void Delete(string userName);

        void DeleteAllForSpecificSurvey(int surveyId);
    }
}
