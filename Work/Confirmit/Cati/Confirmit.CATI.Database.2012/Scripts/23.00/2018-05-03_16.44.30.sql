DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
        SELECT 'Console.KeepAliveCallsToSave', 'KeepAliveCallsToSave', 'Interviewing', 'Number of KeepAlive calls to use when calculating current connection status ', 1, 0, '3'
		UNION
        SELECT 'Console.GoodConnectionThresholdMs', 'GoodConnectionThresholdMs', 'Interviewing', 'Threshold for good connection status indicator in milliseconds', 1, 0, '300'
		UNION
        SELECT 'Console.NormalConnectionThresholdMs', 'NormalConnectionThresholdMs', 'Interviewing', 'Threshold for normal connection status indicator in milliseconds', 1, 0, '1000'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END
GO

PRINT N'Update complete.';
GO