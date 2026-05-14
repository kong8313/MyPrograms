using System;
using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces.Fakes
{
    public class StubISchedulingScriptAction : ISchedulingScriptAction 
    {
        private ISchedulingScriptAction _inner;

        public StubISchedulingScriptAction()
        {
            _inner = null;
        }

        public ISchedulingScriptAction Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExecuteExtendedSchedulingAPIDelegate(ExtendedSchedulingAPI api);
        public ExecuteExtendedSchedulingAPIDelegate ExecuteExtendedSchedulingAPI;

        void ISchedulingScriptAction.Execute(ExtendedSchedulingAPI api)
        {

            if (ExecuteExtendedSchedulingAPI != null)
            {
                ExecuteExtendedSchedulingAPI(api);
            } else if (_inner != null)
            {
                ((ISchedulingScriptAction)_inner).Execute(api);
            }
        }

    }
}