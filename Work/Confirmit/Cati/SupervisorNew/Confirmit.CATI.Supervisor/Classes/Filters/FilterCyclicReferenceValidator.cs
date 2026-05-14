using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    public class FilterCyclicReferenceValidator : IFilterCyclicReferenceValidator
    {
        private readonly IFilterRepository _filterRepository;

        public FilterCyclicReferenceValidator() :
            this(ServiceLocator.Resolve<IFilterRepository>())
        {
            
        }

        public FilterCyclicReferenceValidator(IFilterRepository filterRepository)
        {
            _filterRepository = filterRepository;
        }

        public void Validate(BvFiltersEntity filter, IEnumerable<BvFilterFieldsEntity> fields)
        {
            if (filter.SID == 0)
            {
                return;
            }

            List<int> dependentFiltersSids = _filterRepository.GetAllParentFilters(filter.SID);
            foreach (BvFilterFieldsEntity field in fields.Where(field => field.Table == (int)TableTypes.Subfilter))
            {
                var fieldIdValue = Int32.Parse(field.Value);
                if (dependentFiltersSids.Contains(fieldIdValue))
                {
                    var parentFilter = FilterRepository.GetById(fieldIdValue);
                    throw new UserMessageException(
                        string.Format(Strings.CannotInsertSubfilterCircularReferenceFound, parentFilter.Name, filter.Name));
                }
            }
        }
    }
}