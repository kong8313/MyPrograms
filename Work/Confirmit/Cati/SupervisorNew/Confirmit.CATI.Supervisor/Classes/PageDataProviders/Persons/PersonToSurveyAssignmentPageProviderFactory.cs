namespace Confirmit.CATI.Supervisor.Classes.PageDataProviders.Persons
{
    internal class PersonToSurveyAssignmentPageProviderFactory
    {
        public IPersonSurveyAssignmentPageProvider GetProvider(bool replaceAssignment)
        {
            return replaceAssignment
                 ? (IPersonSurveyAssignmentPageProvider)new ReplacePersonSurveyAssignmentPageProvider()
                 : new AddPersonSurveyAssignmentPageProvider();
        }
    }
}