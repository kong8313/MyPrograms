using System;
using Confirmit.CATI.Core.Validators.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Validators.Interfaces.Fakes
{
    public class StubIPersonGroupValidator : IPersonGroupValidator 
    {
        private IPersonGroupValidator _inner;

        public StubIPersonGroupValidator()
        {
            _inner = null;
        }

        public IPersonGroupValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool IsNameValidStringDelegate(string name);
        public IsNameValidStringDelegate IsNameValidString;

        bool IPersonGroupValidator.IsNameValid(string name)
        {


            if (IsNameValidString != null)
            {
                return IsNameValidString(name);
            } else if (_inner != null)
            {
                return ((IPersonGroupValidator)_inner).IsNameValid(name);
            }

            return default(bool);
        }

        public delegate bool IsValidBvPersonGroupEntityDelegate(BvPersonGroupEntity personGroup);
        public IsValidBvPersonGroupEntityDelegate IsValidBvPersonGroupEntity;

        bool IPersonGroupValidator.IsValid(BvPersonGroupEntity personGroup)
        {


            if (IsValidBvPersonGroupEntity != null)
            {
                return IsValidBvPersonGroupEntity(personGroup);
            } else if (_inner != null)
            {
                return ((IPersonGroupValidator)_inner).IsValid(personGroup);
            }

            return default(bool);
        }

    }
}