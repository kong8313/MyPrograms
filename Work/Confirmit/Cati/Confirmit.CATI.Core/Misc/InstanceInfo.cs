namespace Confirmit.CATI.Core.Misc
{
    public class InstanceInfo : IInstanceInfo
    {
        public bool IsExecutedInBackendInstance
        {
            get { return BackendInstance.Current.IsExecutedInBackendInstance; }
        }

        public bool IsDefaultInstance
        {
            get { return BackendInstance.Current.IsDefaultInstance; }
        }
    }
}