using System;
using Confirmit.CATI.Supervisor.Core.Filters;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Confirmit;

namespace Confirmit.CATI.Supervisor.Core.Filters.Fakes
{
    public class StubIFilterManager : IFilterManager 
    {
        private IFilterManager _inner;

        public StubIFilterManager()
        {
            _inner = null;
        }

        public IFilterManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void DeleteFilterInt32Delegate(int filterSid);
        public DeleteFilterInt32Delegate DeleteFilterInt32;

        void IFilterManager.DeleteFilter(int filterSid)
        {

            if (DeleteFilterInt32 != null)
            {
                DeleteFilterInt32(filterSid);
            } else if (_inner != null)
            {
                ((IFilterManager)_inner).DeleteFilter(filterSid);
            }
        }

        public delegate IEnumerable<VariableInfo> GetFiltersInt32NullableOfInt32Delegate(int surveyID, int? currentFilterSid);
        public GetFiltersInt32NullableOfInt32Delegate GetFiltersInt32NullableOfInt32;

        IEnumerable<VariableInfo> IFilterManager.GetFilters(int surveyID, int? currentFilterSid)
        {


            if (GetFiltersInt32NullableOfInt32 != null)
            {
                return GetFiltersInt32NullableOfInt32(surveyID, currentFilterSid);
            } else if (_inner != null)
            {
                return ((IFilterManager)_inner).GetFilters(surveyID, currentFilterSid);
            }

            return default(IEnumerable<VariableInfo>);
        }

    }
}