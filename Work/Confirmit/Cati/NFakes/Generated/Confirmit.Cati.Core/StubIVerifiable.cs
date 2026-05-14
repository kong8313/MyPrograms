using System;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling.Fakes
{
    public class StubIVerifiable : IVerifiable 
    {
        private IVerifiable _inner;

        public StubIVerifiable()
        {
            _inner = null;
        }

        public IVerifiable Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool ValidateErrorCollectionOutDelegate(out ErrorCollection errors);
        public ValidateErrorCollectionOutDelegate ValidateErrorCollectionOut;

        bool IVerifiable.Validate(out ErrorCollection errors)
        {
            errors = default(ErrorCollection);


            if (ValidateErrorCollectionOut != null)
            {
                return ValidateErrorCollectionOut(out errors);
            } else if (_inner != null)
            {
                return ((IVerifiable)_inner).Validate(out errors);
            }

            return default(bool);
        }

    }
}