using System;

namespace Confirmit.CATI.Core.Services.TimeService
{
    public interface ITimeService
    {
        DateTime GetUtcNow();
    }
}
