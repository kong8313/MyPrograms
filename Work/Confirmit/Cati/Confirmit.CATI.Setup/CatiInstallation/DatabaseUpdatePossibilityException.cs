using System;

namespace CatiInstallation
{
    public class DatabaseUpdatePossibilityException : Exception
    {
        public DatabaseUpdatePossibilityException(string message)
            : base(message)
        {
            
        }
    }
}
