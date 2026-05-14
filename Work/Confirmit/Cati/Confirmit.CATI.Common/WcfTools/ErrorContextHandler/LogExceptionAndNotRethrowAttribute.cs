using System;

namespace Confirmit.CATI.Common.WcfTools.ErrorContextHandler
{
    /// <summary>
    /// When applied to WCF service method - suppress all exceptions occurred
    /// during execution of the method and logs them using <see cref="ErrorContextHandler"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LogExceptionAndNotReThrowAttribute: Attribute
    {
    }
}