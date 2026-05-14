namespace Confirmit.CATI.Core.Misc
{
    public interface IProcessAndEnvironmentInfo
    {
        string ProcessName { get; }
        int ProcessId { get; }
        string MachineName { get; }
        string Version { get; }
        string Changeset { get; }
    }
}
