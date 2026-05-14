GO
PRINT N'update Console.IncludeOpenEndReviewTimeInInterviewDuration';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName like 'ConfirmitCATIV15%' OR @DbName like 'ConfirmitCATIV15TEST%')
BEGIN
   UPDATE BvSystemSettings SET [Description] = 'This setting enables including open end review time in interview duration in all reports and data exports, for all interviews completed after this setting is enabled.'
   WHERE [SystemName] = 'Console.IncludeOpenEndReviewTimeInInterviewDuration'
  	
END

GO
PRINT N'Update complete.';


GO
