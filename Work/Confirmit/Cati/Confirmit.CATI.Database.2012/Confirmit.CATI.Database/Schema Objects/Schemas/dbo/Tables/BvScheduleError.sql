CREATE TABLE [dbo].[BvScheduleError]
(
	[Id] INT IDENTITY NOT NULL,
    [ScheduleID] INT NOT NULL, 
    [SurveySid] INT NOT NULL, 
    [InterviewId] INT NOT NULL, 
    [Timestamp] DATETIME NOT NULL, 
    [TriggeredBy] NVARCHAR(255) NOT NULL, 
    [ExtendedStatus] NVARCHAR(255) NOT NULL, 
    [RuleNumber] NVARCHAR(255) NOT NULL, 
    [Action] NVARCHAR(255) NOT NULL, 
    [Message] NVARCHAR(MAX) NOT NULL, 
   	[NotificationSent] BIT NOT NULL CONSTRAINT DF_BvScheduleError_NotificationSent DEFAULT (0),
    CONSTRAINT [PK_BvScheduleError_ID] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [FK_BvScheduleError_BvSchedule.table] FOREIGN KEY ([ScheduleID]) REFERENCES [BvSchedule]([ScheduleID]) ON DELETE CASCADE
)

GO

CREATE CLUSTERED INDEX[IX_BvScheduleError_ScheduleID_Timestamp] ON [dbo].[BvScheduleError] ([ScheduleID], [Timestamp]) 