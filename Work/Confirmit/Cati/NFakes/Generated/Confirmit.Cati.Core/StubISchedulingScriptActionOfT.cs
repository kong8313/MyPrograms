using System;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces;
using BvDotNetScript.ScriptObjects;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces.Fakes
{
    public class StubISchedulingScriptAction<T> : ISchedulingScriptAction<T> 
    {
        private ISchedulingScriptAction<T> _inner;

        public StubISchedulingScriptAction()
        {
            _inner = null;
        }

        public ISchedulingScriptAction<T> Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExecuteExtendedSchedulingAPITDelegate(ExtendedSchedulingAPI api, T parameter);
        public ExecuteExtendedSchedulingAPITDelegate ExecuteExtendedSchedulingAPIT;

        void ISchedulingScriptAction<T>.Execute(ExtendedSchedulingAPI api, T parameter)
        {

            if (ExecuteExtendedSchedulingAPIT != null)
            {
                ExecuteExtendedSchedulingAPIT(api, parameter);
            } else if (_inner != null)
            {
                ((ISchedulingScriptAction<T>)_inner).Execute(api, parameter);
            }
        }

    }
}