using Confirmit.CATI.Installation.Common.Interfaces;


namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces
{
    public interface IPowerShellScriptExecutor
    {
        string Execute(ILogger logger, string scriptText);
    }
}
