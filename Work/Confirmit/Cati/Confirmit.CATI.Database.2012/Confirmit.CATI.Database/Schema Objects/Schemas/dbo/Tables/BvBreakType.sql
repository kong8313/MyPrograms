CREATE TABLE [dbo].[BvBreakType]
(
	[Id] INT IDENTITY(1,1) CONSTRAINT PK_BvBreakType PRIMARY KEY CLUSTERED,
	[Name] NVARCHAR(256) NOT NULL CONSTRAINT UC_BvBreakType UNIQUE([Name]),
	[Description] NVARCHAR(MAX) NOT NULL, 
	[IsPaid] BIT NOT NULL, 
    [YellowThreshold] INT NULL, 
    [RedThreshold] INT NULL
)