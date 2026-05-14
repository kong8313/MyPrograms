using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.Filters;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations
{
    internal abstract class CallManagementFilteredBatch : FilteredDatabaseBatch
    {
        protected CallManagementFilteredBatch(
            int surveyId, int subFilterId, int timezoneId, CallStates callState, SearchParameterCollection searchParams)
        {
            // TODO: Virtual method call in the constructor
            FilterGenerateMode mode = GetFilterGenerationMode(callState);

            using (var filterHelper = new FilterHelper(surveyId, subFilterId, timezoneId, searchParams))
            {
                Init(surveyId, filterHelper.FilterID, mode);
            }
        }

        protected abstract FilterGenerateMode GetFilterGenerationMode(CallStates callState);
    }
}
