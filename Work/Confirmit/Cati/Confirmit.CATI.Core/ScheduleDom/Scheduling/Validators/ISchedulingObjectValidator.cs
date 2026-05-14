namespace Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators
{
    public interface ISchedulingObjectValidator
    {
        bool Validate<T>(T item, out ErrorCollection errors);

        bool ValidateWithCollection<T, TType>(BaseCollection<T, TType> baseCollection, T item, out ErrorCollection errors)
            where T : BaseObject<TType>
            where TType : struct;
    }
}