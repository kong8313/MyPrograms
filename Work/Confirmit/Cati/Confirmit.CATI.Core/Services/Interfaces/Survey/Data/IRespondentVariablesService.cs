using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public interface IRespondentVariablesService
    {
        Dictionary<string, object> GetVariablesToSend(int surveyId, int respId);
        Dictionary<int, Dictionary<string, object>> GetVariablesToSend(int surveyId, List<int> respIds);
    }
}