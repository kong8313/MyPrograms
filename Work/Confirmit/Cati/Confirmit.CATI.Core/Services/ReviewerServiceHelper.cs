using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using System;
using System.Globalization;

namespace Confirmit.CATI.Core.Services
{
    public static class ReviewerServiceHelper
    {
        public static string GetDefaultSessionName(string supervisorName, string surveyId)
        {
            var timeZoneId = ServiceLocator.Resolve<ICallCenterService>()
                .GetSupervisorCallCenter(supervisorName)
                .LocalTimezoneId;

            var service = ServiceLocator.Resolve<ITimezoneService>();
            var dateTimeNow = service.ConvertTimeFromUtc(timeZoneId, DateTime.UtcNow);

            return string.Format(
                "{0}_{1}_{2}",
                supervisorName,
                surveyId,
                dateTimeNow.ToString(DateTimeFormatInfo.InvariantInfo)
                );
        }
    }
}
