using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Classes.Filters;

namespace Confirmit.CATI.Supervisor.Classes.Filters.Fakes
{
    public class StubIFilterValidator : IFilterValidator 
    {
        private IFilterValidator _inner;

        public StubIFilterValidator()
        {
            _inner = null;
        }

        public IFilterValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ValidateBvFiltersEntityDelegate(BvFiltersEntity filter);
        public ValidateBvFiltersEntityDelegate ValidateBvFiltersEntity;

        void IFilterValidator.Validate(BvFiltersEntity filter)
        {

            if (ValidateBvFiltersEntity != null)
            {
                ValidateBvFiltersEntity(filter);
            } else if (_inner != null)
            {
                ((IFilterValidator)_inner).Validate(filter);
            }
        }

    }
}