using System;

namespace Confirmit.CATI.Common.WcfTools.ErrorContextHandler
{
    public sealed class ErrorContext
    {
        public object[] Parameters { get; set; }

        public string Action { get; set; }

        public string IdentityName { get; set; }

        public Type ServiceType { get; set; }

        public string ServiceName { get; set; }

        public string ServiceNamespace { get; set; }

        public string ToHeader { get; set; }

        public string MethodName { get; set; }
    }
}