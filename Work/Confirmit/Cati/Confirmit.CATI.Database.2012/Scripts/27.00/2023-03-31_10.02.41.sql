GO
PRINT N'Add Console.IncludeOpenEndReviewTimeInInterviewDuration system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%')
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Console.IncludeOpenEndReviewTimeInInterviewDuration', 'Enable including open end review time in interview duration', 'Console', 'This setting enables including open end review time in interview duration in all reports and data exports', 3, 0, 'False'

  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END


IF (@DbName like 'ConfirmitCATIV15[_]%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Console.IncludeOpenEndReviewTimeInInterviewDuration', 'Enable including open end review time in interview duration', 'Console', 'This setting enables including open end review time in interview duration in all reports and data exports', 3, 0, 'True'

  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END


GO
PRINT N'Update complete.';


GO
