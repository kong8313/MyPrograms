using System.Diagnostics;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace RunTestParallelUtility.Interfaces
{
    public interface IProcessManager
    {
        void KillProcessTree(Process process, ILogger logger);
    }
}
