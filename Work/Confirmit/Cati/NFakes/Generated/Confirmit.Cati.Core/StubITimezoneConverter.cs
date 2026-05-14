using System;
using Confirmit.CATI.Core.Timezones;

namespace Confirmit.CATI.Core.Timezones.Fakes
{
    public class StubITimezoneConverter : ITimezoneConverter 
    {
        private ITimezoneConverter _inner;

        public StubITimezoneConverter()
        {
            _inner = null;
        }

        public ITimezoneConverter Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DateTime ConvertToUtcInt32DateTimeDelegate(int tzId, DateTime localTime);
        public ConvertToUtcInt32DateTimeDelegate ConvertToUtcInt32DateTime;

        DateTime ITimezoneConverter.ConvertToUtc(int tzId, DateTime localTime)
        {


            if (ConvertToUtcInt32DateTime != null)
            {
                return ConvertToUtcInt32DateTime(tzId, localTime);
            } else if (_inner != null)
            {
                return ((ITimezoneConverter)_inner).ConvertToUtc(tzId, localTime);
            }

            return default(DateTime);
        }

    }
}