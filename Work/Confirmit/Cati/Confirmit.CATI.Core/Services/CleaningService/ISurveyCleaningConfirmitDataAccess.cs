using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.CleaningService
{
    public interface ISurveyCleaningConfirmitDataAccess
    {
        void SetCreators(List<CleaningServiceEmailInfo> emailInfo);
    }
}