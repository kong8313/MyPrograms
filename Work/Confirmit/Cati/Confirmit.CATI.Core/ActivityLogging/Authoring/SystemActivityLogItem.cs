namespace Confirmit.CATI.Core.ActivityLogging.Authoring
{
    public class SystemActivityLogItem
    {
        public SystemActivityType ActivityType { get; }
        public string ProjectId { get; }
        public string UserName { get; }
        public int CompanyId { get; }
        public string Description { get; }

        public SystemActivityLogItem(SystemActivityType activityType, int companyId, string description, string projectId = "", string userName = "")
        {
            ActivityType = activityType;
            CompanyId = companyId;
            Description = description;
            ProjectId = projectId;
            UserName = userName;
        }
    }
}