using System;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators.Fakes
{
    public class StubIFormDescValidator : IFormDescValidator 
    {
        private IFormDescValidator _inner;

        public StubIFormDescValidator()
        {
            _inner = null;
        }

        public IFormDescValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate ValidationResult ValidateObjectStringDelegate(Object validationData, string value);
        public ValidateObjectStringDelegate ValidateObjectString;

        ValidationResult IFormDescValidator.Validate(Object validationData, string value)
        {


            if (ValidateObjectString != null)
            {
                return ValidateObjectString(validationData, value);
            } else if (_inner != null)
            {
                return ((IFormDescValidator)_inner).Validate(validationData, value);
            }

            return default(ValidationResult);
        }

    }
}