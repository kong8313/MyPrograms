using System;
using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.Exceptions;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Supervisor.Core.Filters
{
    // TODO: Move to FilterService / FilterRepository.

    /// <summary>
    /// Class in responsible for common operations with filters.
    /// </summary>
    public class FilterManager : IFilterManager
    {
        private readonly IFilterRepository _filterRepository;

        public FilterManager() :
            this(ServiceLocator.Resolve<IFilterRepository>())
        {
            
        }

        public FilterManager(IFilterRepository filterRepository)
        {
            _filterRepository = filterRepository;
        }

        public void DeleteFilter(int filterSid)
        {
            if (filterSid <= 0)
            {
                throw new ArgumentOutOfRangeException("filterSid");
            }

            var filter = _filterRepository.GetById(filterSid);

            var evt = new DeleteFilterEvent(filterSid, filter.Name);

            var filters = BvSpFilter_GetDependentFiltersAdapter.ExecuteEntityList(filterSid);

            if (filters.Count > 0)
            {
                throw new FilterIsUsedException(filters.Select(x => x.Name));
            }

            FilterRepository.Delete(filterSid);

            evt.Finish();
        }

        public IEnumerable<VariableInfo> GetFilters(int surveyID, int? currentFilterSid)
        {
            var parentFilters = currentFilterSid.HasValue
                                    ? _filterRepository.GetAllParentFilters(currentFilterSid.Value)
                                    : new List<int>();

            return _filterRepository.GetFiltersList(true, surveyID).Where(
                    filter => filter.SID != currentFilterSid && !parentFilters.Contains(filter.SID)).Select(
                        filter =>
                        new VariableInfo(
                            filter.Name,
                            VariableTypes.Subfilter,
                            TableTypes.Subfilter,
                            filter.SID.ToString(),
                            filter.SID.ToString())).ToList();
        }
    }
}