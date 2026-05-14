PRINT N'Altering [dbo].[BvSpAlertsHistoryReport]...';

GO

ALTER PROCEDURE BvSpAlertsHistoryReport
	@personIds NVARCHAR(MAX),
	@surveyIds NVARCHAR(MAX),
	@SearchCondition NVARCHAR(MAX),
	@PageIndex INT,
	@PageSize INT,
	@OrderField NVARCHAR(64),
	@IsOrderASC BIT
 AS
 
	IF @personIds IS NULL AND @surveyIds IS NULL AND @PageIndex IS NULL AND @PageSize IS NULL
	BEGIN
	/* Looks like we're generating code using FMTONLY. So lets return metadata*/
	SELECT  0 AS PersonId,
			'' AS PersonName,
			0 AS SurveyId,
			'' AS ProjectId,
			'' AS SurveyName,
			0 AlertType,
			cast(0 as bit) Alert,
			0 AS AnswerDuration,
			'' AS QuestionId,
			CAST(NULL AS DATETIME) AS SubmissionTime,
			0 AS InterviewId,
			CAST(0 AS TINYINT) AS InterviewState
     WHERE 1 = 0
	 RETURN 0;
	END
 
    DECLARE @query NVARCHAR(MAX) = '
    SELECT p.Sid AS PersonId,
           p.Name AS PersonName,
           s.SID AS SurveyId,
           s.Name AS ProjectId,
           s.Description AS SurveyName,
           (CASE WHEN h.AnswerSubmissionAlert IS NULL THEN 2 ELSE 1 END) AlertType,
           (CASE WHEN h.AnswerSubmissionAlert IS NULL THEN h.QuickAnswerSubmissionAlert ELSE h.AnswerSubmissionAlert END) Alert,
           h.AnswerDuration,
           h.QuestionId,
           h.SubmissionTime,
           h.InterviewId,
           h.InterviewState
    FROM BvAnswerSubmissionAlertHistory h
    LEFT JOIN dbo.utilSplitNumbers( ''' + ISNULL(@PersonIds, '') + ''', '','') s1 ON s1.Item = h.PersonId
    INNER JOIN BvPerson p ON p.Sid = h.PersonId
    INNER JOIN dbo.utilSplitNumbers( ''' + ISNULL(@SurveyIds, '') + ''', '','') s2 ON s2.Item = h.SurveyId
    INNER JOIN BvSurvey s ON s.SID = h.SurveyId
    WHERE '''' = ''' + ISNULL(@PersonIds, '') + ''' OR s1.Item IS NOT NULL'

    DECLARE @TotalCount INT

    EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @OrderField, @SearchCondition
    RETURN @TotalCount

GO

PRINT N'Update complete.';