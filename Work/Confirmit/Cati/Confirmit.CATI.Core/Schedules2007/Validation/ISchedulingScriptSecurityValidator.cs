namespace Confirmit.CATI.Core.Schedules2007.Validation
{
    public interface ISchedulingScriptSecurityValidator
    {
        SchedulingScriptSecurityValidatorResult Validate(string assemblyFileName);
    }
}