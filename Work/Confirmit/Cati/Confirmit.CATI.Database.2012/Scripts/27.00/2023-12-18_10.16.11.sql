PRINT N'Set true to Toggle.RabbitMqCacheInvalidation setting';

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  UPDATE BvSystemSettings
  SET [BvSystemSettings].[Value] = 'True'
  WHERE [BvSystemSettings].[SystemName] = 'Toggle.RabbitMqCacheInvalidation'
END
GO
PRINT N'Update complete.';


GO

PRINT N'Set true to Toggle.DisableClrForFcd setting';

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName like 'ConfirmitCATIV15[_]%' )
BEGIN
  UPDATE BvSystemSettings
  SET [BvSystemSettings].[Value] = 'True'
  WHERE [BvSystemSettings].[SystemName] = 'Toggle.DisableClrForFcd'
END
GO
PRINT N'Update complete.';


GO
PRINT N'Add async operation to  migrate all surveys ';

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName like 'ConfirmitCATIV15[_]%' )
BEGIN
  INSERT INTO
[BvAsyncOperationQueue] (IsInitiatedBySystem, Type, Title, State, Parameters, SurveySid, Priority, QueuedDate, CreatedBySupervisorName, CallCenterId, TotalItemsCount, ProcessedItemsCount, FailedItemsCount, Server)
VALUES(0, 21, 'Disable CLR in FCD operations for all Surveys', 0, '<Parameters xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><SurveyId>0</SurveyId></Parameters>', 0, 30, 
GETUTCDATE(), 'administrator', 0, 0, 0, 0, '')

END
GO
PRINT N'Update complete.';


GO
PRINT N'Update complete.';
GO
