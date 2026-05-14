CREATE TABLE [dbo].[BvAlerts]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
    [Type] NVARCHAR(100) NOT NULL, 
    [TriggerTime] DATETIME NOT NULL, 
    [CallCenterId] INT NOT NULL, 
    CONSTRAINT [PK_BvAlerts] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC 
	) ON [PRIMARY],
)
