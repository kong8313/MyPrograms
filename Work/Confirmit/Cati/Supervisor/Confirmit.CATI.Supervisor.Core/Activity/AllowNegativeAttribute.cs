using System;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AllowNegativeAttribute : Attribute
    {
    }
}