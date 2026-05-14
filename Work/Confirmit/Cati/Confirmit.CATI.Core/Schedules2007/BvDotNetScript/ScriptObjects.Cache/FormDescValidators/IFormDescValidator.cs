namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators
{
    public interface IFormDescValidator
    {
        ValidationResult Validate(object validationData, string value);
    }
}