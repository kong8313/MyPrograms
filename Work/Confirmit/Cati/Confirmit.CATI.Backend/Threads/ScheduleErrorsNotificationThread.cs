using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;
using Confirmit.CATI.Core.Threading;
using System;
using System.Linq;

namespace Confirmit.CATI.Backend.Threads
{
    public class ScheduleErrorsNotificationThread : DatabasePeriodicalThread
    {
        private readonly ISchedulingScriptNotificator _schedulingScriptNotificator;
        private readonly IScheduleErrorRepository _scheduleErrorRepository;

        public ScheduleErrorsNotificationThread(
            ISchedulingScriptNotificator schedulingScriptNotificator,
             IScheduleErrorRepository scheduleErrorRepository)
           : base("SchedulingScriptErrorsNotificationThread")
        {
            _schedulingScriptNotificator = schedulingScriptNotificator;
            _scheduleErrorRepository = scheduleErrorRepository;
        }

        public override TimeSpan StopTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(30);
            }
        }

        public override TimeSpan SleepTimeout
        {
            get
            {
                return TimeSpan.FromMinutes(5);
            }
        }

        protected override string ResourceName => DatabaseLockTimeoutsAndRecourceNames.ScheduleErrorsNotificationResourceName;

        protected override string Owner => "SendNotificationEmails";

        protected override void DoDatabaseWork()
        {
            DoWork();
        }

        public void DoWork()
        {
            var notSentErrors = _scheduleErrorRepository.GetNotSentErrors();

            var groupedErrors = notSentErrors.GroupBy(x => x.SurveySid);
            foreach (var group in groupedErrors)
            {
                var scheduleId = group.First().ScheduleID;
                var exceptionDescriptions = group.Select(x => new SchedulingScriptNotificatorExceptionDescription(x)).ToList();

                _schedulingScriptNotificator.Notify(exceptionDescriptions, -1, group.Key, scheduleId);
            }

            _scheduleErrorRepository.SetNotificationSent(notSentErrors.Select(x => x.Id).ToList());
        }
    }
}
