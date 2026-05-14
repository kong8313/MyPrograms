CREATE TABLE [dbo].[BvHistoryCustomFields]
(
	[Id] INT NOT NULL CONSTRAINT [PK_BvHistoryCustomFields] PRIMARY KEY,
	[SourceTable] INT NOT NULL, 
    [SourceFieldName] NVARCHAR(50) NOT NULL,
	[DisplayName] NVARCHAR(50),
	[Description] NVarchar(255),
	[IsActive] BIT NOT NULL CONSTRAINT DF_BvHistoryCustomFields_IsActive DEFAULT (1), 
)