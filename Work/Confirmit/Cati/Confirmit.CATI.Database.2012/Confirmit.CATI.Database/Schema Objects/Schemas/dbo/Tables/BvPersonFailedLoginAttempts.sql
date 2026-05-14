CREATE TABLE [dbo].[BvPersonFailedLoginAttempts]
(
	[PersonId] INT NOT NULL CONSTRAINT PK_BvPersonFailedLoginAttempts PRIMARY KEY,
	[Count] INT NOT NULL CONSTRAINT DF_BvPersonFailedLoginAttempts_Count DEFAULT (0)
)
