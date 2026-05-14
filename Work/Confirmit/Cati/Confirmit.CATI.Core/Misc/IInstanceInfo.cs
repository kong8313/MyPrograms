namespace Confirmit.CATI.Core.Misc
{
    public interface IInstanceInfo
    {
        bool IsExecutedInBackendInstance { get; }
        bool IsDefaultInstance { get; }
    }
}