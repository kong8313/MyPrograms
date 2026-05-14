namespace Confirmit.CATI.Core.Misc
{
    public interface ICommandLineParser
    {
        int GetCompanyId(string[] commandLineArgs);
    }
}