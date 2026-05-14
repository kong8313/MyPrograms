GO
PRINT N'Creating [dbo].[BvSpDialerState_InsertUpdateGetStateTime]...';

GO
CREATE PROCEDURE [dbo].[BvSpDialerState_InsertUpdateGetStateTime]
	@serverName VARCHAR(50),
	@dialerId int,
	@latestGetStateRequestDateTime DATETIME,
	@isGetStateSuccessful BIT
AS
	MERGE BvDialerState as target
	USING ( 
		SELECT @serverName, @dialerId, @latestGetStateRequestDateTime, @isGetStateSuccessful) 
			AS source(ServerName, DialerId, LatestGetStateRequestDateTime, IsGetStateSuccessful)
	ON (target.ServerName = source.ServerName AND target.DialerId = source.DialerId)
	WHEN MATCHED THEN 
	   UPDATE SET 
			LatestGetStateRequestDateTime = @latestGetStateRequestDateTime,
			LatestSuccessfulGetStateDateTime = (CASE WHEN @isGetStateSuccessful = 1 THEN @latestGetStateRequestDateTime ELSE LatestSuccessfulGetStateDateTime END)
	WHEN NOT MATCHED THEN	
		INSERT ( [ServerName], [DialerId], [LatestGetStateRequestDateTime], [LatestSuccessfulGetStateDateTime])
		VALUES ( 
				@serverName, 
				@dialerId, 
				@latestGetStateRequestDateTime, 
				(CASE WHEN @isGetStateSuccessful = 1 THEN @latestGetStateRequestDateTime ELSE ('01/01/1900') END)
			   );
GO
PRINT N'Creating [dbo].[BvSpDialerState_UpdateNotificationAndExpirationTime]...';


GO
CREATE PROCEDURE [dbo].[BvSpDialerState_UpdateNotificationAndExpirationTime]
	@serverName VARCHAR(50),
	@dialerId INT,
	@latestNotificationDateTime DATETIME,
	@notificationExpirationTime DATETIME
AS
	UPDATE BvDialerState 
	   SET 
			LatestDialerNotificationDateTime = @latestNotificationDateTime,
			DialerNotificationExpirationTime = (CASE WHEN @notificationExpirationTime IS NULL THEN DialerNotificationExpirationTime ELSE @notificationExpirationTime END)
	   WHERE
			DialerId = @dialerId AND ServerName = @serverName
GO
PRINT N'Update complete.';


GO
