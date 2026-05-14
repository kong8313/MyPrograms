using System;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace BootstrapperLibrary.Interfaces
{
    public interface ISelectActionForm
    {        
        CommandLineParseResult ShowForm(
           ILogger logger,
           Version currentVersion,
           IInstalledProductSearcher installedProductSearcher,
           IObjectFactory objectFactory,
           IMsiParametersStringCreator msiParametersStringCreator);
    }
}