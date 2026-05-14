using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.ConfigurationsApi
{
    public interface IConfigurationApiService
    {
        List<LanguageModel> GetLanguages();
    }
}