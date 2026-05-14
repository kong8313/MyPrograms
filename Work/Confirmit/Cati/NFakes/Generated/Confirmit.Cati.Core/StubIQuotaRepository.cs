using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIQuotaRepository : IQuotaRepository 
    {
        private IQuotaRepository _inner;

        public StubIQuotaRepository()
        {
            _inner = null;
        }

        public IQuotaRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvSurveyQuotaEntity TryGetByIdInt32Int32Delegate(int surveyId, int quotaId);
        public TryGetByIdInt32Int32Delegate TryGetByIdInt32Int32;

        BvSurveyQuotaEntity IQuotaRepository.TryGetById(int surveyId, int quotaId)
        {


            if (TryGetByIdInt32Int32 != null)
            {
                return TryGetByIdInt32Int32(surveyId, quotaId);
            } else if (_inner != null)
            {
                return ((IQuotaRepository)_inner).TryGetById(surveyId, quotaId);
            }

            return default(BvSurveyQuotaEntity);
        }

        public delegate IEnumerable<BvSurveyQuotaEntity> GetAllInt32Delegate(int surveyId);
        public GetAllInt32Delegate GetAllInt32;

        IEnumerable<BvSurveyQuotaEntity> IQuotaRepository.GetAll(int surveyId)
        {


            if (GetAllInt32 != null)
            {
                return GetAllInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IQuotaRepository)_inner).GetAll(surveyId);
            }

            return default(IEnumerable<BvSurveyQuotaEntity>);
        }

        public delegate void MergeBvSurveyQuotaEntityDelegate(BvSurveyQuotaEntity quota);
        public MergeBvSurveyQuotaEntityDelegate MergeBvSurveyQuotaEntity;

        void IQuotaRepository.Merge(BvSurveyQuotaEntity quota)
        {

            if (MergeBvSurveyQuotaEntity != null)
            {
                MergeBvSurveyQuotaEntity(quota);
            } else if (_inner != null)
            {
                ((IQuotaRepository)_inner).Merge(quota);
            }
        }

        public delegate void InsertListOfBvSurveyQuotaEntityDelegate(List<BvSurveyQuotaEntity> quotas);
        public InsertListOfBvSurveyQuotaEntityDelegate InsertListOfBvSurveyQuotaEntity;

        void IQuotaRepository.Insert(List<BvSurveyQuotaEntity> quotas)
        {

            if (InsertListOfBvSurveyQuotaEntity != null)
            {
                InsertListOfBvSurveyQuotaEntity(quotas);
            } else if (_inner != null)
            {
                ((IQuotaRepository)_inner).Insert(quotas);
            }
        }

        public delegate void DeleteAllInt32Delegate(int surveyId);
        public DeleteAllInt32Delegate DeleteAllInt32;

        void IQuotaRepository.DeleteAll(int surveyId)
        {

            if (DeleteAllInt32 != null)
            {
                DeleteAllInt32(surveyId);
            } else if (_inner != null)
            {
                ((IQuotaRepository)_inner).DeleteAll(surveyId);
            }
        }

    }
}