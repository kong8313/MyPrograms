using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIUserSurveyPermissionRepository : IUserSurveyPermissionRepository 
    {
        private IUserSurveyPermissionRepository _inner;

        public StubIUserSurveyPermissionRepository()
        {
            _inner = null;
        }

        public IUserSurveyPermissionRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvSpUserSurveyPermission_GetEntity> GetListByUserNameStringDelegate(string userName);
        public GetListByUserNameStringDelegate GetListByUserNameString;

        List<BvSpUserSurveyPermission_GetEntity> IUserSurveyPermissionRepository.GetListByUserName(string userName)
        {


            if (GetListByUserNameString != null)
            {
                return GetListByUserNameString(userName);
            } else if (_inner != null)
            {
                return ((IUserSurveyPermissionRepository)_inner).GetListByUserName(userName);
            }

            return default(List<BvSpUserSurveyPermission_GetEntity>);
        }

        public delegate void InsertStringStringDelegate(string userName, string projectId);
        public InsertStringStringDelegate InsertStringString;

        void IUserSurveyPermissionRepository.Insert(string userName, string projectId)
        {

            if (InsertStringString != null)
            {
                InsertStringString(userName, projectId);
            } else if (_inner != null)
            {
                ((IUserSurveyPermissionRepository)_inner).Insert(userName, projectId);
            }
        }

        public delegate void DeleteStringStringDelegate(string userName, string projectId);
        public DeleteStringStringDelegate DeleteStringString;

        void IUserSurveyPermissionRepository.Delete(string userName, string projectId)
        {

            if (DeleteStringString != null)
            {
                DeleteStringString(userName, projectId);
            } else if (_inner != null)
            {
                ((IUserSurveyPermissionRepository)_inner).Delete(userName, projectId);
            }
        }

        public delegate void DeleteStringDelegate(string userName);
        public DeleteStringDelegate DeleteString;

        void IUserSurveyPermissionRepository.Delete(string userName)
        {

            if (DeleteString != null)
            {
                DeleteString(userName);
            } else if (_inner != null)
            {
                ((IUserSurveyPermissionRepository)_inner).Delete(userName);
            }
        }

        public delegate void DeleteAllForSpecificSurveyInt32Delegate(int surveyId);
        public DeleteAllForSpecificSurveyInt32Delegate DeleteAllForSpecificSurveyInt32;

        void IUserSurveyPermissionRepository.DeleteAllForSpecificSurvey(int surveyId)
        {

            if (DeleteAllForSpecificSurveyInt32 != null)
            {
                DeleteAllForSpecificSurveyInt32(surveyId);
            } else if (_inner != null)
            {
                ((IUserSurveyPermissionRepository)_inner).DeleteAllForSpecificSurvey(surveyId);
            }
        }

    }
}