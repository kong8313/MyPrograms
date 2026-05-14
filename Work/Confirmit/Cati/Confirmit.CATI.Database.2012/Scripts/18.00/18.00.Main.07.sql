DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
 ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
 (
  SELECT 'Dialer.AllCatiServicesAreStartedEstimatedTime', 'AllCatiServicesAreStartedEstimatedTime', 'Telephony', 'Approximate time required to start all Cati services (in ms).', 1, 0, '300000'
 )
 INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  SELECT d.* FROM Data d LEFT JOIN BvSystemSettings ss ON d.[SystemName] = ss.[SystemName] WHERE ss.[SystemName] IS NULL
END


GO
PRINT N'Creating [dbo].[BvDialerState]...';


GO
CREATE TABLE [dbo].[BvDialerState] (
    [ServerName]                       VARCHAR (50) NOT NULL,
    [DialerId]                         INT          NOT NULL,
    [LatestGetStateRequestDateTime]    DATETIME     NOT NULL,
    [LatestSuccessfulGetStateDateTime] DATETIME     NOT NULL,
    [LatestDialerNotificationDateTime] DATETIME     NOT NULL,
    CONSTRAINT [PK_BvDialerState_ServerName] PRIMARY KEY CLUSTERED ([ServerName] ASC, [DialerId] ASC)
);


GO
PRINT N'Creating DF_BvDialerState_LatestDialerNotificationDateTime...';


GO
ALTER TABLE [dbo].[BvDialerState]
    ADD CONSTRAINT [DF_BvDialerState_LatestDialerNotificationDateTime] DEFAULT ('01/01/1900') FOR [LatestDialerNotificationDateTime];


GO
PRINT N'Creating DF_BvDialerState_LatestGetStateRequestDateTime...';


GO
ALTER TABLE [dbo].[BvDialerState]
    ADD CONSTRAINT [DF_BvDialerState_LatestGetStateRequestDateTime] DEFAULT ('01/01/1900') FOR [LatestGetStateRequestDateTime];


GO
PRINT N'Creating DF_BvDialerState_LatestSuccessfulGetStateDateTime...';


GO
ALTER TABLE [dbo].[BvDialerState]
    ADD CONSTRAINT [DF_BvDialerState_LatestSuccessfulGetStateDateTime] DEFAULT ('01/01/1900') FOR [LatestSuccessfulGetStateDateTime];


GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Update complete.';


GO
