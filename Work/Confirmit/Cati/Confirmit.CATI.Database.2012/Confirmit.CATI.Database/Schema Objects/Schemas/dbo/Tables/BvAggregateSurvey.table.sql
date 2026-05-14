CREATE TABLE [dbo].[BvAggregateSurvey] (
    [SID]                         INT      NOT NULL,
    [ScheduledCallsCount]         INT      NOT NULL CONSTRAINT DF_BvAggregateSurvey_ScheduledCallsCount DEFAULT(0),
    [SuspendedCallsCount]         INT      NOT NULL CONSTRAINT DF_BvAggregateSurvey_SuspendedCallsCount DEFAULT(0),
    [MinutesSpentWorkingOnSurvey] INT      NOT NULL CONSTRAINT DF_BvAggregateSurvey_MinutesSpentWorkingOnSurvey DEFAULT(0)
);

