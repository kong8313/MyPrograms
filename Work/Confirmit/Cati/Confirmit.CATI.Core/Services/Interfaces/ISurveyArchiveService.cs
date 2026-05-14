using System.Threading;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface ISurveyArchiveService
    {
        string Archive(BvSurveyEntity survey);
        string Restore(int surveyId, string data, CancellationToken cancellationToken);
    }
}