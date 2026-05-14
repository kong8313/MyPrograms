using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIFilterRepository : IFilterRepository 
    {
        private IFilterRepository _inner;

        public StubIFilterRepository()
        {
            _inner = null;
        }

        public IFilterRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvFiltersEntity GetByIdInt32Delegate(int sid);
        public GetByIdInt32Delegate GetByIdInt32;

        BvFiltersEntity IFilterRepository.GetById(int sid)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(sid);
            } else if (_inner != null)
            {
                return ((IFilterRepository)_inner).GetById(sid);
            }

            return default(BvFiltersEntity);
        }

        public delegate List<BvFiltersEntity> GetFiltersListBooleanInt32Delegate(bool includeSiteWide, int surveyId);
        public GetFiltersListBooleanInt32Delegate GetFiltersListBooleanInt32;

        List<BvFiltersEntity> IFilterRepository.GetFiltersList(bool includeSiteWide, int surveyId)
        {


            if (GetFiltersListBooleanInt32 != null)
            {
                return GetFiltersListBooleanInt32(includeSiteWide, surveyId);
            } else if (_inner != null)
            {
                return ((IFilterRepository)_inner).GetFiltersList(includeSiteWide, surveyId);
            }

            return default(List<BvFiltersEntity>);
        }

        public delegate List<int> GetAllParentFiltersInt32Delegate(int filterSid);
        public GetAllParentFiltersInt32Delegate GetAllParentFiltersInt32;

        List<int> IFilterRepository.GetAllParentFilters(int filterSid)
        {


            if (GetAllParentFiltersInt32 != null)
            {
                return GetAllParentFiltersInt32(filterSid);
            } else if (_inner != null)
            {
                return ((IFilterRepository)_inner).GetAllParentFilters(filterSid);
            }

            return default(List<int>);
        }

    }
}