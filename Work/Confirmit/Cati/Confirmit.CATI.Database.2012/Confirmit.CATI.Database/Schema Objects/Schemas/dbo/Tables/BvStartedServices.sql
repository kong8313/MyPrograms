CREATE TABLE [dbo].[BvStartedServices]
(
    [MachineName] NVARCHAR(128) NOT NULL, 
    [ServiceName] NVARCHAR(128) NOT NULL, 
    CONSTRAINT PK_BvStartedServices PRIMARY KEY ([MachineName], [ServiceName])
)
