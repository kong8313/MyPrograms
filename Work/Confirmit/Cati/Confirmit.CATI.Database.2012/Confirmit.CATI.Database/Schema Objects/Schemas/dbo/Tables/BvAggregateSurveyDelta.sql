CREATE TABLE [dbo].[BvAggregateSurveyDelta] (
    [ID]                          BIGINT   NOT NULL IDENTITY,
	[SID]                         INT      NOT NULL,
    [ScheduledCallsCount]         INT      NOT NULL CONSTRAINT DF_BvAggregateSurveyDelta_ScheduledCallsCount DEFAULT(0),
    [SuspendedCallsCount]         INT      NOT NULL CONSTRAINT DF_BvAggregateSurveyDelta_SuspendedCallsCount DEFAULT(0),
    [MinutesSpentWorkingOnSurvey] INT      NOT NULL CONSTRAINT DF_BvAggregateSurveyDelta_MinutesSpentWorkingOnSurvey DEFAULT(0),
	CONSTRAINT [BvAggregateSurveyDelta_PK_ID] PRIMARY KEY CLUSTERED ([ID])
);
GO

ALTER TABLE [dbo].[BvAggregateSurveyDelta] SET (LOCK_ESCALATION = DISABLE)
GO