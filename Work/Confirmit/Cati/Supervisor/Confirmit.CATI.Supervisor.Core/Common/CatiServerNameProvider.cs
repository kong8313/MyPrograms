using System;
using Confirmit.Configuration;

namespace Confirmit.CATI.Supervisor.Core.Common
{
    public class CatiServerNameProvider : ICatiServerNameProvider
    {
        public string Get()
        {
            var multimodeBaseUrl = ConfirmitConfiguration.GetStringValue("MultimodeBaseURL", string.Empty);
            return new Uri(multimodeBaseUrl).GetComponents(UriComponents.Host, UriFormat.Unescaped);
        }
    }
}