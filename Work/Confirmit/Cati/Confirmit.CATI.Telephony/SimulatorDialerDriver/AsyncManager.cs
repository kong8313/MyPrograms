using System;
using System.Threading;
using ConfirmitDialerInterface;

namespace SimulatorDialerDriver
{
    internal static class AsyncManager
    {
        public static void Execute(ILogger logger, Action action)
        {
            var thread = new Thread(x =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        logger.Error("AsyncManager.Execute", ex.ToString());
                    }
                }) { IsBackground = true };

            thread.Start();
        }
    }
}
