PRINT N'Add Supervisor.ShowClassicStyleButton setting';

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Supervisor.ShowClassicStyleButton', 'Show classic style setting button in supervisor', 'Supervisor', 'Show classic style setting button in CATI Supervisor', 3, 0, 'True'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END
GO

PRINT N'Update complete.';
GO