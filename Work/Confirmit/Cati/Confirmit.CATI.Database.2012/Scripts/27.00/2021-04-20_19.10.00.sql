PRINT N'Server.CreateCompanyDatabasesFromBackup system settings';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Server.CreateCompanyDatabasesFromBackup', 'Create CATI databases from backup', 'System', ' When enabled, new CATI databases for companies will be created from backup of main CATI database. Otherwise - using database deploy. Should be ''False'' for Azure Managed SQL', 3, 0, 'True'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
