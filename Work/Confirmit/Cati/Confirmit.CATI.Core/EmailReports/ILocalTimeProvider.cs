using System;

namespace Confirmit.CATI.Core.EmailReports
{
    public interface ILocalTimeProvider
    {
        DateTime GetCurrentLocalTime();
        string GetCurrentLocalTimezoneName();
        DateTime ConvertToLocalTime(DateTime utc);
    }
}