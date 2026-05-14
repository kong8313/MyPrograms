using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.InterviewServiceImplementation
{
    public interface IRespondentObtainer
    {
        RespondentRecord GetRespondent(BvSurveyEntity survey, int respId);
    }
}