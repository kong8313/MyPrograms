namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators
{
    public class ValidationResult
    {
        public bool IsSuccess { get; private set; }

        public string ErrorMessage { get; private set; }

        private ValidationResult()
        {
            IsSuccess = true;
        }

        private ValidationResult(string errorMsg)
        {
            IsSuccess = false;
            ErrorMessage = errorMsg;
        }

        public static ValidationResult Success()
        {
           return new ValidationResult();
        }

        public static ValidationResult Error(string errorMsg)
        {
            return new ValidationResult(errorMsg);
        }
    }
}