using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Script.Controls
{
    public partial class CustomScriptPage : ScheduleControlBase
    {
        private readonly ISchedulingObjectValidator _validator;
        protected string KeepSessionUrl => ConfigHelper.ConfirmitKeepSessionAspxUrl;

        public CustomScriptPage()
        {
            _validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            StateChecker.AddSaveButton(btnSave);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            codeEditor.Text = WorkingSchedule.CustomScript.Body;
            btnReference.OnClientClick = $"window.open('{BaseRelativePath("HelpPages/CustomScripting.html")}', '1', 'toolbar=no,status=no,resizable,width=800,height=600')";
        }

        protected void LaunchClick(object sender, EventArgs e)
        {
            if (SaveCustomScript())
            {
                ScheduleLaunchHandler(sender, e);
            }
        }

        protected void SaveClick(object sender, EventArgs e)
        {
            if (SaveCustomScript())
            {
                ScheduleSaveHandler(sender, e);
            }
        }

        /// <summary>
        /// Checks and saves custom script. 
        /// </summary>        
        /// <returns>True if script has been successfully saved otherwise false</returns>
        private bool SaveCustomScript()
        {
            ErrorCollection errors;

            var script = new CustomScript { Id = 1, Body = codeEditor.Text };

            if (_validator.Validate(script, out errors))
            {
                WorkingSchedule.CustomScript = script;

                return true;
            }
            else
            {
                NotifyUser(errors);

                return false;
            }
        }

        public override void AddConfirmationWhileLaunch()
        {
            btnLaunch.Attributes["onclick"] = string.Format("if (!confirm(\"{0}\")) return false;", Strings.LaunchScriptConfirmation);
        }

        protected override string ClientControllerName
        {
            get { return null; }
        }
    }
}