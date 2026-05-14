using System.Diagnostics;

namespace PreventLocking
{
    public class ProcessVerificator
    {
        public void VerifyAndRun()
        {
            var path = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\dfsvc.exe";
            var processName = "dfsvc";

            if (Process.GetProcessesByName(processName).Length == 0)
            {
                using (var scriptProcess = new Process())
                {
                    scriptProcess.StartInfo = new ProcessStartInfo(path)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    scriptProcess.Start();
                }
            }
        }
    }
}