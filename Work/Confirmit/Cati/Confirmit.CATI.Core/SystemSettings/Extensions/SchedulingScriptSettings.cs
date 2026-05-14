using System;
using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Core.SystemSettings
{
    public partial interface ISchedulingScriptSettings
    {
        List<string> SecureExternalMethodList { get; }
    }

    public partial class SchedulingScriptSettings
    {
        private List<string> _secureExternalMethodList;
        public List<string> SecureExternalMethodList
        {
            get
            {
                if (_secureExternalMethodList == null)
                {
                    _secureExternalMethodList = new List<string>();
                    
                    _secureExternalMethodList.AddRange(Schedules2007.Validation.Resources.Resource.SecureExternalMethods
                        .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace("\r", "").Replace("\n", "").Trim()));
                    
                    _secureExternalMethodList.AddRange(SecureExternalMethods
                        .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                }

                return _secureExternalMethodList;
            }
        }

        partial void OnSettingsChanged()
        {
            _secureExternalMethodList = null;
        }
    }
}
