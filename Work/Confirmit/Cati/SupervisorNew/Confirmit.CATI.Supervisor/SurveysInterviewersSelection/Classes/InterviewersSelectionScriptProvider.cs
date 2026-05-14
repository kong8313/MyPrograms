using System;
using Confirmit.CATI.Supervisor.Classes.Activity;

namespace Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes
{
    public class InterviewersSelectionScriptProvider
    {
        public static string Get(SourceList sourceList)
        {
            return Get(sourceList, string.Empty, null, null);
        }

        public static string Get(SourceList sourceList, string updatePanelClientId, int? selectedInterviewerId)
        {
            return Get(sourceList, updatePanelClientId, selectedInterviewerId, null);
        }
        public static string Get(SourceList sourceList, string updatePanelClientId, int? selectedInterviewerId, string postbackReference)
        {
            var pageUrl = GetInterviewersSelectionPageUrl();

            return String.Format("Common.selectInterviewers('{0}', {1}, '{2}', {3}, '{4}');",
                                  updatePanelClientId,
                                  (int)sourceList,
                                  selectedInterviewerId.HasValue ? selectedInterviewerId.ToString() : String.Empty,
                                  string.IsNullOrEmpty(postbackReference) ? "''" : postbackReference,
                                  pageUrl);
        }

        private static string GetInterviewersSelectionPageUrl()
        {
            return "SurveysInterviewersSelection/InterviewersSelectionPage.aspx";
        }
    }
}
