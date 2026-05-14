using System;

namespace CatiOlympicPrepareTest.Constants
{
    public static class SurveyMode
    {
        public static string Predictive = "Predictive";

        public static string PreviewInPredictive = "PreviewInPredictive";
        
        public static string NoDialer = "NoDialer";

        public static bool IsDialerRequired(string mode)
        {
            if (string.Equals(mode, NoDialer, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
