GO
PRINT N'Adding Toggle.RabbitMqCacheInvalidation system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Toggle.RabbitMqCacheInvalidation', 'Use RabbitMq cache invalidation instead of sql service broker', 'Toggle', 'Use RabbitMq cache invalidation instead of sql service broker', 3, 0, 'False'

  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
