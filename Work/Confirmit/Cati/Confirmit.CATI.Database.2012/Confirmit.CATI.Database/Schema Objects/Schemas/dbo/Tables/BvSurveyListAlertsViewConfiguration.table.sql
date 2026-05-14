CREATE TABLE [dbo].[BvSurveyListAlertsViewConfiguration] (
    [UpdatingTime]     INT      NOT NULL CONSTRAINT DF_BvSurveyListAlertsViewConfiguration_UpdatingTime DEFAULT (15),
    [LastCall]         DATETIME NULL,
    [SyncUpdatingTime] INT      NOT NULL CONSTRAINT DF_BvSurveyListAlertsViewConfiguration_SyncUpdatingTime DEFAULT (3600),
    [SyncLastCall]     DATETIME NULL,
    [IdlePeriodMaxCountOfChecks] int NOT NULL CONSTRAINT DF_BvSurveyListAlertsViewConfiguration_IdlePeriodMaxCountOfChecks DEFAULT(60), --60 time (ever 15 sec) we monitor interviewers' activity
                                               --if there is no such we execute synchronize sp.
    [IdlePeriodCheckCounter] int NOT NULL CONSTRAINT DF_BvSurveyListAlertsViewConfiguration_IdlePeriodCheckCounter DEFAULT(0),
    [IdlePeriodMaxSeconds] int NOT NULL CONSTRAINT DF_BvSurveyListAlertsViewConfiguration_IdlePeriodMaxSeconds DEFAULT(3600) --(in seconds) If SecondSinceLastSubmission of interviewier is greater
);

