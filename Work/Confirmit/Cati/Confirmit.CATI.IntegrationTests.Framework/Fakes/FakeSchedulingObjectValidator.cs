using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;

namespace Confirmit.CATI.IntegrationTests.Framework.Fakes
{
    public class FakeSchedulingObjectValidator : ISchedulingObjectValidator
    {
        private ISchedulingObjectValidator _inner;

        public FakeSchedulingObjectValidator()
        {
            _inner = null;
        }

        public ISchedulingObjectValidator Inner
        {
            set { _inner = value; }
            get { return _inner; }
        }

        bool ISchedulingObjectValidator.Validate<T>(T item, out ErrorCollection errors)
        {
            if(_inner != null)
            {
                return _inner.Validate(item, out errors);
            }
            
            errors = new ErrorCollection();
            return true;
        }

        bool ISchedulingObjectValidator.ValidateWithCollection<T, TType>(BaseCollection<T, TType> baseCollection, T item, out ErrorCollection errors)
        {
            if(_inner != null)
            {
                return _inner.ValidateWithCollection(baseCollection, item, out errors);
            }

            errors = new ErrorCollection();
            return true;
        }
    }
}