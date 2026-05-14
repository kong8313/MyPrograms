GO
PRINT N'Add Toggle.Supervisor.EnableScriptErrorsLogging system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%')
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Toggle.Supervisor.EnableScriptErrorsLogging', 'Enable CatiSupervisor logging for script errrors', 'Toggle', 'Enable logging of uncaught JavaScript exceptions for CatiSupervisor', 3, 0, 'True'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
