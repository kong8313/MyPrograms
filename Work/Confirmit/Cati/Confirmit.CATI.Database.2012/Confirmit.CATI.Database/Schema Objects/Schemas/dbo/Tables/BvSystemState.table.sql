CREATE TABLE [dbo].[BvSystemState]
(
	[SystemName] NVARCHAR(256) NOT NULL , 
    [Value] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_BvSystemState_SystemName] PRIMARY KEY ([SystemName])
)
