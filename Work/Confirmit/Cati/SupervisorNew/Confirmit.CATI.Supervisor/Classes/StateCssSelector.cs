namespace Confirmit.CATI.Supervisor.Classes
{
    public static class StateCssSelector
    {
        public static string Get(string stateName)
        {
            if (stateName == "Completed")
            {
                return " greenFont";
            }
            
            if (stateName == "Interrupted by system" ||
                stateName == "Interrupted by interviewer" ||
                stateName == "Error" ||
                stateName == "Survey script error" ||
                stateName == "Telephony failure")
            {
                return " redFont";
            }

            return "";
        }
    }
}