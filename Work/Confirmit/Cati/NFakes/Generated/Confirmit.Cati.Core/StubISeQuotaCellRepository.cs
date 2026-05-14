using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Repositories.SurveyEngine.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.SurveyEngine.Interfaces.Fakes
{
    public class StubISeQuotaCellRepository : ISeQuotaCellRepository 
    {
        private ISeQuotaCellRepository _inner;

        public StubISeQuotaCellRepository()
        {
            _inner = null;
        }

        public ISeQuotaCellRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvSurveyQuotaCellEntity GetByIdInt32Int32Int32IEnumerableOfStringDelegate(int surveyId, int quotaId, int cellId, IEnumerable<string> quotaFields);
        public GetByIdInt32Int32Int32IEnumerableOfStringDelegate GetByIdInt32Int32Int32IEnumerableOfString;

        BvSurveyQuotaCellEntity ISeQuotaCellRepository.GetById(int surveyId, int quotaId, int cellId, IEnumerable<string> quotaFields)
        {


            if (GetByIdInt32Int32Int32IEnumerableOfString != null)
            {
                return GetByIdInt32Int32Int32IEnumerableOfString(surveyId, quotaId, cellId, quotaFields);
            } else if (_inner != null)
            {
                return ((ISeQuotaCellRepository)_inner).GetById(surveyId, quotaId, cellId, quotaFields);
            }

            return default(BvSurveyQuotaCellEntity);
        }

        public delegate BvSurveyQuotaCellEntity TryGetByIdInt32Int32Int32IEnumerableOfStringDelegate(int surveyId, int quotaId, int cellId, IEnumerable<string> quotaFields);
        public TryGetByIdInt32Int32Int32IEnumerableOfStringDelegate TryGetByIdInt32Int32Int32IEnumerableOfString;

        BvSurveyQuotaCellEntity ISeQuotaCellRepository.TryGetById(int surveyId, int quotaId, int cellId, IEnumerable<string> quotaFields)
        {


            if (TryGetByIdInt32Int32Int32IEnumerableOfString != null)
            {
                return TryGetByIdInt32Int32Int32IEnumerableOfString(surveyId, quotaId, cellId, quotaFields);
            } else if (_inner != null)
            {
                return ((ISeQuotaCellRepository)_inner).TryGetById(surveyId, quotaId, cellId, quotaFields);
            }

            return default(BvSurveyQuotaCellEntity);
        }

        public delegate IEnumerable<BvSurveyQuotaCellEntity> GetAllByQuotaInt32Int32IEnumerableOfStringDelegate(int surveyId, int quotaId, IEnumerable<string> quotaFields);
        public GetAllByQuotaInt32Int32IEnumerableOfStringDelegate GetAllByQuotaInt32Int32IEnumerableOfString;

        IEnumerable<BvSurveyQuotaCellEntity> ISeQuotaCellRepository.GetAllByQuota(int surveyId, int quotaId, IEnumerable<string> quotaFields)
        {


            if (GetAllByQuotaInt32Int32IEnumerableOfString != null)
            {
                return GetAllByQuotaInt32Int32IEnumerableOfString(surveyId, quotaId, quotaFields);
            } else if (_inner != null)
            {
                return ((ISeQuotaCellRepository)_inner).GetAllByQuota(surveyId, quotaId, quotaFields);
            }

            return default(IEnumerable<BvSurveyQuotaCellEntity>);
        }

    }
}