using System;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement.Fakes
{
    public class StubIcallManagementViewProvider : ICallManagementViewsProvider 
    {
        private ICallManagementViewsProvider _inner;

        public StubIcallManagementViewProvider()
        {
            _inner = null;
        }

        public ICallManagementViewsProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate CallManagementViews SetDefaultViewsCallManagementViewsDelegate(CallManagementViews callManagementViews);
        public SetDefaultViewsCallManagementViewsDelegate SetDefaultViewsCallManagementViews;

        CallManagementViews ICallManagementViewsProvider.SetDefaultViews(CallManagementViews callManagementViews)
        {


            if (SetDefaultViewsCallManagementViews != null)
            {
                return SetDefaultViewsCallManagementViews(callManagementViews);
            } else if (_inner != null)
            {
                return ((ICallManagementViewsProvider)_inner).SetDefaultViews(callManagementViews);
            }

            return default(CallManagementViews);
        }

        private List<CallManagementColumnKey> _ScheduledColumnKeys;
        public Func<List<CallManagementColumnKey>> ScheduledColumnKeysGet;
        public Action<List<CallManagementColumnKey>> ScheduledColumnKeysSetListOfCallManagementColumnKey;

        List<CallManagementColumnKey> ICallManagementViewsProvider.ScheduledColumnKeys
        {
            get
            {
                if (ScheduledColumnKeysGet != null)
                {
                    return ScheduledColumnKeysGet();
                } else if (_inner != null)
                {
                    return ((ICallManagementViewsProvider)_inner).ScheduledColumnKeys;
                }

                if (ScheduledColumnKeysSetListOfCallManagementColumnKey == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ScheduledColumnKeys;
                }

                return default(List<CallManagementColumnKey>);
            }

        }

    }
}