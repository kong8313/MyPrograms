using System;
using Confirmit.CATI.Core.Repositories.SurveyEngine.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.SurveyEngine.Interfaces.Fakes
{
    public class StubISeQuotaRepository : ISeQuotaRepository 
    {
        private ISeQuotaRepository _inner;

        public StubISeQuotaRepository()
        {
            _inner = null;
        }

        public ISeQuotaRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvSurveyQuotaEntity GetByIdInt32Int32Delegate(int surveyId, int quotaId);
        public GetByIdInt32Int32Delegate GetByIdInt32Int32;

        BvSurveyQuotaEntity ISeQuotaRepository.GetById(int surveyId, int quotaId)
        {


            if (GetByIdInt32Int32 != null)
            {
                return GetByIdInt32Int32(surveyId, quotaId);
            } else if (_inner != null)
            {
                return ((ISeQuotaRepository)_inner).GetById(surveyId, quotaId);
            }

            return default(BvSurveyQuotaEntity);
        }

        public delegate BvSurveyQuotaEntity TryGetByIdInt32Int32Delegate(int surveyId, int quotaId);
        public TryGetByIdInt32Int32Delegate TryGetByIdInt32Int32;

        BvSurveyQuotaEntity ISeQuotaRepository.TryGetById(int surveyId, int quotaId)
        {


            if (TryGetByIdInt32Int32 != null)
            {
                return TryGetByIdInt32Int32(surveyId, quotaId);
            } else if (_inner != null)
            {
                return ((ISeQuotaRepository)_inner).TryGetById(surveyId, quotaId);
            }

            return default(BvSurveyQuotaEntity);
        }

        public delegate BvSurveyQuotaEntity GetByNameInt32StringDelegate(int surveyId, string quotaName);
        public GetByNameInt32StringDelegate GetByNameInt32String;

        BvSurveyQuotaEntity ISeQuotaRepository.GetByName(int surveyId, string quotaName)
        {


            if (GetByNameInt32String != null)
            {
                return GetByNameInt32String(surveyId, quotaName);
            } else if (_inner != null)
            {
                return ((ISeQuotaRepository)_inner).GetByName(surveyId, quotaName);
            }

            return default(BvSurveyQuotaEntity);
        }

        public delegate BvSurveyQuotaEntity TryGetByNameInt32StringDelegate(int surveyId, string quotaName);
        public TryGetByNameInt32StringDelegate TryGetByNameInt32String;

        BvSurveyQuotaEntity ISeQuotaRepository.TryGetByName(int surveyId, string quotaName)
        {


            if (TryGetByNameInt32String != null)
            {
                return TryGetByNameInt32String(surveyId, quotaName);
            } else if (_inner != null)
            {
                return ((ISeQuotaRepository)_inner).TryGetByName(surveyId, quotaName);
            }

            return default(BvSurveyQuotaEntity);
        }

        public delegate IEnumerable<BvSurveyQuotaEntity> GetAllInt32Delegate(int surveyId);
        public GetAllInt32Delegate GetAllInt32;

        IEnumerable<BvSurveyQuotaEntity> ISeQuotaRepository.GetAll(int surveyId)
        {


            if (GetAllInt32 != null)
            {
                return GetAllInt32(surveyId);
            } else if (_inner != null)
            {
                return ((ISeQuotaRepository)_inner).GetAll(surveyId);
            }

            return default(IEnumerable<BvSurveyQuotaEntity>);
        }

    }
}