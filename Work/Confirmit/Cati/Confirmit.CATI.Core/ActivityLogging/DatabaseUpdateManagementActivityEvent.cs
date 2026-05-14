using System;

namespace  Confirmit.CATI.Core.ActivityLogging
{
    public class DatabaseUpdateManagementActivityEvent<TDetails> : ManagementActivityEventBase<TDetails> where TDetails : ManagementActivityEventDetails, new()
    {
        /// <summary>
        /// Gets or sets the duration of activity event.
        /// </summary>
        public int DurationInMilliseconds;

        public void Save(string confirmlogConnectionString)
        {
            Save(confirmlogConnectionString, DurationInMilliseconds);
        }

        protected DatabaseUpdateManagementActivityEvent(ManagementEvent eventType) : base(ManagementEventCategory.System, eventType)
        {
        }
    }

    [Serializable]
    public class DatabaseUpdateScriptApplyingParameters : ManagementActivityEventDetails
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public string BranchName { get; set; }
        public int ScriptNumber { get; set; }

        public string Description { get; set; }
        public bool IsAppliedDuringDBCreation { get; set; }
        public string ScriptOutput { get; set; }
        public string ScriptText { get; set; }
    }

    [Serializable]
    public class DatabaseUpdateFinishEventParameters : ManagementActivityEventDetails
    {
        public string Description { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.DatabaseUpdateScriptApplying)]
    public class DatabaseUpdateScriptApplyingEvent : DatabaseUpdateManagementActivityEvent<DatabaseUpdateScriptApplyingParameters>
    {
        public DatabaseUpdateScriptApplyingEvent(DateTime stopTime, int companyId, string serverName, string userName, int durationInMilliseconds, DatabaseUpdateScriptApplyingParameters details):
            base(ManagementEvent.DatabaseUpdateScriptApplying)
        {
            DurationInMilliseconds = durationInMilliseconds;
            StartTime = stopTime.AddMilliseconds(-durationInMilliseconds);
            CompanyId = companyId;
            ServerName = serverName;
            Supervisor = userName;
            ObjectId = 0;
            ObjectName = "Database";

            Details = details;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DatabaseUpdateFinish)]
    public class DatabaseUpdateFinishEvent : DatabaseUpdateManagementActivityEvent<DatabaseUpdateFinishEventParameters>
    {
        public DatabaseUpdateFinishEvent(DateTime stopTime, int companyId, string serverName, string userName, int durationInMilliseconds, string description):
            base(ManagementEvent.DatabaseUpdateFinish)
        {
            DurationInMilliseconds = durationInMilliseconds;
            StartTime = stopTime.AddMilliseconds(-durationInMilliseconds);
            CompanyId = companyId;
            ServerName = serverName;
            Supervisor = userName;
            ObjectId = 0;
            ObjectName = "Database";

            Details = new DatabaseUpdateFinishEventParameters
            {
                Description = description,
            };
        }
    }
}