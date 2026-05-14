PRINT N'Add Setup.InterviewerAPIVersion and Setup.BBCCVersion settings';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
    SELECT 'Setup.InterviewerAPIVersion', 'CATI Interviewer API version', 'Setup', 'Version of the CATI Interviewer API', 2, 0, ''
    UNION ALL
    SELECT 'Setup.BBCCVersion', 'Browser Based CATI Console version', 'Setup', 'Version of the Browser Based CATI Console', 2, 0, ''
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END
GO

PRINT N'Update complete.';
GO