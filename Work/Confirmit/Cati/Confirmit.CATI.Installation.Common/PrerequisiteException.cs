using System;

namespace Confirmit.CATI.Installation.Common
{
    public class PrerequisiteException : Exception
    {
        public PrerequisiteException(string message)
            : base(message)
        {
        }
    }
}
