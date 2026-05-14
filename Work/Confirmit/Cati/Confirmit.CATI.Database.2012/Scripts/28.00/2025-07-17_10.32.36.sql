GO
PRINT N'Upadate system setting CallManagement.PageSize';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%')
BEGIN
  UPDATE BvSystemSettings
  SET [Value] = '100'
  WHERE [SystemName] = 'CallManagement.PageSize'
END

GO
PRINT N'Update complete.';