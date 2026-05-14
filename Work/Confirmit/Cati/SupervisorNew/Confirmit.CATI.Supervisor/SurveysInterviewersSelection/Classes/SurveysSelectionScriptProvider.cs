using System;
using Confirmit.CATI.Supervisor.Classes.Activity;

namespace Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes
{
    public class SurveysSelectionScriptProvider
    {
        public static string Get(SourceList sourceList)
        {
            return Get(sourceList, String.Empty, null, string.Empty);
        }

        public static string Get(SourceList sourceList, string updatePanelClientId)
        {
            return Get(sourceList, updatePanelClientId, null, string.Empty);
        }

        public static string Get(SourceList sourceList, int? selectedSurveyId, string postbackReference)
        {
            return Get(sourceList, null, selectedSurveyId, postbackReference);
        }

        private static string Get(SourceList sourceList, string updatePanelClientId, int? selectedSurveyId, string postbackReference)
        {
            var pageUrl = GetSurveySelectionPageUrl();

            return String.Format("Common.selectSurveys('{0}', {1}, '{2}', {3}, '{4}');",
                                  updatePanelClientId,
                                  (int)sourceList,
                                  selectedSurveyId.HasValue ? selectedSurveyId.ToString() : String.Empty,
                                  string.IsNullOrEmpty(postbackReference) ? "''" : postbackReference,
                                  pageUrl);
        }

        private static string GetSurveySelectionPageUrl()
        {
            return "SurveysInterviewersSelection/SurveysSelectionPage.aspx";

        }
    }
}