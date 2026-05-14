using System;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators.Fakes
{
    public class StubISchedulingObjectValidator : ISchedulingObjectValidator 
    {
        private ISchedulingObjectValidator _inner;

        public StubISchedulingObjectValidator()
        {
            _inner = null;
        }

        public ISchedulingObjectValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        bool ISchedulingObjectValidator.Validate<T>(T item, out ErrorCollection errors)
        {
            errors = default(ErrorCollection);


            return default(bool);
        }

        bool ISchedulingObjectValidator.ValidateWithCollection<T, TType>(BaseCollection<T, TType> baseCollection, T item, out ErrorCollection errors)
        {
            errors = default(ErrorCollection);


            return default(bool);
        }

    }
}