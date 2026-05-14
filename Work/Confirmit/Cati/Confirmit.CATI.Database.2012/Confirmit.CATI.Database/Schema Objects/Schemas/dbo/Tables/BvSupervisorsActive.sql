CREATE TABLE [dbo].[BvSupervisorsActive]
(
	[UserName] NVARCHAR(255) NOT NULL CONSTRAINT PK_BvSupervisorsActive PRIMARY KEY CLUSTERED, 
    [LastActiveTime] DATETIME NOT NULL, 
    [Connections] INT NOT NULL, 
    [CallCenterId] INT NOT NULL
)
