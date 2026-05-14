using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Classes.Filters;

namespace Confirmit.CATI.Supervisor.Classes.Filters.Fakes
{
    public class StubIFilterCyclicReferenceValidator : IFilterCyclicReferenceValidator 
    {
        private IFilterCyclicReferenceValidator _inner;

        public StubIFilterCyclicReferenceValidator()
        {
            _inner = null;
        }

        public IFilterCyclicReferenceValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ValidateBvFiltersEntityIEnumerableOfBvFilterFieldsEntityDelegate(BvFiltersEntity filter, IEnumerable<BvFilterFieldsEntity> fields);
        public ValidateBvFiltersEntityIEnumerableOfBvFilterFieldsEntityDelegate ValidateBvFiltersEntityIEnumerableOfBvFilterFieldsEntity;

        void IFilterCyclicReferenceValidator.Validate(BvFiltersEntity filter, IEnumerable<BvFilterFieldsEntity> fields)
        {

            if (ValidateBvFiltersEntityIEnumerableOfBvFilterFieldsEntity != null)
            {
                ValidateBvFiltersEntityIEnumerableOfBvFilterFieldsEntity(filter, fields);
            } else if (_inner != null)
            {
                ((IFilterCyclicReferenceValidator)_inner).Validate(filter, fields);
            }
        }

    }
}