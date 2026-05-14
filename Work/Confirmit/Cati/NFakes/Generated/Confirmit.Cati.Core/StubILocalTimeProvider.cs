using System;
using Confirmit.CATI.Core.EmailReports;

namespace Confirmit.CATI.Core.EmailReports.Fakes
{
    public class StubILocalTimeProvider : ILocalTimeProvider 
    {
        private ILocalTimeProvider _inner;

        public StubILocalTimeProvider()
        {
            _inner = null;
        }

        public ILocalTimeProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DateTime GetCurrentLocalTimeDelegate();
        public GetCurrentLocalTimeDelegate GetCurrentLocalTime;

        DateTime ILocalTimeProvider.GetCurrentLocalTime()
        {


            if (GetCurrentLocalTime != null)
            {
                return GetCurrentLocalTime();
            } else if (_inner != null)
            {
                return ((ILocalTimeProvider)_inner).GetCurrentLocalTime();
            }

            return default(DateTime);
        }

        public delegate string GetCurrentLocalTimezoneNameDelegate();
        public GetCurrentLocalTimezoneNameDelegate GetCurrentLocalTimezoneName;

        string ILocalTimeProvider.GetCurrentLocalTimezoneName()
        {


            if (GetCurrentLocalTimezoneName != null)
            {
                return GetCurrentLocalTimezoneName();
            } else if (_inner != null)
            {
                return ((ILocalTimeProvider)_inner).GetCurrentLocalTimezoneName();
            }

            return default(string);
        }

        public delegate DateTime ConvertToLocalTimeDateTimeDelegate(DateTime utc);
        public ConvertToLocalTimeDateTimeDelegate ConvertToLocalTimeDateTime;

        DateTime ILocalTimeProvider.ConvertToLocalTime(DateTime utc)
        {


            if (ConvertToLocalTimeDateTime != null)
            {
                return ConvertToLocalTimeDateTime(utc);
            } else if (_inner != null)
            {
                return ((ILocalTimeProvider)_inner).ConvertToLocalTime(utc);
            }

            return default(DateTime);
        }

    }
}