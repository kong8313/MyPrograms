using System;

namespace DialerCommon.Logging
{
    public interface IUtcOffsetSource
    {
        TimeSpan Get();
    }
}