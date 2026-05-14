using System;
using Confirmit.CATI.Core.AsynchronousTrigger;
using Confirmit.CATI.Core.AsynchronousTrigger.Messages;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Fakes
{
    public class StubIAsynchronousTrigger : IAsynchronousTrigger 
    {
        private IAsynchronousTrigger _inner;

        public StubIAsynchronousTrigger()
        {
            _inner = null;
        }

        public IAsynchronousTrigger Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeDelegate();
        public InitializeDelegate Initialize;

        void IAsynchronousTrigger.Initialize()
        {

            if (Initialize != null)
            {
                Initialize();
            } else if (_inner != null)
            {
                ((IAsynchronousTrigger)_inner).Initialize();
            }
        }

        public delegate void UninitializeDelegate();
        public UninitializeDelegate Uninitialize;

        void IAsynchronousTrigger.Uninitialize()
        {

            if (Uninitialize != null)
            {
                Uninitialize();
            } else if (_inner != null)
            {
                ((IAsynchronousTrigger)_inner).Uninitialize();
            }
        }

        public delegate void OnTableChangedTriggerMessageDelegate(TriggerMessage triggerMessage);
        public OnTableChangedTriggerMessageDelegate OnTableChangedTriggerMessage;

        void IAsynchronousTrigger.OnTableChanged(TriggerMessage triggerMessage)
        {

            if (OnTableChangedTriggerMessage != null)
            {
                OnTableChangedTriggerMessage(triggerMessage);
            } else if (_inner != null)
            {
                ((IAsynchronousTrigger)_inner).OnTableChanged(triggerMessage);
            }
        }

        private string _TrigerName;
        public Func<string> TrigerNameGet;
        public Action<string> TrigerNameSetString;

        string IAsynchronousTrigger.TrigerName
        {
            get
            {
                if (TrigerNameGet != null)
                {
                    return TrigerNameGet();
                } else if (_inner != null)
                {
                    return ((IAsynchronousTrigger)_inner).TrigerName;
                }

                if (TrigerNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TrigerName;
                }

                return default(string);
            }

        }

        private string _TableName;
        public Func<string> TableNameGet;
        public Action<string> TableNameSetString;

        string IAsynchronousTrigger.TableName
        {
            get
            {
                if (TableNameGet != null)
                {
                    return TableNameGet();
                } else if (_inner != null)
                {
                    return ((IAsynchronousTrigger)_inner).TableName;
                }

                if (TableNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TableName;
                }

                return default(string);
            }

        }

    }
}