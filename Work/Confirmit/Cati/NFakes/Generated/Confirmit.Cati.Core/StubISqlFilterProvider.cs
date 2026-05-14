using System;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.Fakes
{
    public class StubISqlFilterProvider : ISqlFilterProvider 
    {
        private ISqlFilterProvider _inner;

        public StubISqlFilterProvider()
        {
            _inner = null;
        }

        public ISqlFilterProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvFilterFieldsEntity> GetFieldsInt32Delegate(int filterId);
        public GetFieldsInt32Delegate GetFieldsInt32;

        List<BvFilterFieldsEntity> ISqlFilterProvider.GetFields(int filterId)
        {


            if (GetFieldsInt32 != null)
            {
                return GetFieldsInt32(filterId);
            } else if (_inner != null)
            {
                return ((ISqlFilterProvider)_inner).GetFields(filterId);
            }

            return default(List<BvFilterFieldsEntity>);
        }

        public delegate SqlFilter GetFilterInt32Int32Delegate(int filterId, int surveyId);
        public GetFilterInt32Int32Delegate GetFilterInt32Int32;

        SqlFilter ISqlFilterProvider.GetFilter(int filterId, int surveyId)
        {


            if (GetFilterInt32Int32 != null)
            {
                return GetFilterInt32Int32(filterId, surveyId);
            } else if (_inner != null)
            {
                return ((ISqlFilterProvider)_inner).GetFilter(filterId, surveyId);
            }

            return default(SqlFilter);
        }

        public delegate SqlFilter TryToGetFilterNullableOfInt32Int32Delegate(int? filterId, int surveyId);
        public TryToGetFilterNullableOfInt32Int32Delegate TryToGetFilterNullableOfInt32Int32;

        SqlFilter ISqlFilterProvider.TryToGetFilter(int? filterId, int surveyId)
        {


            if (TryToGetFilterNullableOfInt32Int32 != null)
            {
                return TryToGetFilterNullableOfInt32Int32(filterId, surveyId);
            } else if (_inner != null)
            {
                return ((ISqlFilterProvider)_inner).TryToGetFilter(filterId, surveyId);
            }

            return default(SqlFilter);
        }

    }
}