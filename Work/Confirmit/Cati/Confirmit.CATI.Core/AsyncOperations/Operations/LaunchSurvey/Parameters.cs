using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.LaunchSurvey
{

    public class SurveyProperties
    {
        public string CfSqlServerConnectionString { get; set; }
        public string CreatedUserName { get; set; }
        public string ProjectName { get; set; }
        public int? DialingMode { get; set; }
        public bool? OpenEndReview { get; set; }
        public bool LiveMonitoring { get; set; }
        public bool? VoiceRecording { get; set; }
        public bool? ScreenRecording { get; set; }
        public bool SupportBlacklist { get; set; }
        public bool AllowRespondentsDynamicCreation { get; set; }
        public string NotificationEmail { get; set; }
        public bool EnforceHttps { get; set; }
        public bool ReplicationStatus { get; set; }
    }

    public class Parameters : IAsyncOperationParameters
    {
        public int SurveyId { get; set; }
        public string ProjectId { get; set; }
        public bool RemoveData { get; set; }
        public SurveyProperties SurveyProperties { get; set; }
        public string[] PermittedUsers { get; set; }
        public TableInfo[] ReplicatedTables { get; set; }
    }

    
}
