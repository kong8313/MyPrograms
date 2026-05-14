using System;

namespace BootstrapperLibrary.Interfaces
{
    public interface IInstalledProductSearcher
    {
        string CurrentProductName { get; }
        bool IsProductAlreadyInstalled { get; }
        Version InstalledVersion { get; }
        string ProductCode { get; }
        string ProductName { get; }
        string InstallLocation { get; }
    }
}