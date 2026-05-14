namespace Confirmit.CATI.Installation.Common.Interfaces
{
    public interface IPathProvider
    {
        string GetSqlPackageUtilityPath();

        string GetPathToGit();

        string GetPathToMsBuild();

        string GetStartupPath();

        string GetPathToMsTest();
    }
}