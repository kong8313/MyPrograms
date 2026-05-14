PRINT N'Set true to Console.UseHttpsForConsoleStateService setting';

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  UPDATE BvSystemSettings
  SET [BvSystemSettings].[Value] = 'True'
  WHERE [BvSystemSettings].[SystemName] = 'Console.UseHttpsForConsoleStateService'
END


GO
PRINT N'Update complete.';