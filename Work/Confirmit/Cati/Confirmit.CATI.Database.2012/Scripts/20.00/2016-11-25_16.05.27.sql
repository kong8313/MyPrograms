DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'RetryingService.DelayBetweenRetriesInMilliseconds', 'DelayBetweenRetriesInMilliseconds', 'RetryingService', 'Delay between retries in milliseconds.', 1, 0, '1000'
	UNION ALL
	SELECT 'RetryingService.NumberOfRetryAttempts', 'NumberOfRetryAttempts', 'RetryingService', 'Number of retry attempts', 1, 0, '5'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END


GO
PRINT N'Update complete.';


GO
