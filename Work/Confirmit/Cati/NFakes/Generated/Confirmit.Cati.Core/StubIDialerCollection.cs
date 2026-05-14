using System;
using Confirmit.CATI.Core.Telephony;
using System.Collections.Generic;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Telephony;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerCollection : IDialerCollection 
    {
        private IDialerCollection _inner;

        public StubIDialerCollection()
        {
            _inner = null;
        }

        public IDialerCollection Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<IDialerInstance> GetDialersDelegate();
        public GetDialersDelegate GetDialers;

        IEnumerable<IDialerInstance> IDialerCollection.GetDialers()
        {


            if (GetDialers != null)
            {
                return GetDialers();
            } else if (_inner != null)
            {
                return ((IDialerCollection)_inner).GetDialers();
            }

            return default(IEnumerable<IDialerInstance>);
        }

        public delegate IEnumerable<IDialerInstance> GetDialersDialTypeDelegate(DialType dialType);
        public GetDialersDialTypeDelegate GetDialersDialType;

        IEnumerable<IDialerInstance> IDialerCollection.GetDialers(DialType dialType)
        {


            if (GetDialersDialType != null)
            {
                return GetDialersDialType(dialType);
            } else if (_inner != null)
            {
                return ((IDialerCollection)_inner).GetDialers(dialType);
            }

            return default(IEnumerable<IDialerInstance>);
        }

        public delegate IEnumerable<IDialerInstance> GetInitializedDialersDialTypeDelegate(DialType dialType);
        public GetInitializedDialersDialTypeDelegate GetInitializedDialersDialType;

        IEnumerable<IDialerInstance> IDialerCollection.GetInitializedDialers(DialType dialType)
        {


            if (GetInitializedDialersDialType != null)
            {
                return GetInitializedDialersDialType(dialType);
            } else if (_inner != null)
            {
                return ((IDialerCollection)_inner).GetInitializedDialers(dialType);
            }

            return default(IEnumerable<IDialerInstance>);
        }

        public delegate int[] GetDialerIdsDialTypeDelegate(DialType dialType);
        public GetDialerIdsDialTypeDelegate GetDialerIdsDialType;

        int[] IDialerCollection.GetDialerIds(DialType dialType)
        {


            if (GetDialerIdsDialType != null)
            {
                return GetDialerIdsDialType(dialType);
            } else if (_inner != null)
            {
                return ((IDialerCollection)_inner).GetDialerIds(dialType);
            }

            return default(int[]);
        }

        public delegate IDialerInstance GetDialerByIdInt32Delegate(int dialerId);
        public GetDialerByIdInt32Delegate GetDialerByIdInt32;

        IDialerInstance IDialerCollection.GetDialerById(int dialerId)
        {


            if (GetDialerByIdInt32 != null)
            {
                return GetDialerByIdInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerCollection)_inner).GetDialerById(dialerId);
            }

            return default(IDialerInstance);
        }

        public delegate IDialerInstance GetFirstInitializedDialerDialTypeDelegate(DialType dialType);
        public GetFirstInitializedDialerDialTypeDelegate GetFirstInitializedDialerDialType;

        IDialerInstance IDialerCollection.GetFirstInitializedDialer(DialType dialType)
        {


            if (GetFirstInitializedDialerDialType != null)
            {
                return GetFirstInitializedDialerDialType(dialType);
            } else if (_inner != null)
            {
                return ((IDialerCollection)_inner).GetFirstInitializedDialer(dialType);
            }

            return default(IDialerInstance);
        }

        public delegate bool IsDialerInitializedInt32Delegate(int dialerId);
        public IsDialerInitializedInt32Delegate IsDialerInitializedInt32;

        bool IDialerCollection.IsDialerInitialized(int dialerId)
        {


            if (IsDialerInitializedInt32 != null)
            {
                return IsDialerInitializedInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerCollection)_inner).IsDialerInitialized(dialerId);
            }

            return default(bool);
        }

        public delegate void InitializeCollectionDelegate();
        public InitializeCollectionDelegate InitializeCollection;

        void IDialerCollection.InitializeCollection()
        {

            if (InitializeCollection != null)
            {
                InitializeCollection();
            } else if (_inner != null)
            {
                ((IDialerCollection)_inner).InitializeCollection();
            }
        }

        public delegate bool InitializedDialerExistsDelegate();
        public InitializedDialerExistsDelegate InitializedDialerExists;

        bool IDialerCollection.InitializedDialerExists()
        {


            if (InitializedDialerExists != null)
            {
                return InitializedDialerExists();
            } else if (_inner != null)
            {
                return ((IDialerCollection)_inner).InitializedDialerExists();
            }

            return default(bool);
        }

        public delegate bool InitializedDialerExistsDialTypeDelegate(DialType dialType);
        public InitializedDialerExistsDialTypeDelegate InitializedDialerExistsDialType;

        bool IDialerCollection.InitializedDialerExists(DialType dialType)
        {


            if (InitializedDialerExistsDialType != null)
            {
                return InitializedDialerExistsDialType(dialType);
            } else if (_inner != null)
            {
                return ((IDialerCollection)_inner).InitializedDialerExists(dialType);
            }

            return default(bool);
        }

        private IDialerAPI _FirstLoadedDialerApi;
        public Func<IDialerAPI> FirstLoadedDialerApiGet;
        public Action<IDialerAPI> FirstLoadedDialerApiSetIDialerAPI;

        IDialerAPI IDialerCollection.FirstLoadedDialerApi
        {
            get
            {
                if (FirstLoadedDialerApiGet != null)
                {
                    return FirstLoadedDialerApiGet();
                } else if (_inner != null)
                {
                    return ((IDialerCollection)_inner).FirstLoadedDialerApi;
                }

                if (FirstLoadedDialerApiSetIDialerAPI == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _FirstLoadedDialerApi;
                }

                return default(IDialerAPI);
            }

        }

    }
}