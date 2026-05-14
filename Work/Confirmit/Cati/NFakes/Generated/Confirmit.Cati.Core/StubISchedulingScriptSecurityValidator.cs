using System;
using Confirmit.CATI.Core.Schedules2007.Validation;

namespace Confirmit.CATI.Core.Schedules2007.Validation.Fakes
{
    public class StubISchedulingScriptSecurityValidator : ISchedulingScriptSecurityValidator 
    {
        private ISchedulingScriptSecurityValidator _inner;

        public StubISchedulingScriptSecurityValidator()
        {
            _inner = null;
        }

        public ISchedulingScriptSecurityValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate SchedulingScriptSecurityValidatorResult ValidateStringDelegate(string assemblyFileName);
        public ValidateStringDelegate ValidateString;

        SchedulingScriptSecurityValidatorResult ISchedulingScriptSecurityValidator.Validate(string assemblyFileName)
        {


            if (ValidateString != null)
            {
                return ValidateString(assemblyFileName);
            } else if (_inner != null)
            {
                return ((ISchedulingScriptSecurityValidator)_inner).Validate(assemblyFileName);
            }

            return default(SchedulingScriptSecurityValidatorResult);
        }

    }
}