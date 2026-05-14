using System;

namespace Confirmit.CATI.Core.Timezones
{
    public interface ITimezoneConverter
    {
        DateTime ConvertToUtc(int tzId, DateTime localTime);
    }
}
