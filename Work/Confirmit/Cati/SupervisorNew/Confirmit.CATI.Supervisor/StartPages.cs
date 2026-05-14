namespace Confirmit.CATI.Supervisor
{
    /// <summary>
    /// Defines avaliable CP start pages, when starting CP from Confirmit.
    /// </summary>
    public enum StartPages
    {
        None = -1,
        Default = 0,
        ProductivityReport = 1,
        CATIsurvey = 2,
        CATIinterviewer = 3,
        Resources = 4,
        Scheduling = 5,
        Reports = 6,
        ActivityViews = 7,
        RecordedInterviews = 8,
        CallCenters = 9
    }
}