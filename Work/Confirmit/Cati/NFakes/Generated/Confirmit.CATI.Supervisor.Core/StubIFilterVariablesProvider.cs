using System;
using Confirmit.CATI.Supervisor.Core.Filters;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Confirmit;

namespace Confirmit.CATI.Supervisor.Core.Filters.Fakes
{
    public class StubIFilterVariablesProvider : IFilterVariablesProvider 
    {
        private IFilterVariablesProvider _inner;

        public StubIFilterVariablesProvider()
        {
            _inner = null;
        }

        public IFilterVariablesProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<VariableInfo> GetVariablesInt32NullableOfInt32Delegate(int surveyId, int? filterId);
        public GetVariablesInt32NullableOfInt32Delegate GetVariablesInt32NullableOfInt32;

        List<VariableInfo> IFilterVariablesProvider.GetVariables(int surveyId, int? filterId)
        {


            if (GetVariablesInt32NullableOfInt32 != null)
            {
                return GetVariablesInt32NullableOfInt32(surveyId, filterId);
            } else if (_inner != null)
            {
                return ((IFilterVariablesProvider)_inner).GetVariables(surveyId, filterId);
            }

            return default(List<VariableInfo>);
        }

    }
}