PRINT N'Add Supervisor.AlwaysOpenNewUI setting';

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Supervisor.AlwaysOpenNewUI', 'Open new UI by default', 'Supervisor', 'Indicates whether CATI Supervisor should always be loaded in a new style on launch.', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END
GO

PRINT N'Delete Toggle.EnableNewSupervisor setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.EnableNewSupervisor'
GO

PRINT N'Update complete.';
GO