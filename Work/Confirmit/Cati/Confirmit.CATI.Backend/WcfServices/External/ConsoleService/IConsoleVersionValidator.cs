namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    public interface IConsoleVersionValidator
    {
        void ValidateVersion(string version);
        bool IsLatestVersion(string version);
    }
}