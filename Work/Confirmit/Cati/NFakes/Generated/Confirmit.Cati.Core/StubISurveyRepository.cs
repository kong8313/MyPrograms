using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubISurveyRepository : ISurveyRepository 
    {
        private ISurveyRepository _inner;

        public StubISurveyRepository()
        {
            _inner = null;
        }

        public ISurveyRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvSurveyEntity GetByIdInt32Delegate(int sid);
        public GetByIdInt32Delegate GetByIdInt32;

        BvSurveyEntity ISurveyRepository.GetById(int sid)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(sid);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).GetById(sid);
            }

            return default(BvSurveyEntity);
        }

        public delegate BvSurveyEntity GetWithNoCacheInt32Delegate(int sid);
        public GetWithNoCacheInt32Delegate GetWithNoCacheInt32;

        BvSurveyEntity ISurveyRepository.GetWithNoCache(int sid)
        {


            if (GetWithNoCacheInt32 != null)
            {
                return GetWithNoCacheInt32(sid);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).GetWithNoCache(sid);
            }

            return default(BvSurveyEntity);
        }

        public delegate BvSurveyEntity TryGetByIdInt32Delegate(int sid);
        public TryGetByIdInt32Delegate TryGetByIdInt32;

        BvSurveyEntity ISurveyRepository.TryGetById(int sid)
        {


            if (TryGetByIdInt32 != null)
            {
                return TryGetByIdInt32(sid);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).TryGetById(sid);
            }

            return default(BvSurveyEntity);
        }

        public delegate BvSurveyEntity GetByNameStringDelegate(string name);
        public GetByNameStringDelegate GetByNameString;

        BvSurveyEntity ISurveyRepository.GetByName(string name)
        {


            if (GetByNameString != null)
            {
                return GetByNameString(name);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).GetByName(name);
            }

            return default(BvSurveyEntity);
        }

        public delegate BvSurveyEntity TryGetByNameStringDelegate(string name);
        public TryGetByNameStringDelegate TryGetByNameString;

        BvSurveyEntity ISurveyRepository.TryGetByName(string name)
        {


            if (TryGetByNameString != null)
            {
                return TryGetByNameString(name);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).TryGetByName(name);
            }

            return default(BvSurveyEntity);
        }

        public delegate BvSurveyEntity GetByProjectIdStringDelegate(string projectId);
        public GetByProjectIdStringDelegate GetByProjectIdString;

        BvSurveyEntity ISurveyRepository.GetByProjectId(string projectId)
        {


            if (GetByProjectIdString != null)
            {
                return GetByProjectIdString(projectId);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).GetByProjectId(projectId);
            }

            return default(BvSurveyEntity);
        }

        public delegate BvSurveyEntity TryGetByProjectIdStringDelegate(string projectId);
        public TryGetByProjectIdStringDelegate TryGetByProjectIdString;

        BvSurveyEntity ISurveyRepository.TryGetByProjectId(string projectId)
        {


            if (TryGetByProjectIdString != null)
            {
                return TryGetByProjectIdString(projectId);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).TryGetByProjectId(projectId);
            }

            return default(BvSurveyEntity);
        }

        public delegate BvSurveyEntity GetByCampaignIdInt64Delegate(long campaignId);
        public GetByCampaignIdInt64Delegate GetByCampaignIdInt64;

        BvSurveyEntity ISurveyRepository.GetByCampaignId(long campaignId)
        {


            if (GetByCampaignIdInt64 != null)
            {
                return GetByCampaignIdInt64(campaignId);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).GetByCampaignId(campaignId);
            }

            return default(BvSurveyEntity);
        }

        public delegate BvSurveyEntity TryGetByCampaignIdInt64Delegate(long campaignId);
        public TryGetByCampaignIdInt64Delegate TryGetByCampaignIdInt64;

        BvSurveyEntity ISurveyRepository.TryGetByCampaignId(long campaignId)
        {


            if (TryGetByCampaignIdInt64 != null)
            {
                return TryGetByCampaignIdInt64(campaignId);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).TryGetByCampaignId(campaignId);
            }

            return default(BvSurveyEntity);
        }

        public delegate string GetSurveyNameOrErrorStringInt32Delegate(int surveyId);
        public GetSurveyNameOrErrorStringInt32Delegate GetSurveyNameOrErrorStringInt32;

        string ISurveyRepository.GetSurveyNameOrErrorString(int surveyId)
        {


            if (GetSurveyNameOrErrorStringInt32 != null)
            {
                return GetSurveyNameOrErrorStringInt32(surveyId);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).GetSurveyNameOrErrorString(surveyId);
            }

            return default(string);
        }

        public delegate string CampaignIdToProjectIdInt64Delegate(long compaingId);
        public CampaignIdToProjectIdInt64Delegate CampaignIdToProjectIdInt64;

        string ISurveyRepository.CampaignIdToProjectId(long compaingId)
        {


            if (CampaignIdToProjectIdInt64 != null)
            {
                return CampaignIdToProjectIdInt64(compaingId);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).CampaignIdToProjectId(compaingId);
            }

            return default(string);
        }

        public delegate IEnumerable<BvSurveyEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        IEnumerable<BvSurveyEntity> ISurveyRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).GetAll();
            }

            return default(IEnumerable<BvSurveyEntity>);
        }

        public delegate int InsertBvSurveyEntityDelegate(BvSurveyEntity survey);
        public InsertBvSurveyEntityDelegate InsertBvSurveyEntity;

        int ISurveyRepository.Insert(BvSurveyEntity survey)
        {


            if (InsertBvSurveyEntity != null)
            {
                return InsertBvSurveyEntity(survey);
            } else if (_inner != null)
            {
                return ((ISurveyRepository)_inner).Insert(survey);
            }

            return default(int);
        }

        public delegate void UpdateBvSurveyEntityDelegate(BvSurveyEntity survey);
        public UpdateBvSurveyEntityDelegate UpdateBvSurveyEntity;

        void ISurveyRepository.Update(BvSurveyEntity survey)
        {

            if (UpdateBvSurveyEntity != null)
            {
                UpdateBvSurveyEntity(survey);
            } else if (_inner != null)
            {
                ((ISurveyRepository)_inner).Update(survey);
            }
        }

        public delegate void DeleteInt32Delegate(int sid);
        public DeleteInt32Delegate DeleteInt32;

        void ISurveyRepository.Delete(int sid)
        {

            if (DeleteInt32 != null)
            {
                DeleteInt32(sid);
            } else if (_inner != null)
            {
                ((ISurveyRepository)_inner).Delete(sid);
            }
        }

    }
}