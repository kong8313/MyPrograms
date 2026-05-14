CREATE TABLE [dbo].[BvSchedulingScriptLog]
(
    [Id] INT IDENTITY (1, 1)     NOT NULL,
    [ScheduleID]  INT            NOT NULL,
    [SurveySid]   INT            NOT NULL,
    [InterviewId] INT            NOT NULL,
    [Timestamp]   DATETIME       NOT NULL,
    [LogMessages] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_BvSchedulingScriptLog_ID] PRIMARY KEY NONCLUSTERED ([ID])
)

GO

CREATE Clustered INDEX [IX_BvSchedulingScriptLog_SurveySid_InterviewId] ON [dbo].[BvSchedulingScriptLog] ([SurveySid], [InterviewId])

GO

CREATE INDEX [IX_BvSchedulingScriptLog_TimeStamp] ON [dbo].[BvSchedulingScriptLog] ([Timestamp])
