
/*
WITH Statuses AS
(
	SELECT ISNULL(q1,0) AS q1, i.TransientState as ITS, sg.Name, 1 AS [Cnt]
	FROM BvInterview i 
	Join BvReplicatedData_53 r
		ON i.ID = r.respid
	JOIN BvSurvey s
		ON i.SurveySID = s.SID
	JOIN BvState sg
		ON sg.StateGroupID = s.StateGroupID AND sg.StateID = i.TransientState
	WHERE surveySid=53
)
SELECT 
	[Name]											AS [_column0],
	ISNULL(COUNT(ITS), 0)							AS [_column1],
	''												AS [_column2], 
	ISNULL(COUNT(CASE WHEN q1 = 0 THEN q1 END), 0)	AS [_column3],
	ISNULL(COUNT(CASE WHEN q1 = 1 THEN q1 END), 0)	AS [_column4],
	ISNULL(COUNT(CASE WHEN q1 = 2 THEN q1 END), 0)	AS [_column5],
	ISNULL(COUNT(CASE WHEN q1 = 3 THEN q1 END), 0)	AS [_column6]
FROM Statuses
GROUP BY ITS, Name
ORDER BY ITS

With scheduled calls 


;WITH Statuses AS
(
SELECT ISNULL(q1,0) AS q1, i.TransientState as ITS, sg.Name, c.ID as CallID
	FROM BvInterview i 
	Join BvReplicatedData_53 r
		ON i.ID = r.respid
	JOIN BvSurvey s
		ON i.SurveySID = s.SID
	JOIN BvState sg
		ON sg.StateGroupID = s.StateGroupID AND sg.StateID = i.TransientState
	LEFT JOIN BvSvySchedule c
		ON i.Id = c.InterviewID and i.SurveySID =c.SurveySID
	WHERE i.surveySid=53
)
SELECT 
	[Name] AS Status											AS [_column0],
	CAST(ISNULL(COUNT(ITS), 0) AS VARCHAR(MAX)) + ' (' + 
		CAST(ISNULL(COUNT(CallId),0) AS VARCHAR(MAX)) + ')'		AS [_column1], 
	''															AS [_column2], 
	CAST( ISNULL(COUNT(CASE WHEN q1 = 0 THEN q1 END), 0) AS VARCHAR(MAX)) + ' (' +
	CAST( ISNULL(COUNT(CASE WHEN q1 = 0 AND CallId IS NOT NULL THEN q1 END), 0) AS VARCHAR(MAX) ) + ')' AS [_column3],
	CAST( ISNULL(COUNT(CASE WHEN q1 = 1 THEN q1 END), 0) AS VARCHAR(MAX)) + ' (' +
	CAST( ISNULL(COUNT(CASE WHEN q1 = 1 AND CallId IS NOT NULL THEN q1 END), 0) AS VARCHAR(MAX) ) + ')' AS [_column4],
	CAST( ISNULL(COUNT(CASE WHEN q1 = 2 THEN q1 END), 0) AS VARCHAR(MAX)) + ' (' +
	CAST( ISNULL(COUNT(CASE WHEN q1 = 2 AND CallId IS NOT NULL THEN q1 END), 0) AS VARCHAR(MAX) ) + ')' AS [_column5],
	CAST( ISNULL(COUNT(CASE WHEN q1 = 3 THEN q1 END), 0) AS VARCHAR(MAX)) + ' (' +
	CAST( ISNULL(COUNT(CASE WHEN q1 = 4 AND CallId IS NOT NULL THEN q1 END), 0) AS VARCHAR(MAX) ) + ')' AS [_column6]
FROM Statuses
GROUP BY [Name], [ITS]
ORDER BY ITS



*/

CREATE PROCEDURE [dbo].[BvSpSampleStatusSummaryByQuestionReport]
@SurveyID       INTEGER,
@ITSIDs			VARCHAR(MAX),
@QuestionId		VARCHAR(MAX),
@Precodes		VARCHAR(MAX),
@AnswerTexts	VARCHAR(MAX),
@ShowScheduled  BIT
AS
SET NOCOUNT OFF
BEGIN

	DECLARE @sqlQuery	NVARCHAR(MAX)
	DECLARE @selectList NVARCHAR(MAX) = ''
	DECLARE @Question NVARCHAR(MAX) = ''


	IF (@QuestionId IS NOT NULL)
		SET @Question = 'r.' + QUOTENAME(@QuestionId) + ' AS __columnName,'

	IF (@ShowScheduled = 1) 
		BEGIN 
			SET @sqlQuery = ';WITH Statuses AS
				(
					SELECT ' + @Question + ' i.TransientState as ITS, sg.Name, c.ID as CallID
					FROM BvInterview i 
					Join BvReplicatedData_' + CAST(@SurveyID AS VARCHAR(MAX)) + ' r
						ON i.ID = r.respid
					JOIN BvSurvey s
						ON i.SurveySID = s.SID
					JOIN BvState sg
						ON sg.StateGroupID = s.StateGroupID AND sg.StateID = i.TransientState
					LEFT JOIN BvSvySchedule c
						ON i.Id = c.InterviewID and i.SurveySID =c.SurveySID
					WHERE i.surveySid=' + CAST(@SurveyID AS VARCHAR(MAX)) + '
				)
				SELECT 
					[Name]																				AS [_column0],
					CAST(ISNULL(COUNT(ITS), 0) AS VARCHAR(MAX)) + ' + 
						' '' (''' + ' + CAST(ISNULL(COUNT(CallId),0) AS VARCHAR(MAX))' + ' + '')''' + ' AS [_column1],
					''''																				AS [_column2]'


            if (@QuestionId IS NOT NULL)
			BEGIN
				SET @sqlQuery = @sqlQuery + ', 
					CAST( ISNULL(COUNT(CASE WHEN __columnName IS NULL THEN 0 END), 0) AS VARCHAR(MAX)) + ' + ''' (''' +
					' + CAST( ISNULL(COUNT(CASE WHEN __columnName IS NULL AND CallId IS NOT NULL THEN 0 END), 0) AS VARCHAR(MAX) ) + ' + ''')''' + ' AS [_column3]
					'

 				SELECT @selectList = ISNULL(@selectList + ',','') + 'CAST(ISNULL(COUNT(CASE WHEN __columnName=' + Precode + ' THEN __columnName END), 0) AS VARCHAR(MAX)) + ' + ''' (''' + 
						  ' + CAST(ISNULL(COUNT(CASE WHEN __columnName=' + Precode + ' AND CallId IS NOT NULL THEN __columnName END), 0) AS VARCHAR(MAX)) + ' + ''')''' + ' AS ' + QUOTENAME(AnswerText) + '
				'
				FROM  (SELECT p.Item AS Precode, a.Item AS AnswerText FROM dbo.utilSplitStringWithOrderId(@Precodes, ',' ) p
					JOIN  dbo.utilSplitStringWithOrderId(@AnswerTexts, ',' ) a
					ON p.OrderID =a.OrderId
				  ) AS t
			END
		END
	ELSE
		BEGIN
			SET @sqlQuery = ';WITH Statuses AS
			(
				SELECT ' + @Question + ' sg.Name, i.TransientState AS [ITS]
				FROM BvInterview i 
				JOIN BvReplicatedData_' + CAST(@SurveyID AS VARCHAR(MAX)) + ' r
					ON i.ID = r.respid
				JOIN BvSurvey s
					ON i.SurveySID = s.SID
				JOIN BvState sg
					ON sg.StateGroupID = s.StateGroupID AND sg.StateID = i.TransientState
				WHERE surveySid=' + CAST(@SurveyID AS VARCHAR(MAX)) + '
			)
			SELECT 
				[Name]										AS [_column0],
				ISNULL(COUNT(ITS), 0)						AS [_column1],
				''''										AS [_column2]'

            if (@QuestionId IS NOT NULL)
			BEGIN
				
				SET @sqlQuery = @sqlQuery + ',
				ISNULL(COUNT(CASE WHEN __columnName IS NULL THEN 0 END), 0)	AS [_column3]
			'

				SELECT @selectList = ISNULL(@selectList + ',','') + 'ISNULL(COUNT(CASE WHEN __columnName=' + Precode + ' THEN __columnName END), 0) AS ' + QUOTENAME(AnswerText) + '
				'
				FROM  (SELECT p.Item AS Precode, a.Item AS AnswerText FROM dbo.utilSplitStringWithOrderId(@Precodes, ',' ) p
					JOIN  dbo.utilSplitStringWithOrderId(@AnswerTexts, ',' ) a
					ON p.OrderID =a.OrderId
				  ) AS t
			END
		END


	SET @sqlQuery = @sqlQuery + @selectList + ' 
	FROM Statuses
	'
	IF (@ITSIDs IS NOT NULL)
		SET @sqlQuery = @sqlQuery + '
		JOIN dbo.utilSplitNumbers(''' + @ITSIDs + ''', '','') s
			ON ITS = s.Item
		'
	SET @sqlQuery = @sqlQuery + '
	GROUP BY ITS, Name
	ORDER BY ITS' 

	EXEC sp_executesql @sqlQuery

END
