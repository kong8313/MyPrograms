PRINT N'Add Console.ForceUpdateToNewVersion setting';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Console.ForceUpdateToNewVersion', 'Force update to new version', 'Interviewing', 'This setting controls when the interviewer console is updated to a new version. If it is set true then it must be before a new interview can be started. If it is set false then it must be before the interviewer console application can be launched.', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END


GO
PRINT N'Update complete.';


GO
