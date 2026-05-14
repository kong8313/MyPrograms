using System;
using Confirmit.CATI.Supervisor.Classes.Script;

namespace Confirmit.CATI.Supervisor.Classes.Script.Fakes
{
    public class StubIShiftInfo : IShiftInfo 
    {
        private IShiftInfo _inner;

        public StubIShiftInfo()
        {
            _inner = null;
        }

        public IShiftInfo Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int? _Id;
        public Func<int?> IdGet;
        public Action<int?> IdSetNullableOfInt32;

        int? IShiftInfo.Id
        {
            get
            {
                if (IdGet != null)
                {
                    return IdGet();
                } else if (_inner != null)
                {
                    return ((IShiftInfo)_inner).Id;
                }

                if (IdSetNullableOfInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Id;
                }

                return default(int?);
            }

        }

        private int _ShiftTypeId;
        public Func<int> ShiftTypeIdGet;
        public Action<int> ShiftTypeIdSetInt32;

        int IShiftInfo.ShiftTypeId
        {
            get
            {
                if (ShiftTypeIdGet != null)
                {
                    return ShiftTypeIdGet();
                } else if (_inner != null)
                {
                    return ((IShiftInfo)_inner).ShiftTypeId;
                }

                if (ShiftTypeIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ShiftTypeId;
                }

                return default(int);
            }

        }

        private ShiftStatus _ShiftStatus;
        public Func<ShiftStatus> ShiftStatusGet;
        public Action<ShiftStatus> ShiftStatusSetShiftStatus;

        ShiftStatus IShiftInfo.ShiftStatus
        {
            get
            {
                if (ShiftStatusGet != null)
                {
                    return ShiftStatusGet();
                } else if (_inner != null)
                {
                    return ((IShiftInfo)_inner).ShiftStatus;
                }

                if (ShiftStatusSetShiftStatus == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ShiftStatus;
                }

                return default(ShiftStatus);
            }

        }

        private string _StartDayName;
        public Func<string> StartDayNameGet;
        public Action<string> StartDayNameSetString;

        string IShiftInfo.StartDayName
        {
            get
            {
                if (StartDayNameGet != null)
                {
                    return StartDayNameGet();
                } else if (_inner != null)
                {
                    return ((IShiftInfo)_inner).StartDayName;
                }

                if (StartDayNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _StartDayName;
                }

                return default(string);
            }

        }

        private string _EndDayName;
        public Func<string> EndDayNameGet;
        public Action<string> EndDayNameSetString;

        string IShiftInfo.EndDayName
        {
            get
            {
                if (EndDayNameGet != null)
                {
                    return EndDayNameGet();
                } else if (_inner != null)
                {
                    return ((IShiftInfo)_inner).EndDayName;
                }

                if (EndDayNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EndDayName;
                }

                return default(string);
            }

        }

        private string _StartTimeToString;
        public Func<string> StartTimeToStringGet;
        public Action<string> StartTimeToStringSetString;

        string IShiftInfo.StartTimeToString
        {
            get
            {
                if (StartTimeToStringGet != null)
                {
                    return StartTimeToStringGet();
                } else if (_inner != null)
                {
                    return ((IShiftInfo)_inner).StartTimeToString;
                }

                if (StartTimeToStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _StartTimeToString;
                }

                return default(string);
            }

        }

        private string _EndTimeToString;
        public Func<string> EndTimeToStringGet;
        public Action<string> EndTimeToStringSetString;

        string IShiftInfo.EndTimeToString
        {
            get
            {
                if (EndTimeToStringGet != null)
                {
                    return EndTimeToStringGet();
                } else if (_inner != null)
                {
                    return ((IShiftInfo)_inner).EndTimeToString;
                }

                if (EndTimeToStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EndTimeToString;
                }

                return default(string);
            }

        }

        private bool _HasRespondentTimeZone;
        public Func<bool> HasRespondentTimeZoneGet;
        public Action<bool> HasRespondentTimeZoneSetBoolean;

        bool IShiftInfo.HasRespondentTimeZone
        {
            get
            {
                if (HasRespondentTimeZoneGet != null)
                {
                    return HasRespondentTimeZoneGet();
                } else if (_inner != null)
                {
                    return ((IShiftInfo)_inner).HasRespondentTimeZone;
                }

                if (HasRespondentTimeZoneSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _HasRespondentTimeZone;
                }

                return default(bool);
            }

        }

    }
}