using System;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Backend.WebApiServices
{
    [Serializable]
    public class WebApiDisabledException :Exception
    {
        public WebApiDisabledException()
        {
        }

        public WebApiDisabledException(string message)
            : base(message)
        {
        }
        public WebApiDisabledException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected WebApiDisabledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
