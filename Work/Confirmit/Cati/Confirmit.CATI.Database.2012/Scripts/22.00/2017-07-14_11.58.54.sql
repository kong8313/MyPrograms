DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Console.EnableAbilityToCancelDial', 'ConsoleEnableAbilityToCancelDial', 'Interviewing', 'Is Interviewer Console ability to cancel dialling process enabled', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END
GO

PRINT N'Update complete.';
GO