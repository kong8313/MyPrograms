namespace Confirmit.CATI.Supervisor.Classes.PageDataProviders.Surveys
{
    internal class SurveyToPersonAssignmentPageProviderFactory
    {
        public ISurveyPersonAssignmentPageProvider GetProvider(bool replaceAssignment)
        {
            return replaceAssignment
                 ? (ISurveyPersonAssignmentPageProvider)new ReplaceSurveyPersonAssignmentPageProvider()
                 : new AddSurveyPersonAssignmentPageProvider();
        }
    }
}