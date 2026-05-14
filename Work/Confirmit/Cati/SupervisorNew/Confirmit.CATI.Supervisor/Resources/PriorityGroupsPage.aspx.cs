using System;
using System.Web.Script.Services;
using System.Web.Services;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class PriorityGroupsPage : BaseForm
    {
        public override string TopTitle
        {
            get
            {
                return Strings.CallGroups;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [ScriptMethod, WebMethod(EnableSession = true)]
        public static object HasGroupAssignments(int groupId)
        {
            var hasGroupAssignments = false;
            string warningText = String.Empty;

            try
            {
                var group = ServiceLocator.Resolve<IPriorityGroupsManager>().GetGroup(groupId);

                hasGroupAssignments = PersonRepository.GetAllAssignedOnCallGroup(groupId).Count > 0;

                if (hasGroupAssignments)
                {
                    warningText = string.Format(Strings.CallGroupContainsUsersWarning, group.Name);
                }

            }
            catch (Exception ex)
            {
                ExceptionTraceHelper.TraceException(ex);
            }

            return new { HasAssignments = hasGroupAssignments, WarningText = warningText };
        }
    }
}