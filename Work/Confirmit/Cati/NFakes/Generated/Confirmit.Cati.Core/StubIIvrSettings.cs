using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIIvrSettings : IIvrSettings 
    {
        private IIvrSettings _inner;

        public StubIIvrSettings()
        {
            _inner = null;
        }

        public IIvrSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private bool _Beep;
        public Func<bool> BeepGet;
        public Action<bool> BeepSetBoolean;

        bool IIvrSettings.Beep
        {
            get
            {
                if (BeepGet != null)
                {
                    return BeepGet();
                } else if (_inner != null)
                {
                    return ((IIvrSettings)_inner).Beep;
                }

                if (BeepSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Beep;
                }

                return default(bool);
            }

            set
            {
                if (BeepSetBoolean != null)
                {
                    BeepSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IIvrSettings)_inner).Beep = value;
                    return;
                }

                if (BeepGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Beep = value;
                }

            }
        }

        private bool _DtmfTerm;
        public Func<bool> DtmfTermGet;
        public Action<bool> DtmfTermSetBoolean;

        bool IIvrSettings.DtmfTerm
        {
            get
            {
                if (DtmfTermGet != null)
                {
                    return DtmfTermGet();
                } else if (_inner != null)
                {
                    return ((IIvrSettings)_inner).DtmfTerm;
                }

                if (DtmfTermSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DtmfTerm;
                }

                return default(bool);
            }

            set
            {
                if (DtmfTermSetBoolean != null)
                {
                    DtmfTermSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IIvrSettings)_inner).DtmfTerm = value;
                    return;
                }

                if (DtmfTermGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DtmfTerm = value;
                }

            }
        }

        private int _FinalSilence;
        public Func<int> FinalSilenceGet;
        public Action<int> FinalSilenceSetInt32;

        int IIvrSettings.FinalSilence
        {
            get
            {
                if (FinalSilenceGet != null)
                {
                    return FinalSilenceGet();
                } else if (_inner != null)
                {
                    return ((IIvrSettings)_inner).FinalSilence;
                }

                if (FinalSilenceSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _FinalSilence;
                }

                return default(int);
            }

            set
            {
                if (FinalSilenceSetInt32 != null)
                {
                    FinalSilenceSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IIvrSettings)_inner).FinalSilence = value;
                    return;
                }

                if (FinalSilenceGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _FinalSilence = value;
                }

            }
        }

        private int _MaxTime;
        public Func<int> MaxTimeGet;
        public Action<int> MaxTimeSetInt32;

        int IIvrSettings.MaxTime
        {
            get
            {
                if (MaxTimeGet != null)
                {
                    return MaxTimeGet();
                } else if (_inner != null)
                {
                    return ((IIvrSettings)_inner).MaxTime;
                }

                if (MaxTimeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaxTime;
                }

                return default(int);
            }

            set
            {
                if (MaxTimeSetInt32 != null)
                {
                    MaxTimeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IIvrSettings)_inner).MaxTime = value;
                    return;
                }

                if (MaxTimeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MaxTime = value;
                }

            }
        }

        private string _RecordType;
        public Func<string> RecordTypeGet;
        public Action<string> RecordTypeSetString;

        string IIvrSettings.RecordType
        {
            get
            {
                if (RecordTypeGet != null)
                {
                    return RecordTypeGet();
                } else if (_inner != null)
                {
                    return ((IIvrSettings)_inner).RecordType;
                }

                if (RecordTypeSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RecordType;
                }

                return default(string);
            }

            set
            {
                if (RecordTypeSetString != null)
                {
                    RecordTypeSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IIvrSettings)_inner).RecordType = value;
                    return;
                }

                if (RecordTypeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _RecordType = value;
                }

            }
        }

        private string _TermChar;
        public Func<string> TermCharGet;
        public Action<string> TermCharSetString;

        string IIvrSettings.TermChar
        {
            get
            {
                if (TermCharGet != null)
                {
                    return TermCharGet();
                } else if (_inner != null)
                {
                    return ((IIvrSettings)_inner).TermChar;
                }

                if (TermCharSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TermChar;
                }

                return default(string);
            }

            set
            {
                if (TermCharSetString != null)
                {
                    TermCharSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IIvrSettings)_inner).TermChar = value;
                    return;
                }

                if (TermCharGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _TermChar = value;
                }

            }
        }

        private TimeSpan _TransferTimeout;
        public Func<TimeSpan> TransferTimeoutGet;
        public Action<TimeSpan> TransferTimeoutSetTimeSpan;

        TimeSpan IIvrSettings.TransferTimeout
        {
            get
            {
                if (TransferTimeoutGet != null)
                {
                    return TransferTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IIvrSettings)_inner).TransferTimeout;
                }

                if (TransferTimeoutSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TransferTimeout;
                }

                return default(TimeSpan);
            }

            set
            {
                if (TransferTimeoutSetTimeSpan != null)
                {
                    TransferTimeoutSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IIvrSettings)_inner).TransferTimeout = value;
                    return;
                }

                if (TransferTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _TransferTimeout = value;
                }

            }
        }

    }
}