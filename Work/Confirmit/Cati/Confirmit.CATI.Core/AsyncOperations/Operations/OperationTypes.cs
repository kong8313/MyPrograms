namespace Confirmit.CATI.Core.AsyncOperations.Operations
{
    public enum OperationTypes : byte
    {
        ActivateCalls,
        EnableCalls,
        MoveCalls,
        MoveAndRescheduleCalls,
        ChangePriorityOfCalls,
        ChangeShiftTypeOfCalls,
        AssignCalls,
        RestoreSurvey,
        DeactivateCalls,
        ChangeDialModeOfInterviews,
        LaunchSurvey,
        DeleteSurvey,
        ConfigureClusteredQuota,
        DeleteRespondents,
        UpdateFcdQuota,
        ExecuteRoutineMaintenance,
        DeleteCallsByBlacklist,
        InitializeDeleteCallsByBlacklist,
        EditCalls,
        SynchronizeRespondents,
        RereadSurveyReplicatedData,
        SampleUpload
    }
}
