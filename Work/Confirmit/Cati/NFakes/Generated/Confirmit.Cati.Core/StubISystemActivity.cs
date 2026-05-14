using System;
using Confirmit.CATI.Core.ActivityLogging.Authoring;

namespace Confirmit.CATI.Core.ActivityLogging.Authoring.Fakes
{
    public class StubISystemActivity : ISystemActivity 
    {
        private ISystemActivity _inner;

        public StubISystemActivity()
        {
            _inner = null;
        }

        public ISystemActivity Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AddSystemActivitySystemActivityLogItemDelegate(SystemActivityLogItem log);
        public AddSystemActivitySystemActivityLogItemDelegate AddSystemActivitySystemActivityLogItem;

        void ISystemActivity.AddSystemActivity(SystemActivityLogItem log)
        {

            if (AddSystemActivitySystemActivityLogItem != null)
            {
                AddSystemActivitySystemActivityLogItem(log);
            } else if (_inner != null)
            {
                ((ISystemActivity)_inner).AddSystemActivity(log);
            }
        }

    }
}