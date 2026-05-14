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
