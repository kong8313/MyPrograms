using System;

namespace ClickOnceConsoleInstallationTest
{
    public class ConsoleLogger
    {
        public void Log(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss.ms") + " "  + message);
        }
    }
}