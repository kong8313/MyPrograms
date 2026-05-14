using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class CopyToDefaultSchedulingScriptEventParameters : ManagementActivityEventDetails
    {
        public int? CustomSchedulingScriptId { get; set; }
        public string CustomSchedulingScriptName { get; set; }
    }
    /// <summary>
    /// Occurs when new script is created
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.CreateScript)]
    public class CreateScriptEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public CreateScriptEvent(int scriptSid, string scriptName):
            base(ManagementEventCategory.SchedulingScript, ManagementEvent.CreateScript)
        {
            ObjectId = scriptSid;
            ObjectName = scriptName;
        }
    }

    /// <summary>
    /// Occurs when script properties (script name) are updated
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.UpdateScript)]
    public class UpdateScriptEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public UpdateScriptEvent(int scriptSid, string scriptName):
            base(ManagementEventCategory.SchedulingScript, ManagementEvent.UpdateScript)
        {
            ObjectId = scriptSid;
            ObjectName = scriptName;
        }
    }

    /// <summary>
    /// Occurs when script is imported
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.ScriptImport)]
    public class ScriptImportEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ScriptImportEvent(int scriptSid, string scriptName):
            base(ManagementEventCategory.SchedulingScript, ManagementEvent.ScriptImport)
        {
            ObjectId = scriptSid;
            ObjectName = scriptName;
        }
    }

    /// <summary>
    /// Occurs when script is deleted
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.ScriptDelete)]
    public class ScriptDeleteEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ScriptDeleteEvent(int scriptSid, string scriptName):
            base(ManagementEventCategory.SchedulingScript, ManagementEvent.ScriptDelete)
        {
            ObjectId = scriptSid;
            ObjectName = scriptName;
        }
    }

    /// <summary>
    /// Occurs when script is duplicated
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.ScriptDuplicate)]
    public class ScriptDuplicateEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ScriptDuplicateEvent():
            base(ManagementEventCategory.SchedulingScript, ManagementEvent.ScriptDuplicate)
        {
        }
    }
    /// <summary>
    /// Occurs when user copy the contents of custom script to default
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.CopyToDefaultSchedulingScript)]
    public class CopyToDefaultSchedulingScriptEvent : ManagementActivityEvent<CopyToDefaultSchedulingScriptEventParameters>
    {
        public CopyToDefaultSchedulingScriptEvent(int defaultScriptId, string defaultScriptName, int customScriptId, string customScriptName):
            base(ManagementEventCategory.SchedulingScript, ManagementEvent.CopyToDefaultSchedulingScript)
        {
            ObjectId = defaultScriptId;
            ObjectName = defaultScriptName;
            Details = new CopyToDefaultSchedulingScriptEventParameters()
            {
                CustomSchedulingScriptId = customScriptId,
                CustomSchedulingScriptName = customScriptName
            };
        }
    }

    /// <summary>
    /// Occurs when scheduling script is launched
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.ScriptLaunch)]
    public class ScriptLaunchEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ScriptLaunchEvent(int scriptSid, string scriptName):
            base(ManagementEventCategory.SchedulingScript, ManagementEvent.ScriptLaunch)
        {
            ObjectId = scriptSid;
            ObjectName = scriptName;
        }
    }

    /// <summary>
    /// Occurs when certain scheduling script is saved
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.ScriptSave)]
    public class ScriptSaveEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ScriptSaveEvent(int scriptSid, string scriptName):
            base(ManagementEventCategory.SchedulingScript, ManagementEvent.ScriptSave)
        {
            ObjectId = scriptSid;
            ObjectName = scriptName;
        }
    }
}