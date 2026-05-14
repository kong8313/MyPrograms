PRINT N'SchedulingScript.MaxActionsToExecute system setting';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'SchedulingScript.MaxActionsToExecute', 'Maximum amount of actions that will be executed in scheduling script', 'Scheduling script', 'Maximum amount of actions in scheduling script that can be executed without raising an error.', 1, 0, '1000'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END


GO
PRINT N'Update complete.';


GO
