using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Confirmit;

namespace Confirmit.CATI.Supervisor.Core.Filters
{
    public interface IFilterVariablesProvider
    {
         /// <summary>
        /// Returns a list of variables for filtering with Confirmit variables.
        /// </summary>
        /// <param name="surveyId">Fusion survey SID.</param>
        /// <param name="filterId">Current filter's SID.</param>
         List<VariableInfo> GetVariables(int surveyId, int? filterId);
    }
}