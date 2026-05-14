using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIUserSurveyListRepository : IUserSurveyListRepository 
    {
        private IUserSurveyListRepository _inner;

        public StubIUserSurveyListRepository()
        {
            _inner = null;
        }

        public IUserSurveyListRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<BvSpUserSurveyList_GetEntity> GetListUserSurveyListTypeDelegate(UserSurveyListType listType);
        public GetListUserSurveyListTypeDelegate GetListUserSurveyListType;

        IEnumerable<BvSpUserSurveyList_GetEntity> IUserSurveyListRepository.GetList(UserSurveyListType listType)
        {


            if (GetListUserSurveyListType != null)
            {
                return GetListUserSurveyListType(listType);
            } else if (_inner != null)
            {
                return ((IUserSurveyListRepository)_inner).GetList(listType);
            }

            return default(IEnumerable<BvSpUserSurveyList_GetEntity>);
        }

        public delegate void InsertUserSurveyListTypeInt32Delegate(UserSurveyListType listType, int surveyId);
        public InsertUserSurveyListTypeInt32Delegate InsertUserSurveyListTypeInt32;

        void IUserSurveyListRepository.Insert(UserSurveyListType listType, int surveyId)
        {

            if (InsertUserSurveyListTypeInt32 != null)
            {
                InsertUserSurveyListTypeInt32(listType, surveyId);
            } else if (_inner != null)
            {
                ((IUserSurveyListRepository)_inner).Insert(listType, surveyId);
            }
        }

    }
}