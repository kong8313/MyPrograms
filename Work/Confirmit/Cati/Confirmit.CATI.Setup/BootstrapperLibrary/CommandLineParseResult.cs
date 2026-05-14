using System.Collections.Generic;

namespace BootstrapperLibrary
{
    public class CommandLineParseResult
    {
        /// <summary>
        /// What should we do: install/uninstall/update
        /// </summary>
        public InstallationAction Action;       

        /// <summary>
        /// Parameter string for call msiexec in passive mode
        /// </summary>
        public string MsiPropertiesForUnattendedInstallation = string.Empty;
       
        /// <summary>
        /// Update version even if the existed version is bigger or the same with the current one
        /// </summary>
        public bool IgnoreVersion;
    }
}
