CREATE TABLE [dbo].[BvMonthAgregatedHistory]
(
	[SurveyId] INT NOT NULL,
	[PersonId] INT NOT NULL,
	[ITS] INT NOT NULL,
	[LogonTime] INT NOT NULL,
	[WaitingTime] INT NOT NULL,
	[DailingsCount] INT NOT NULL,
	[StartTime] DATETIME NOT NULL
	CONSTRAINT PK_BvMonthAgregatedHistory PRIMARY KEY CLUSTERED([SurveyId], [PersonId], [ITS], [StartTime])
)
