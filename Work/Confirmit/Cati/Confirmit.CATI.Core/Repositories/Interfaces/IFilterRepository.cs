using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IFilterRepository
    {
        [CanBeNull]
        BvFiltersEntity GetById(int sid);

        List<BvFiltersEntity> GetFiltersList(bool includeSiteWide, int surveyId);

        /// <summary>
        /// Recursively gets the IDs of all filters, dependent from the specified.
        /// </summary>
        /// <param name="filterSid">The filter SID.</param>
        List<int> GetAllParentFilters(int filterSid);
    }
}