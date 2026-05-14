using System;

namespace Confirmit.CATI.Supervisor.Classes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CheckSurveyPermissionAttribute : Attribute
    {
        public CheckSurveyPermissionAttribute()
        {
            IsRequired = true;
        }

        public bool IsRequired
        {
            get;
            set;
        }

        public string RequestParameterName
        {
            get;
            set;
        }

        public string SeparatorCharacter
        {
            get;
            set;
        }        
    }
}
