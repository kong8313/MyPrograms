using System;

namespace Confirmit.CATI.Supervisor.Classes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class StoreInViewStateAttribute : Attribute
    {
    }
}