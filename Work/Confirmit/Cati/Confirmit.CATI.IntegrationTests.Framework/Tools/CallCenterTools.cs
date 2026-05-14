using System;
using System.Linq;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Timezones;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class CallCenterTools
    {
        public static int DefaultId
        {
            get { return ServiceLocator.Resolve<ICallCenterRepository>().Default.ID; }
        }

        public static BvCallCenterEntity Create()
        {
            var callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
            var callCenter = new BvCallCenterEntity()
            {
                Name = "CC " + Guid.NewGuid(),
                LocalTimezoneId = 1,
                Description = "CCD",
            };

            callCenterRepository.Insert(callCenter);

            return callCenter;
        }

        public static void SetCallCenterTimeZone(int id)
        {
            var callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
            var callCenter = callCenterRepository.Get(DefaultId);
            
            TimezoneManager.AddTimezone(id);
            callCenter.LocalTimezoneId = id;
            callCenterRepository.Update(callCenter);
        }

        public static void ReassignSurvey(int surveyId, params BvCallCenterEntity[] callCenters)
        {
            var service = ServiceLocator.Resolve<ICallCenterService>();
            service.ReassignSurveys(callCenters.Select(x => x.ID), new[] {surveyId});
        }
    }
}
