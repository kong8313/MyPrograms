using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Confirmit;

namespace Confirmit.CATI.Supervisor.Core.Filters
{
    public interface IFilterManager
    {
        /// <summary>
        /// Deletes the filter.
        /// </summary>
        /// <param name="filterSid">The filter SID.</param>
        void DeleteFilter(int filterSid);

        /// <summary>
        /// Returns list of filters without the current filter and its parent filters (to prevent circular references).
        /// </summary>
        /// <param name="surveyID">Current survey's Fusion ID.</param>
        /// <param name="currentFilterSid">Current filter's SID. null if there is no current filter (creating new filter).</param>
        IEnumerable<VariableInfo> GetFilters(int surveyID, int? currentFilterSid);
    }
}