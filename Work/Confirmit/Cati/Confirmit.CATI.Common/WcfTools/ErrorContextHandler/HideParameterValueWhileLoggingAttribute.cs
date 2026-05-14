using System;

namespace Confirmit.CATI.Common.WcfTools.ErrorContextHandler
{
    /// <summary>
    /// Attribute to set for method parameters not to log them.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class HideParameterValueWhileLoggingAttribute : Attribute
    {
    }
} 
