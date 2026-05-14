using System;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIShiftService : IShiftService 
    {
        private IShiftService _inner;

        public StubIShiftService()
        {
            _inner = null;
        }

        public IShiftService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CheckConfigurationDelegate();
        public CheckConfigurationDelegate CheckConfiguration;

        void IShiftService.CheckConfiguration()
        {

            if (CheckConfiguration != null)
            {
                CheckConfiguration();
            } else if (_inner != null)
            {
                ((IShiftService)_inner).CheckConfiguration();
            }
        }

        public delegate int GetShiftTypeWorkIDInt32Delegate(int shiftTypeID);
        public GetShiftTypeWorkIDInt32Delegate GetShiftTypeWorkIDInt32;

        int IShiftService.GetShiftTypeWorkID(int shiftTypeID)
        {


            if (GetShiftTypeWorkIDInt32 != null)
            {
                return GetShiftTypeWorkIDInt32(shiftTypeID);
            } else if (_inner != null)
            {
                return ((IShiftService)_inner).GetShiftTypeWorkID(shiftTypeID);
            }

            return default(int);
        }

        public delegate ShiftService.MatchingShift GetMatchingShiftDateTimeInt32Delegate(DateTime utcTime, int tzID);
        public GetMatchingShiftDateTimeInt32Delegate GetMatchingShiftDateTimeInt32;

        ShiftService.MatchingShift IShiftService.GetMatchingShift(DateTime utcTime, int tzID)
        {


            if (GetMatchingShiftDateTimeInt32 != null)
            {
                return GetMatchingShiftDateTimeInt32(utcTime, tzID);
            } else if (_inner != null)
            {
                return ((IShiftService)_inner).GetMatchingShift(utcTime, tzID);
            }

            return default(ShiftService.MatchingShift);
        }

        public delegate ShiftService.MatchingShift GetNextShiftMatchingShiftInt32Int32OutDelegate(ShiftService.MatchingShift currentShift, int tzID, out int countSkipShifts);
        public GetNextShiftMatchingShiftInt32Int32OutDelegate GetNextShiftMatchingShiftInt32Int32Out;

        ShiftService.MatchingShift IShiftService.GetNextShift(ShiftService.MatchingShift currentShift, int tzID, out int countSkipShifts)
        {
            countSkipShifts = default(int);


            if (GetNextShiftMatchingShiftInt32Int32Out != null)
            {
                return GetNextShiftMatchingShiftInt32Int32Out(currentShift, tzID, out countSkipShifts);
            } else if (_inner != null)
            {
                return ((IShiftService)_inner).GetNextShift(currentShift, tzID, out countSkipShifts);
            }

            return default(ShiftService.MatchingShift);
        }

        public delegate ShiftService.MatchingShift GetShiftAfterNumberOfShiftsMatchingShiftInt32Int32BooleanDelegate(ShiftService.MatchingShift curentShift, int tzID, int numberOfShifts, bool isTakingExclusionIntoAccount);
        public GetShiftAfterNumberOfShiftsMatchingShiftInt32Int32BooleanDelegate GetShiftAfterNumberOfShiftsMatchingShiftInt32Int32Boolean;

        ShiftService.MatchingShift IShiftService.GetShiftAfterNumberOfShifts(ShiftService.MatchingShift curentShift, int tzID, int numberOfShifts, bool isTakingExclusionIntoAccount)
        {


            if (GetShiftAfterNumberOfShiftsMatchingShiftInt32Int32Boolean != null)
            {
                return GetShiftAfterNumberOfShiftsMatchingShiftInt32Int32Boolean(curentShift, tzID, numberOfShifts, isTakingExclusionIntoAccount);
            } else if (_inner != null)
            {
                return ((IShiftService)_inner).GetShiftAfterNumberOfShifts(curentShift, tzID, numberOfShifts, isTakingExclusionIntoAccount);
            }

            return default(ShiftService.MatchingShift);
        }

        public delegate DateTime GetMatchingTimeDateTimeInt32Delegate(DateTime utcNowTime, int tzID);
        public GetMatchingTimeDateTimeInt32Delegate GetMatchingTimeDateTimeInt32;

        DateTime IShiftService.GetMatchingTime(DateTime utcNowTime, int tzID)
        {


            if (GetMatchingTimeDateTimeInt32 != null)
            {
                return GetMatchingTimeDateTimeInt32(utcNowTime, tzID);
            } else if (_inner != null)
            {
                return ((IShiftService)_inner).GetMatchingTime(utcNowTime, tzID);
            }

            return default(DateTime);
        }

        public delegate ShiftService.MatchingShift GetExactShiftDateTimeInt32Delegate(DateTime utcNowTime, int tzID);
        public GetExactShiftDateTimeInt32Delegate GetExactShiftDateTimeInt32;

        ShiftService.MatchingShift IShiftService.GetExactShift(DateTime utcNowTime, int tzID)
        {


            if (GetExactShiftDateTimeInt32 != null)
            {
                return GetExactShiftDateTimeInt32(utcNowTime, tzID);
            } else if (_inner != null)
            {
                return ((IShiftService)_inner).GetExactShift(utcNowTime, tzID);
            }

            return default(ShiftService.MatchingShift);
        }

        public delegate ShiftService.MatchingShift GetNextShiftMatchingShiftInt32Delegate(ShiftService.MatchingShift currentShift, int tzID);
        public GetNextShiftMatchingShiftInt32Delegate GetNextShiftMatchingShiftInt32;

        ShiftService.MatchingShift IShiftService.GetNextShift(ShiftService.MatchingShift currentShift, int tzID)
        {


            if (GetNextShiftMatchingShiftInt32 != null)
            {
                return GetNextShiftMatchingShiftInt32(currentShift, tzID);
            } else if (_inner != null)
            {
                return ((IShiftService)_inner).GetNextShift(currentShift, tzID);
            }

            return default(ShiftService.MatchingShift);
        }

        public delegate ShiftService.MatchingShift GetShiftAfterNumberOfMinutesDateTimeInt32Int32Delegate(DateTime utcNowTime, int tzID, int countMinutes);
        public GetShiftAfterNumberOfMinutesDateTimeInt32Int32Delegate GetShiftAfterNumberOfMinutesDateTimeInt32Int32;

        ShiftService.MatchingShift IShiftService.GetShiftAfterNumberOfMinutes(DateTime utcNowTime, int tzID, int countMinutes)
        {


            if (GetShiftAfterNumberOfMinutesDateTimeInt32Int32 != null)
            {
                return GetShiftAfterNumberOfMinutesDateTimeInt32Int32(utcNowTime, tzID, countMinutes);
            } else if (_inner != null)
            {
                return ((IShiftService)_inner).GetShiftAfterNumberOfMinutes(utcNowTime, tzID, countMinutes);
            }

            return default(ShiftService.MatchingShift);
        }

        public delegate ShiftService.MatchingShift GetShiftAfterNumberOfShiftsDateTimeInt32Int32Delegate(DateTime utcNowTime, int tzID, int numberOfShifts);
        public GetShiftAfterNumberOfShiftsDateTimeInt32Int32Delegate GetShiftAfterNumberOfShiftsDateTimeInt32Int32;

        ShiftService.MatchingShift IShiftService.GetShiftAfterNumberOfShifts(DateTime utcNowTime, int tzID, int numberOfShifts)
        {


            if (GetShiftAfterNumberOfShiftsDateTimeInt32Int32 != null)
            {
                return GetShiftAfterNumberOfShiftsDateTimeInt32Int32(utcNowTime, tzID, numberOfShifts);
            } else if (_inner != null)
            {
                return ((IShiftService)_inner).GetShiftAfterNumberOfShifts(utcNowTime, tzID, numberOfShifts);
            }

            return default(ShiftService.MatchingShift);
        }

        public delegate ShiftService.MatchingShift GetNextShiftOfSpecifiedTypeDateTimeInt32Int32Delegate(DateTime utcTime, int tzID, int scriptShiftTypeID);
        public GetNextShiftOfSpecifiedTypeDateTimeInt32Int32Delegate GetNextShiftOfSpecifiedTypeDateTimeInt32Int32;

        ShiftService.MatchingShift IShiftService.GetNextShiftOfSpecifiedType(DateTime utcTime, int tzID, int scriptShiftTypeID)
        {


            if (GetNextShiftOfSpecifiedTypeDateTimeInt32Int32 != null)
            {
                return GetNextShiftOfSpecifiedTypeDateTimeInt32Int32(utcTime, tzID, scriptShiftTypeID);
            } else if (_inner != null)
            {
                return ((IShiftService)_inner).GetNextShiftOfSpecifiedType(utcTime, tzID, scriptShiftTypeID);
            }

            return default(ShiftService.MatchingShift);
        }

        public delegate ShiftService.MatchingShift GetNextShiftByIDDateTimeInt32Int32Delegate(DateTime utcTime, int tzID, int scriptShiftID);
        public GetNextShiftByIDDateTimeInt32Int32Delegate GetNextShiftByIDDateTimeInt32Int32;

        ShiftService.MatchingShift IShiftService.GetNextShiftByID(DateTime utcTime, int tzID, int scriptShiftID)
        {


            if (GetNextShiftByIDDateTimeInt32Int32 != null)
            {
                return GetNextShiftByIDDateTimeInt32Int32(utcTime, tzID, scriptShiftID);
            } else if (_inner != null)
            {
                return ((IShiftService)_inner).GetNextShiftByID(utcTime, tzID, scriptShiftID);
            }

            return default(ShiftService.MatchingShift);
        }

        private int _ScheduleID;
        public Func<int> ScheduleIDGet;
        public Action<int> ScheduleIDSetInt32;

        int IShiftService.ScheduleID
        {
            get
            {
                if (ScheduleIDGet != null)
                {
                    return ScheduleIDGet();
                } else if (_inner != null)
                {
                    return ((IShiftService)_inner).ScheduleID;
                }

                if (ScheduleIDSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ScheduleID;
                }

                return default(int);
            }

        }

        private bool _IsTakingExclusionIntoAccount;
        public Func<bool> IsTakingExclusionIntoAccountGet;
        public Action<bool> IsTakingExclusionIntoAccountSetBoolean;

        bool IShiftService.IsTakingExclusionIntoAccount
        {
            get
            {
                if (IsTakingExclusionIntoAccountGet != null)
                {
                    return IsTakingExclusionIntoAccountGet();
                } else if (_inner != null)
                {
                    return ((IShiftService)_inner).IsTakingExclusionIntoAccount;
                }

                if (IsTakingExclusionIntoAccountSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsTakingExclusionIntoAccount;
                }

                return default(bool);
            }

        }

    }
}