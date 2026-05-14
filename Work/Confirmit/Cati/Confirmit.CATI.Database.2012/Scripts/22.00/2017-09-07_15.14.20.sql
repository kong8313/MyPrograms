DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
  	SELECT 'SchedulingScript.EnableRestrictedMode', 'Use restricted mode', 'Scheduling script', 'Enable restricted mode to check custom code.', 3, 0, 'False'
  	UNION ALL
  	SELECT 'SchedulingScript.SecureExternalMethods', 'Secure external methods', 'Scheduling script', 'List of secure methods which can be called from scheduling script assembly.', 2, 0, ''
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END
GO

PRINT N'Update complete.';
GO
