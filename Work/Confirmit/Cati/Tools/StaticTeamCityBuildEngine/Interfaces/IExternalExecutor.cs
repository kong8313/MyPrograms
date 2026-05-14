namespace StaticTeamCityBuildEngine.Interfaces
{
    public interface IExternalExecutor
    {
        void Invoke(string scriptNameOrPath, string args, int delay = -1);

        string ExecuteGitUtility(string command);
    }
}
