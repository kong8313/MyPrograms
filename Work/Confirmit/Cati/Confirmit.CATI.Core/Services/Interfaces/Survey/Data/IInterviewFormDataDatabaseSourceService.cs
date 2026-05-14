namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public interface IInterviewFormDataDatabaseSourceService : IInterviewFormDataSourceService
    {
        string GetDiff();
    }
}