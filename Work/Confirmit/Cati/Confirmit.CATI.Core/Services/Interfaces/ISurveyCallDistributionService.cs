using System;
using System.Data;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface ISurveyCallDistributionService
    {
        DataTable GetCallsSentToDialerDistribution(int surveySid, DateTime? dateTime, int timezoneId, out int totalCount);
        DataTable GetCallsDispositionCodes(int surveySid, DateTime startTime, DateTime endTime, out int totalCount);
        DataTable GetDialerCallsBreakdown(int surveySid, out int totalCount);
        void CleanupCallsDistribution(TimeSpan expirationPeriod);
    }
}