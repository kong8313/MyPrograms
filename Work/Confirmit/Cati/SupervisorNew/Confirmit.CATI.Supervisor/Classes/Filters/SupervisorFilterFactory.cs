using System;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    internal class SupervisorFilterFactory : ISupervisorFilterFactory
    {
        private readonly IFilterFactory _filterFactory;
        private readonly IFilterFieldsFactory _fieldsFactory;
        private readonly IFilterCyclicReferenceValidator _cyclicValidator;

        public SupervisorFilterFactory([NotNull] IFilterFactory filterFactory,
                                       [NotNull] IFilterFieldsFactory fieldsFactory,
                                       [NotNull] IFilterCyclicReferenceValidator cyclicValidator)
        {
            _filterFactory = filterFactory;
            _fieldsFactory = fieldsFactory;
            _cyclicValidator = cyclicValidator;

            if (filterFactory == null) throw new ArgumentNullException("filterFactory");
            if (fieldsFactory == null) throw new ArgumentNullException("fieldsFactory");
            if (cyclicValidator == null) throw new ArgumentNullException("cyclicValidator");
        }

        public FilterData Create(int id, string name, string description, string operatorString, string fieldsXml)
        {
            var filter = _filterFactory.Create(id, name, description, operatorString);
            var fields = _fieldsFactory.Create(fieldsXml);
            _cyclicValidator.Validate(filter, fields);

            return new FilterData {Filter = filter, Fields = fields};
        }
    }
}