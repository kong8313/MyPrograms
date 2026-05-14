using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIQuotaBalancingRepository : IQuotaBalancingRepository 
    {
        private IQuotaBalancingRepository _inner;

        public StubIQuotaBalancingRepository()
        {
            _inner = null;
        }

        public IQuotaBalancingRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvQuotaBalancingEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<BvQuotaBalancingEntity> IQuotaBalancingRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IQuotaBalancingRepository)_inner).GetAll();
            }

            return default(List<BvQuotaBalancingEntity>);
        }

        public delegate void SetBalancedQuotasForSurveyInt32IEnumerableOfBvQuotaBalancingEntityIEnumerableOfStringDelegate(int surveyId, IEnumerable<BvQuotaBalancingEntity> quotas, IEnumerable<string> fields);
        public SetBalancedQuotasForSurveyInt32IEnumerableOfBvQuotaBalancingEntityIEnumerableOfStringDelegate SetBalancedQuotasForSurveyInt32IEnumerableOfBvQuotaBalancingEntityIEnumerableOfString;

        void IQuotaBalancingRepository.SetBalancedQuotasForSurvey(int surveyId, IEnumerable<BvQuotaBalancingEntity> quotas, IEnumerable<string> fields)
        {

            if (SetBalancedQuotasForSurveyInt32IEnumerableOfBvQuotaBalancingEntityIEnumerableOfString != null)
            {
                SetBalancedQuotasForSurveyInt32IEnumerableOfBvQuotaBalancingEntityIEnumerableOfString(surveyId, quotas, fields);
            } else if (_inner != null)
            {
                ((IQuotaBalancingRepository)_inner).SetBalancedQuotasForSurvey(surveyId, quotas, fields);
            }
        }

        public delegate List<BvQuotaBalancingEntity> GetBalancedQuotasForSurveyInt32Delegate(int surveyId);
        public GetBalancedQuotasForSurveyInt32Delegate GetBalancedQuotasForSurveyInt32;

        List<BvQuotaBalancingEntity> IQuotaBalancingRepository.GetBalancedQuotasForSurvey(int surveyId)
        {


            if (GetBalancedQuotasForSurveyInt32 != null)
            {
                return GetBalancedQuotasForSurveyInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IQuotaBalancingRepository)_inner).GetBalancedQuotasForSurvey(surveyId);
            }

            return default(List<BvQuotaBalancingEntity>);
        }

        public delegate string[] GetBalancedFieldsForSurveyInt32Delegate(int surveyId);
        public GetBalancedFieldsForSurveyInt32Delegate GetBalancedFieldsForSurveyInt32;

        string[] IQuotaBalancingRepository.GetBalancedFieldsForSurvey(int surveyId)
        {


            if (GetBalancedFieldsForSurveyInt32 != null)
            {
                return GetBalancedFieldsForSurveyInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IQuotaBalancingRepository)_inner).GetBalancedFieldsForSurvey(surveyId);
            }

            return default(string[]);
        }

    }
}