DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  DELETE FROM BvSystemSettings
  WHERE SystemName='Toggle.StopUsingXp'
END


GO
PRINT N'Update complete.';


GO
