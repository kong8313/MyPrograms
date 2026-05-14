using System;
using Confirmit.CATI.Supervisor.Core.ConfigurationsApi;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.ConfigurationsApi.Fakes
{
    public class StubIConfigurationApiService : IConfigurationApiService 
    {
        private IConfigurationApiService _inner;

        public StubIConfigurationApiService()
        {
            _inner = null;
        }

        public IConfigurationApiService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<LanguageModel> GetLanguagesDelegate();
        public GetLanguagesDelegate GetLanguages;

        List<LanguageModel> IConfigurationApiService.GetLanguages()
        {


            if (GetLanguages != null)
            {
                return GetLanguages();
            } else if (_inner != null)
            {
                return ((IConfigurationApiService)_inner).GetLanguages();
            }

            return default(List<LanguageModel>);
        }

    }
}