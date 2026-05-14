using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Script.Classes;
using Confirmit.CATI.Supervisor.ServerControls;

namespace Confirmit.CATI.Supervisor.Script.Controls
{
    public abstract class ScheduleControlBase : BaseWUC
    {
        public StateChecker StateChecker { get; private set; }
        protected PlaceHolder placeholder;
        private const string ExportFileName = "Schedule.xml";

        protected int ScheduleId
        {
            get
            {
                return Convert.ToInt32(Request.QueryString["ID"]);
            }
        }

        private BvScheduleEntity _schedule;
        private readonly IScheduleService _scheduleService;
        private readonly ISupervisorServiceClient _supervisorService;

        protected ScheduleControlBase()
        {
            StateChecker = new StateChecker();

            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
            _supervisorService = ServiceLocator.Resolve<ISupervisorServiceClient>();
        }

        public static Schedule WorkingSchedule
        {
            get 
            {
                var scheduleId = HttpContext.Current.Request.QueryString["ID"];
                return (Schedule)HttpContext.Current.Session[$"WorkingSchedule_{scheduleId}"]; 
            }
        }

        private bool ScheduleChanged
        {
            get { return (bool) (Session[$"ScheduleChanged_{ScheduleId}"] ?? false); }
            set { Session[$"ScheduleChanged_{ScheduleId}"] = value; }
        }

        public abstract void AddConfirmationWhileLaunch();

        protected abstract string ClientControllerName { get; }

        protected BvScheduleEntity Schedule
        {
            get { return _schedule ?? (_schedule = ScheduleRepository.GetById(ScheduleId)); }
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            placeholder.Controls.Add(StateChecker);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ScheduleChanged)
                {
                    StateChecker.MarkAsChanged();
                }
            }

            base.OnLoad(e);

            if (WorkingSchedule.CustomParameters.Count > 0 && _scheduleService.DoesSheduleHaveParametersInUse(ScheduleId))
            {
                AddConfirmationWhileLaunch();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Page.RegisterStartupScript(@"Common.onGlobalEvent('ScriptViewChanged', StateChecker.MarkAsChanged);");
            Page.RegisterStartupScript(@"Common.onGlobalEvent('ScriptViewSaved', StateChecker.MarkAsUnchanged);");            
            
            base.OnPreRender(e);
        }

        private void ScheduleSave()
        {
            _supervisorService.SaveSchedule(Schedule.ScheduleID, ScheduleManager.SerializeSchedule(WorkingSchedule));

            StateChecker.MarkAsUnchanged();
            ScheduleChanged = false;
            Page.RegisterStartupScript("Common.fireGlobalEvent('ScriptViewSaved');");
        }

        private void ScheduleLaunch()
        {
            BvScheduleEntity schedule = ScheduleRepository.GetById(Schedule.ScheduleID);
            
            /* TODO: We assign and do nothing??? 99% bug is here */
            schedule.XmlUnderDev = ScheduleManager.SerializeSchedule(WorkingSchedule);

            _supervisorService.LaunchSchedule(schedule.ScheduleID);            
        }

        protected void ScheduleLaunchHandler(object sender, EventArgs e)
        {
            try
            {
                ScheduleSave();
                ScheduleLaunch();
                Page.AddUserMessage(Strings.LaunchSuccessFinished);
                Page.RefreshListFrame();
            }
            catch (SchedulingScriptSyntaxErrorException ex)
            {
                ProcessErrorInScheduleScript(ex);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void ScheduleSaveHandler(object sender, EventArgs e)
        {
            try
            {
                ScheduleSave();
                Page.AddUserMessage(Strings.SaveSuccessFinished);
                Page.RefreshListFrame();
            }
            catch (SchedulingScriptSyntaxErrorException ex)
            {
                ProcessErrorInScheduleScript(ex);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void ScheduleChangedHandler(object sender, EventArgs e)
        {
            StateChecker.MarkAsChanged();
            ScheduleChanged = true;
            Page.RegisterStartupScript("Common.fireGlobalEvent('ScriptViewChanged');");
        }

        protected void ScheduleExport(object sender, EventArgs e)
        {
            var list = new List<SchedulingScript> {new SchedulingScript(Schedule.Name, WorkingSchedule)};

            Page.FileToClientSender.Send(list, ExportFileName);
        }

        protected void NotifyUser(ErrorCollection errors)
        {
            if (errors == null || errors.Count == 0) return;
            NotifyUser(String.Join("\n", errors.ToStringArray()));
        }

        protected void NotifyUser(string message)
        {
            Page.AddUserMessage(message);
        }

        private void ProcessErrorInScheduleScript(SchedulingScriptSyntaxErrorException ex)
        {
            string message = ScheduleManager.GetLaunchExeptionMessage(WorkingSchedule, ex.ErrorDetails);

            var exception = string.IsNullOrEmpty(message) ? ex : new UserMessageException(message, ex);

            Context.AddError(exception);
        }   
    }
}