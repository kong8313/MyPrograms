namespace Confirmit.CATI.Installation.Common
{
    public enum MachineConfigChangingState
    { 
        DoNotChange,
        SetAutoConfigTrue,
        SetCustomSettings
    }

    public class MachineConfigProperties
    {
        public MachineConfigChangingState MachineConfigChanging { get; private set; }
        public string MinWorkerThreads {get; private set; }
        public string MaxWorkerThreads {get; private set; }
        public string MinIoThreads {get; private set; }
        public string MaxIoThreads {get; private set; }
        public string MinFreeThreads {get; private set; }
        public string MinLocalRequestFreeThreads {get; private set; }

        public MachineConfigProperties(
            MachineConfigChangingState machineConfigChanging,
            string minWorkerThreads,
            string maxWorkerThreads,
            string minIoThreads,
            string maxIoThreads,
            string minFreeThreads,
            string minLocalRequestFreeThreads)
        {
            MachineConfigChanging = machineConfigChanging;
            MinWorkerThreads = minWorkerThreads ?? string.Empty;
            MaxWorkerThreads = maxWorkerThreads ?? string.Empty;
            MinIoThreads = minIoThreads ?? string.Empty;
            MaxIoThreads = maxIoThreads ?? string.Empty;
            MinFreeThreads = minFreeThreads ?? string.Empty;
            MinLocalRequestFreeThreads = minLocalRequestFreeThreads ?? string.Empty;
        }
    }
}
