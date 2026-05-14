using System;
using Confirmit.CATI.Supervisor.Core.Security;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Security.Fakes
{
    public class StubISurveyPermissionProvider : ISurveyPermissionProvider 
    {
        private ISurveyPermissionProvider _inner;

        public StubISurveyPermissionProvider()
        {
            _inner = null;
        }

        public ISurveyPermissionProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitUserSurveyPermissionsStringInt32Delegate(string userName, int companyId);
        public InitUserSurveyPermissionsStringInt32Delegate InitUserSurveyPermissionsStringInt32;

        void ISurveyPermissionProvider.InitUserSurveyPermissions(string userName, int companyId)
        {

            if (InitUserSurveyPermissionsStringInt32 != null)
            {
                InitUserSurveyPermissionsStringInt32(userName, companyId);
            } else if (_inner != null)
            {
                ((ISurveyPermissionProvider)_inner).InitUserSurveyPermissions(userName, companyId);
            }
        }

        public delegate bool IsSurveyAccessibleStringInt32Delegate(string userName, int surveySid);
        public IsSurveyAccessibleStringInt32Delegate IsSurveyAccessibleStringInt32;

        bool ISurveyPermissionProvider.IsSurveyAccessible(string userName, int surveySid)
        {


            if (IsSurveyAccessibleStringInt32 != null)
            {
                return IsSurveyAccessibleStringInt32(userName, surveySid);
            } else if (_inner != null)
            {
                return ((ISurveyPermissionProvider)_inner).IsSurveyAccessible(userName, surveySid);
            }

            return default(bool);
        }

        public delegate List<int> GetUserSurveyPermissionStringDelegate(string userName);
        public GetUserSurveyPermissionStringDelegate GetUserSurveyPermissionString;

        List<int> ISurveyPermissionProvider.GetUserSurveyPermission(string userName)
        {


            if (GetUserSurveyPermissionString != null)
            {
                return GetUserSurveyPermissionString(userName);
            } else if (_inner != null)
            {
                return ((ISurveyPermissionProvider)_inner).GetUserSurveyPermission(userName);
            }

            return default(List<int>);
        }

    }
}