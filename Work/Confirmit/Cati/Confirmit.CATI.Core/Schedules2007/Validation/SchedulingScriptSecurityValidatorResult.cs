namespace Confirmit.CATI.Core.Schedules2007.Validation
{
    public class SchedulingScriptSecurityValidatorResult
    {
        public SchedulingScriptSecurityValidatorResult(string[] unsecureCalls)
        {
            UnsecureCalls = unsecureCalls;
        }

        public bool IsSecure
        {
            get
            {
                return UnsecureCalls.Length == 0;
            }
        }

        public bool IsUnsecure
        {
            get
            {
                return !IsSecure;
            }
        }

        public string[] UnsecureCalls { get; private set; }
    }
}