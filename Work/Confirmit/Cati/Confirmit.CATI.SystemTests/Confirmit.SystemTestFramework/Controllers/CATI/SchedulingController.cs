using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Script.Classes;
using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework.Controllers.CATI
{
    public class SchedulingController : TestController
    {
        public SchedulingController(UserInfo userInfo)
        {
            UserInfo = userInfo;
        }

        public string Load(string path)
        {
            return Create(File.ReadAllText(path));
        }

        public string Create(string xml)
        {
            var serializer = new XmlSerializer(typeof(List<SchedulingScript>));
            var schedulingScript = ((List<SchedulingScript>)serializer.Deserialize(new StringReader(xml))).First();

            var scheduleEntity = ScheduleRepository.GetByName(schedulingScript.Name);

            using (var transactionScope = new DatabaseTransactionScope("ScriptsList.ImportScript", DeadlockPriority.Supervisor))
            {
                if (scheduleEntity == null)
                {
                    scheduleEntity = ScheduleManager.AddSchedule(schedulingScript.Name);
                }

                var evt = new ScriptImportEvent(scheduleEntity.ScheduleID, scheduleEntity.Name);
                var bvschedule = ScheduleRepository.GetById(scheduleEntity.ScheduleID);
                bvschedule.XmlUnderDev = ScheduleManager.SerializeSchedule(schedulingScript.Schedule);
                ScheduleRepository.Update(bvschedule);
                evt.Finish();
                transactionScope.Commit();
            }

            //var currentContext = HttpContext.Current;

            var httpRequest = new HttpRequest("", "http://test/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            HttpContext.Current = new HttpContext(httpRequest, httpResponse)
            {
                User = new SupervisorPrincipal(Properties.Settings.Default.Login,
                    UserInfo.ClientKey,
                    Properties.Settings.Default.CompanyId,
                    Properties.Settings.Default.CompanyName,
                    Tabs.None, true, true, true)
            };

            var schedule = ScheduleRepository.GetByName(schedulingScript.Name);
            schedule.XmlUnderDev = ScheduleManager.SerializeSchedule(schedulingScript.Schedule);
            ServiceLocator.Resolve<ISupervisorServiceClient>().LaunchSchedule(schedule.ScheduleID);

            //HttpContext.Current = currentContext;

            return schedulingScript.Name;
        }
    }
}