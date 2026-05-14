using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    internal class FilterFactory : IFilterFactory
    {
        private readonly IFilterValidator _validator;
        private readonly IFilterRepository _filterRepository;

        public FilterFactory(IFilterValidator validator, IFilterRepository filterRepository)
        {
            if (validator == null)
            {
                throw new ArgumentNullException("validator");
            }

            if (filterRepository == null)
            {
                throw new ArgumentNullException("filterRepository");
            }

            _validator = validator;
            _filterRepository = filterRepository;
        }

        public BvFiltersEntity Create(int id, string name, string description, string operatorString)
        {
            BvFiltersEntity result = (id == Int32.MinValue ? new BvFiltersEntity() : _filterRepository.GetById(id));
            result.Name = name;
            result.Description = description;
            result.AndOrOperator = Convert.ToByte(operatorString);

            _validator.Validate(result);

            return result;
        }
    }
}