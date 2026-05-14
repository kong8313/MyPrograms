using System;

namespace Confirmit.CATI.Installation.Common
{
    public class ValidateException : Exception
    {
        public ValidateException(string message)
            : base(message)
        {
        }

        public ValidateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
