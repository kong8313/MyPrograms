using System;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Script.Classes;
using Confirmit.CATI.Supervisor.Script.Controls;

namespace Confirmit.CATI.Supervisor.Script.ScriptViewTabs
{
    public partial class ScriptViewSchedulingRulesNew : BaseForm
    {
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new SessionPageStatePersister(this);
            }
        }        

        [ScriptMethod, WebMethod(EnableSession = true)]
        public static SchedulingRulesOperationResult EnableAction(string actionKey, bool enabled, string scriptId)
        {
            return SchedulingRulesNewControl.EnableAction(actionKey, enabled, scriptId);
        }

        [ScriptMethod, WebMethod(EnableSession = true)]
        public static SchedulingRulesOperationResult DeleteRow(string rowKey, string scriptId)
        {
            return SchedulingRulesNewControl.Delete(rowKey, scriptId);
        }

        [ScriptMethod, WebMethod(EnableSession = true)]
        public static SchedulingRulesOperationResult MoveRow(string rowKey, bool moveUp, string scriptId)
        {
            return SchedulingRulesNewControl.Move(rowKey, moveUp, scriptId);
        }

        [ScriptMethod, WebMethod(EnableSession = true)]
        public static SchedulingRulesOperationResult PasteRow(string copiedRowKey, string pasteRowKey, string scriptId)
        {
            return SchedulingRulesNewControl.Paste(copiedRowKey, pasteRowKey, scriptId);
        }
    }
}