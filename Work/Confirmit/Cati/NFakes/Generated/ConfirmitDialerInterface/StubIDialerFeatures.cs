using System;
using ConfirmitDialerInterface;

namespace ConfirmitDialerInterface.Fakes
{
    public class StubIDialerFeatures : IDialerFeatures 
    {
        private IDialerFeatures _inner;

        public StubIDialerFeatures()
        {
            _inner = null;
        }

        public IDialerFeatures Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private bool _IsIVRSupported;
        public Func<bool> IsIVRSupportedGet;
        public Action<bool> IsIVRSupportedSetBoolean;

        bool IDialerFeatures.IsIVRSupported
        {
            get
            {
                if (IsIVRSupportedGet != null)
                {
                    return IsIVRSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerFeatures)_inner).IsIVRSupported;
                }

                if (IsIVRSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsIVRSupported;
                }

                return default(bool);
            }

        }

        private bool _IsInboundSupported;
        public Func<bool> IsInboundSupportedGet;
        public Action<bool> IsInboundSupportedSetBoolean;

        bool IDialerFeatures.IsInboundSupported
        {
            get
            {
                if (IsInboundSupportedGet != null)
                {
                    return IsInboundSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerFeatures)_inner).IsInboundSupported;
                }

                if (IsInboundSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsInboundSupported;
                }

                return default(bool);
            }

        }

        private bool _IsExternalTransferSupported;
        public Func<bool> IsExternalTransferSupportedGet;
        public Action<bool> IsExternalTransferSupportedSetBoolean;

        bool IDialerFeatures.IsExternalTransferSupported
        {
            get
            {
                if (IsExternalTransferSupportedGet != null)
                {
                    return IsExternalTransferSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerFeatures)_inner).IsExternalTransferSupported;
                }

                if (IsExternalTransferSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsExternalTransferSupported;
                }

                return default(bool);
            }

        }

        private bool _IsInternalTransferSupported;
        public Func<bool> IsInternalTransferSupportedGet;
        public Action<bool> IsInternalTransferSupportedSetBoolean;

        bool IDialerFeatures.IsInternalTransferSupported
        {
            get
            {
                if (IsInternalTransferSupportedGet != null)
                {
                    return IsInternalTransferSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerFeatures)_inner).IsInternalTransferSupported;
                }

                if (IsInternalTransferSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsInternalTransferSupported;
                }

                return default(bool);
            }

        }

        private bool _IsCoachingSupported;
        public Func<bool> IsCoachingSupportedGet;
        public Action<bool> IsCoachingSupportedSetBoolean;

        bool IDialerFeatures.IsCoachingSupported
        {
            get
            {
                if (IsCoachingSupportedGet != null)
                {
                    return IsCoachingSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerFeatures)_inner).IsCoachingSupported;
                }

                if (IsCoachingSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsCoachingSupported;
                }

                return default(bool);
            }

        }

        private bool _IsBargingSupported;
        public Func<bool> IsBargingSupportedGet;
        public Action<bool> IsBargingSupportedSetBoolean;

        bool IDialerFeatures.IsBargingSupported
        {
            get
            {
                if (IsBargingSupportedGet != null)
                {
                    return IsBargingSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerFeatures)_inner).IsBargingSupported;
                }

                if (IsBargingSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsBargingSupported;
                }

                return default(bool);
            }

        }

        private bool _IsMonitoringMuteSupported;
        public Func<bool> IsMonitoringMuteSupportedGet;
        public Action<bool> IsMonitoringMuteSupportedSetBoolean;

        bool IDialerFeatures.IsMonitoringMuteSupported
        {
            get
            {
                if (IsMonitoringMuteSupportedGet != null)
                {
                    return IsMonitoringMuteSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerFeatures)_inner).IsMonitoringMuteSupported;
                }

                if (IsMonitoringMuteSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsMonitoringMuteSupported;
                }

                return default(bool);
            }

        }

        private bool _IsSoftphoneSingleSignOnSupported;
        public Func<bool> IsSoftphoneSingleSignOnSupportedGet;
        public Action<bool> IsSoftphoneSingleSignOnSupportedSetBoolean;

        bool IDialerFeatures.IsSoftphoneSingleSignOnSupported
        {
            get
            {
                if (IsSoftphoneSingleSignOnSupportedGet != null)
                {
                    return IsSoftphoneSingleSignOnSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerFeatures)_inner).IsSoftphoneSingleSignOnSupported;
                }

                if (IsSoftphoneSingleSignOnSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsSoftphoneSingleSignOnSupported;
                }

                return default(bool);
            }

        }

        private bool _IsAudioContentDownloadSupported;
        public Func<bool> IsAudioContentDownloadSupportedGet;
        public Action<bool> IsAudioContentDownloadSupportedSetBoolean;

        bool IDialerFeatures.IsAudioContentDownloadSupported
        {
            get
            {
                if (IsAudioContentDownloadSupportedGet != null)
                {
                    return IsAudioContentDownloadSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerFeatures)_inner).IsAudioContentDownloadSupported;
                }

                if (IsAudioContentDownloadSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsAudioContentDownloadSupported;
                }

                return default(bool);
            }

        }

        private bool _CustomIvrPipeline;
        public Func<bool> CustomIvrPipelineGet;
        public Action<bool> CustomIvrPipelineSetBoolean;

        bool IDialerFeatures.CustomIvrPipeline
        {
            get
            {
                if (CustomIvrPipelineGet != null)
                {
                    return CustomIvrPipelineGet();
                } else if (_inner != null)
                {
                    return ((IDialerFeatures)_inner).CustomIvrPipeline;
                }

                if (CustomIvrPipelineSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CustomIvrPipeline;
                }

                return default(bool);
            }

        }

    }
}