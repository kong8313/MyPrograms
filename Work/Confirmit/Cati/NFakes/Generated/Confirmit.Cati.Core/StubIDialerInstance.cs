using System;
using ConfirmitDialerInterface;
using BvCallHandlerLibrary;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Common;
using DialerCommon;

namespace BvCallHandlerLibrary.Fakes
{
    public class StubIDialerInstance : IDialerInstance 
    {
        private IDialerInstance _inner;

        public StubIDialerInstance()
        {
            _inner = null;
        }

        public IDialerInstance Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnDialerStateDialerStateDelegate(DialerState dialerState);
        public OnDialerStateDialerStateDelegate OnDialerStateDialerState;

        void IDialerInstance.OnDialerState(DialerState dialerState)
        {

            if (OnDialerStateDialerState != null)
            {
                OnDialerStateDialerState(dialerState);
            } else if (_inner != null)
            {
                ((IDialerInstance)_inner).OnDialerState(dialerState);
            }
        }

        public delegate void UninitializeBooleanBooleanDelegate(bool releaseDialerWs, bool withReconnection);
        public UninitializeBooleanBooleanDelegate UninitializeBooleanBoolean;

        void IDialerInstance.Uninitialize(bool releaseDialerWs, bool withReconnection)
        {

            if (UninitializeBooleanBoolean != null)
            {
                UninitializeBooleanBoolean(releaseDialerWs, withReconnection);
            } else if (_inner != null)
            {
                ((IDialerInstance)_inner).Uninitialize(releaseDialerWs, withReconnection);
            }
        }

        public delegate void CreateDelegate();
        public CreateDelegate Create;

        void IDialerInstance.Create()
        {

            if (Create != null)
            {
                Create();
            } else if (_inner != null)
            {
                ((IDialerInstance)_inner).Create();
            }
        }

        public delegate void InitializeDelegate();
        public InitializeDelegate Initialize;

        void IDialerInstance.Initialize()
        {

            if (Initialize != null)
            {
                Initialize();
            } else if (_inner != null)
            {
                ((IDialerInstance)_inner).Initialize();
            }
        }

        private IDialerAPI _Api;
        public Func<IDialerAPI> ApiGet;
        public Action<IDialerAPI> ApiSetIDialerAPI;

        IDialerAPI IDialerInstance.Api
        {
            get
            {
                if (ApiGet != null)
                {
                    return ApiGet();
                } else if (_inner != null)
                {
                    return ((IDialerInstance)_inner).Api;
                }

                if (ApiSetIDialerAPI == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Api;
                }

                return default(IDialerAPI);
            }

        }

        private int _DialerId;
        public Func<int> DialerIdGet;
        public Action<int> DialerIdSetInt32;

        int IDialerInstance.DialerId
        {
            get
            {
                if (DialerIdGet != null)
                {
                    return DialerIdGet();
                } else if (_inner != null)
                {
                    return ((IDialerInstance)_inner).DialerId;
                }

                if (DialerIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DialerId;
                }

                return default(int);
            }

            set
            {
                if (DialerIdSetInt32 != null)
                {
                    DialerIdSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerInstance)_inner).DialerId = value;
                    return;
                }

                if (DialerIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DialerId = value;
                }

            }
        }

        private string _DialerName;
        public Func<string> DialerNameGet;
        public Action<string> DialerNameSetString;

        string IDialerInstance.DialerName
        {
            get
            {
                if (DialerNameGet != null)
                {
                    return DialerNameGet();
                } else if (_inner != null)
                {
                    return ((IDialerInstance)_inner).DialerName;
                }

                if (DialerNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DialerName;
                }

                return default(string);
            }

            set
            {
                if (DialerNameSetString != null)
                {
                    DialerNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerInstance)_inner).DialerName = value;
                    return;
                }

                if (DialerNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DialerName = value;
                }

            }
        }

        private DialType _DialType;
        public Func<DialType> DialTypeGet;
        public Action<DialType> DialTypeSetDialType;

        DialType IDialerInstance.DialType
        {
            get
            {
                if (DialTypeGet != null)
                {
                    return DialTypeGet();
                } else if (_inner != null)
                {
                    return ((IDialerInstance)_inner).DialType;
                }

                if (DialTypeSetDialType == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DialType;
                }

                return default(DialType);
            }

            set
            {
                if (DialTypeSetDialType != null)
                {
                    DialTypeSetDialType(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerInstance)_inner).DialType = value;
                    return;
                }

                if (DialTypeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DialType = value;
                }

            }
        }

        private bool _IsDialerInitialized;
        public Func<bool> IsDialerInitializedGet;
        public Action<bool> IsDialerInitializedSetBoolean;

        bool IDialerInstance.IsDialerInitialized
        {
            get
            {
                if (IsDialerInitializedGet != null)
                {
                    return IsDialerInitializedGet();
                } else if (_inner != null)
                {
                    return ((IDialerInstance)_inner).IsDialerInitialized;
                }

                if (IsDialerInitializedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsDialerInitialized;
                }

                return default(bool);
            }

            set
            {
                if (IsDialerInitializedSetBoolean != null)
                {
                    IsDialerInitializedSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerInstance)_inner).IsDialerInitialized = value;
                    return;
                }

                if (IsDialerInitializedGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IsDialerInitialized = value;
                }

            }
        }

        private bool _DialerOperationalState;
        public Func<bool> DialerOperationalStateGet;
        public Action<bool> DialerOperationalStateSetBoolean;

        bool IDialerInstance.DialerOperationalState
        {
            get
            {
                if (DialerOperationalStateGet != null)
                {
                    return DialerOperationalStateGet();
                } else if (_inner != null)
                {
                    return ((IDialerInstance)_inner).DialerOperationalState;
                }

                if (DialerOperationalStateSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DialerOperationalState;
                }

                return default(bool);
            }

            set
            {
                if (DialerOperationalStateSetBoolean != null)
                {
                    DialerOperationalStateSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerInstance)_inner).DialerOperationalState = value;
                    return;
                }

                if (DialerOperationalStateGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DialerOperationalState = value;
                }

            }
        }

        private string _TenantId;
        public Func<string> TenantIdGet;
        public Action<string> TenantIdSetString;

        string IDialerInstance.TenantId
        {
            get
            {
                if (TenantIdGet != null)
                {
                    return TenantIdGet();
                } else if (_inner != null)
                {
                    return ((IDialerInstance)_inner).TenantId;
                }

                if (TenantIdSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TenantId;
                }

                return default(string);
            }

            set
            {
                if (TenantIdSetString != null)
                {
                    TenantIdSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerInstance)_inner).TenantId = value;
                    return;
                }

                if (TenantIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _TenantId = value;
                }

            }
        }

        private int _TenantIdInt;
        public Func<int> TenantIdIntGet;
        public Action<int> TenantIdIntSetInt32;

        int IDialerInstance.TenantIdInt
        {
            get
            {
                if (TenantIdIntGet != null)
                {
                    return TenantIdIntGet();
                } else if (_inner != null)
                {
                    return ((IDialerInstance)_inner).TenantIdInt;
                }

                if (TenantIdIntSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TenantIdInt;
                }

                return default(int);
            }

            set
            {
                if (TenantIdIntSetInt32 != null)
                {
                    TenantIdIntSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerInstance)_inner).TenantIdInt = value;
                    return;
                }

                if (TenantIdIntGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _TenantIdInt = value;
                }

            }
        }

        private DialerFeatures _SupportedFeatures;
        public Func<DialerFeatures> SupportedFeaturesGet;
        public Action<DialerFeatures> SupportedFeaturesSetDialerFeatures;

        DialerFeatures IDialerInstance.SupportedFeatures
        {
            get
            {
                if (SupportedFeaturesGet != null)
                {
                    return SupportedFeaturesGet();
                } else if (_inner != null)
                {
                    return ((IDialerInstance)_inner).SupportedFeatures;
                }

                if (SupportedFeaturesSetDialerFeatures == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SupportedFeatures;
                }

                return default(DialerFeatures);
            }

            set
            {
                if (SupportedFeaturesSetDialerFeatures != null)
                {
                    SupportedFeaturesSetDialerFeatures(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerInstance)_inner).SupportedFeatures = value;
                    return;
                }

                if (SupportedFeaturesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SupportedFeatures = value;
                }

            }
        }

        private string _Version;
        public Func<string> VersionGet;
        public Action<string> VersionSetString;

        string IDialerInstance.Version
        {
            get
            {
                if (VersionGet != null)
                {
                    return VersionGet();
                } else if (_inner != null)
                {
                    return ((IDialerInstance)_inner).Version;
                }

                if (VersionSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Version;
                }

                return default(string);
            }

        }

    }
}