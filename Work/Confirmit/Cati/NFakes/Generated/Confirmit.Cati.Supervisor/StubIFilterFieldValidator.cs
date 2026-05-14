using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Classes.Filters;

namespace Confirmit.CATI.Supervisor.Classes.Filters.Fakes
{
    public class StubIFilterFieldValidator : IFilterFieldValidator 
    {
        private IFilterFieldValidator _inner;

        public StubIFilterFieldValidator()
        {
            _inner = null;
        }

        public IFilterFieldValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ValidateBvFilterFieldsEntityDelegate(BvFilterFieldsEntity field);
        public ValidateBvFilterFieldsEntityDelegate ValidateBvFilterFieldsEntity;

        void IFilterFieldValidator.Validate(BvFilterFieldsEntity field)
        {

            if (ValidateBvFilterFieldsEntity != null)
            {
                ValidateBvFilterFieldsEntity(field);
            } else if (_inner != null)
            {
                ((IFilterFieldValidator)_inner).Validate(field);
            }
        }

    }
}