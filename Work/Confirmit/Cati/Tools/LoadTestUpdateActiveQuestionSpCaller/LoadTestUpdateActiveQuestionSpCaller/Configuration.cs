namespace LoadTestUpdateActiveQuestionSpCaller;

public class Configuration
{
    public string CatiSqlServerName { get; set; }
    public int CatiCompanyId { get; set; }
    public int InitialInterviewerSID { get; set; }
    public int InterviewersAmount { get; set; }
    public int SpCallMinIntervalInMilliseconds { get; set; }
    public int SpCallMaxIntervalInMilliseconds { get; set; }
    public int TestDurationInSeconds { get; set; }
}