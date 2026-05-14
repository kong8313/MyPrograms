using System;
using System.Diagnostics;

namespace RunTestParallelUtility
{
    public class ResultFileIsReadyArgs : EventArgs
    {
        public ResultFileIsReadyArgs(Process process)
        {
            Process = process;
        }

        public Process Process { get; private set; }
    }
}