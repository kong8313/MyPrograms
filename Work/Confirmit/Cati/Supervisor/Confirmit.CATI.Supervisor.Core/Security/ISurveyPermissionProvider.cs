using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Security
{
    public interface ISurveyPermissionProvider
    {
        void InitUserSurveyPermissions(string userName, int companyId);

        bool IsSurveyAccessible(string userName, int surveySid);

        List<int> GetUserSurveyPermission(string userName);
    }
}
