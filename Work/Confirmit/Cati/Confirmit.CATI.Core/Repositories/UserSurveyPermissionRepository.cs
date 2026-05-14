using System.Collections.Generic;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class UserSurveyPermissionRepository : IUserSurveyPermissionRepository
    {
        public List<BvSpUserSurveyPermission_GetEntity> GetListByUserName(string userName)
        {
            return BvSpUserSurveyPermission_GetAdapter.ExecuteEntityList(userName);
        }

        public void Insert(string userName, string projectId)
        {
            BvSpUserSurveyPermission_InsertAdapter.ExecuteNonQuery(userName, projectId);
        }

        public void Delete(string userName, string projectId)
        {
            BvSpUserSurveyPermission_DeleteAdapter.ExecuteNonQuery(userName, projectId);
        }

        public void Delete(string userName)
        {
            BvSpUserSurveyPermission_DeleteAdapter.ExecuteNonQuery(userName, null);
        }

        public void DeleteAllForSpecificSurvey(int surveyId)
        {
            BvUserSurveyPermissionAdapter.DeleteByCondition("SurveySID = @SurveySID", new SqlParameter("@SurveySID", surveyId));
        }
    }
}
