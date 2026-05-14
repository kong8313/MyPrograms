using System;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement.Fakes
{
    public class StubICallManagementViewsProvider : ICallManagementViewsProvider 
    {
        private ICallManagementViewsProvider _inner;

        public StubICallManagementViewsProvider()
        {
            _inner = null;
        }

        public ICallManagementViewsProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate CallManagementViews GetDefaultViewsDelegate();
        public GetDefaultViewsDelegate GetDefaultViews;

        CallManagementViews ICallManagementViewsProvider.GetDefaultViews()
        {


            if (GetDefaultViews != null)
            {
                return GetDefaultViews();
            } else if (_inner != null)
            {
                return ((ICallManagementViewsProvider)_inner).GetDefaultViews();
            }

            return default(CallManagementViews);
        }

        public delegate CallManagementViews MergeViewsCallManagementViewsCallManagementViewsDelegate(CallManagementViews defaultViews, CallManagementViews customViews);
        public MergeViewsCallManagementViewsCallManagementViewsDelegate MergeViewsCallManagementViewsCallManagementViews;

        CallManagementViews ICallManagementViewsProvider.MergeViews(CallManagementViews defaultViews, CallManagementViews customViews)
        {


            if (MergeViewsCallManagementViewsCallManagementViews != null)
            {
                return MergeViewsCallManagementViewsCallManagementViews(defaultViews, customViews);
            } else if (_inner != null)
            {
                return ((ICallManagementViewsProvider)_inner).MergeViews(defaultViews, customViews);
            }

            return default(CallManagementViews);
        }

        public delegate CallManagementViews RemoveDefaultViewsCallManagementViewsDelegate(CallManagementViews callManagementViews);
        public RemoveDefaultViewsCallManagementViewsDelegate RemoveDefaultViewsCallManagementViews;

        CallManagementViews ICallManagementViewsProvider.RemoveDefaultViews(CallManagementViews callManagementViews)
        {


            if (RemoveDefaultViewsCallManagementViews != null)
            {
                return RemoveDefaultViewsCallManagementViews(callManagementViews);
            } else if (_inner != null)
            {
                return ((ICallManagementViewsProvider)_inner).RemoveDefaultViews(callManagementViews);
            }

            return default(CallManagementViews);
        }

        public delegate int GetViewNameIndexStringInt32Delegate(string name, int customViewIndex);
        public GetViewNameIndexStringInt32Delegate GetViewNameIndexStringInt32;

        int ICallManagementViewsProvider.GetViewNameIndex(string name, int customViewIndex)
        {


            if (GetViewNameIndexStringInt32 != null)
            {
                return GetViewNameIndexStringInt32(name, customViewIndex);
            } else if (_inner != null)
            {
                return ((ICallManagementViewsProvider)_inner).GetViewNameIndex(name, customViewIndex);
            }

            return default(int);
        }

        public delegate string GetTranslationCallManagementColumnKeyDelegate(CallManagementColumnKey callManagementColumnKey);
        public GetTranslationCallManagementColumnKeyDelegate GetTranslationCallManagementColumnKey;

        string ICallManagementViewsProvider.GetTranslation(CallManagementColumnKey callManagementColumnKey)
        {


            if (GetTranslationCallManagementColumnKey != null)
            {
                return GetTranslationCallManagementColumnKey(callManagementColumnKey);
            } else if (_inner != null)
            {
                return ((ICallManagementViewsProvider)_inner).GetTranslation(callManagementColumnKey);
            }

            return default(string);
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