namespace TeamCityBuildEngine.Interfaces
{
    public interface IExternalExecutor
    {
        int ExitCode { get; }

        void Invoke(string scriptNameOrPath, string args, int delay = -1);
    }
}
