namespace Confirmit.CATI.Supervisor.Core.Common
{
    public class SupervisorPageProvider
    {
        public string NewSupervisorLink { get; } = "/catisupervisor/";

        public string GetNewSupervisorPage(StartPages oldSupervisorPage)
        {
            switch (oldSupervisorPage)
            {
                case StartPages.CATIsurvey:
                    return $"{NewSupervisorLink}surveys/";
                case StartPages.CATIinterviewer:
                    return $"{NewSupervisorLink}interviewers/";
                case StartPages.Scheduling:
                    return $"{NewSupervisorLink}scheduling/";
                case StartPages.Reports:
                    return $"{NewSupervisorLink}reports/surveyoverview/";
                case StartPages.ActivityViews:
                    return $"{NewSupervisorLink}activity/";
                case StartPages.RecordedInterviews:
                    return $"{NewSupervisorLink}recordings/";
                case StartPages.CallCenters:
                    return $"{NewSupervisorLink}callcenters/";
                case StartPages.Resources:
                    return $"{NewSupervisorLink}resources/stategroups/";
                default:
                    return NewSupervisorLink;
            }
        }
    }
}
