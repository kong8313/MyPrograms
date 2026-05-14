using System;

namespace DialerCommon.Logging
{
    public class UtcOffsetString
    {
        private readonly IUtcOffsetSource _utcOffsetSource;

        private TimeSpan _utcOffset;
        private string _utcOffsetAsString;

        public UtcOffsetString(IUtcOffsetSource utcOffsetSource)
        {
            _utcOffsetSource = utcOffsetSource;
        }

        public override string ToString()
        {
            UpdateUtcOffsetStringIfChanged();

            return _utcOffsetAsString;
        }

        private void UpdateUtcOffsetStringIfChanged()
        {
            var utcOffset = _utcOffsetSource.Get();

            if (utcOffset.Equals(_utcOffset))
            {
                return;
            }

            _utcOffset = utcOffset;

            _utcOffsetAsString = string.Format(
                (_utcOffset.Minutes == 0) ? "{0:+0;-0}" : "{0:+0;-0}{1:00}", utcOffset.Hours, Math.Abs(utcOffset.Minutes));
        }
    }
}