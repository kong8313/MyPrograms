PRINT N'Creating [dbo].[BvSpQuotaProgressReport]...';


GO


/*
Example of final query :

;WITH HistoryRecordsByCellId as 
(
 SELECT
    t.quotaid as CellId,
    t.q1,
    t.q2,
    t.q1+','+t.q2 AS CellValues,
	DATEDIFF(d,'2017-01-30 00:00:00', h.firedtime) as [Day],
	InterviewId,
	Item as ITS
	FROM [##quota_4673F1C2-0F56-4C24-BE52-6279C36356E8] t
	LEFT JOIN [BvReplicatedData_83] r
		ON t.q1=r.q1 AND t.q2=r.q2
	LEFT JOIN BvHistory h
		ON h.SurveyId =83 AND r.respid = h.InterviewId and h.FiredTime between '2017-01-30 00:00:00' AND '2017-02-07 00:00:00'
	 LEFT JOIN dbo.utilSplitNumbers('13', ',') s
			ON h.ITS = s.Item
)
SELECT   
	t.CellValues AS [Quota cells],
	COUNT(case when [Day] = 0 then t.ITS END) as Day1,
	COUNT(case when [Day] = 1 then t.ITS END) as Day2,
	COUNT(case when [Day] = 2 then t.ITS END) as Day3,
	COUNT(case when [Day] = 3 then t.ITS END) as Day4,
	COUNT(case when [Day] = 4 then t.ITS END) as Day5,
	COUNT(case when [Day] = 5 then t.ITS END) as Day6,
	COUNT(case when [Day] = 6 then t.ITS END) as Day7,
	COUNT(t.ITS)/7.0 [Avg7days],
	COUNT(case when [Day] = 7 then t.ITS END) as Day8 
FROM HistoryRecordsByCellId t
GROUP BY t.q1,t.q2,t.CellId, t.CellValues
ORDER BY t.CellId
*/
CREATE PROCEDURE [dbo].[BvSpQuotaProgressReport]
@SurveyId INT,
@ITSIDs	NVARCHAR(MAX), 
@QuotaName NVARCHAR(256),
@TargetDate DateTime,	--in UTC
@CfDbSchemaPath NVARCHAR(MAX)
AS 
BEGIN

DECLARE @sql NVARCHAR(MAX)
DECLARE @tmpTable NVARCHAR(MAX) = '[##quota_' + CAST(NEWID() AS nvarchar(MAX)) + ']'
DECLARE @quotaFieldsStr NVARCHAR(MAX)
DECLARE @CellValues NVARCHAR(MAX)
DECLARE @Join NVARCHAR(MAX)
DECLARE @tablename NVARCHAR(max)  
DECLARE @startColumnIdForQuotaVars INT = 7

-- retrieve a quota table name from survey DB
-- Example : SELECT @tableNameOUT = tablename from survey_p1000268.dbo.quotas where quotaname ='quota2'
SET @sql = N'SELECT @tableNameOUT = tablename from ' + @CfDbSchemaPath + 'dbo.quotas where quotaname =''' + @QuotaName + ''''
EXEC sp_executesql @sql, N'@tableNameOUT nvarchar(max) OUTPUT', @tableNameOUT=@tablename OUTPUT;

-- copy content of quota table from survey DB to CATI DB
-- Example : SELECT * INTO [##quota_4673F1C2-0F56-4C24-BE52-6279C36356E8] FROM survey_p1000268.dbo.quota_2
SET @sql = N'SELECT * INTO ' + @tmpTable + ' FROM ' + @CfDbSchemaPath + 'dbo.' + @tablename
EXEC sp_executesql @sql


SELECT 
@startColumnIdForQuotaVars = 8
FROM tempdb.sys.columns WHERE object_id = OBJECT_ID('tempdb..' + @tmpTable) AND name = 'disabled'

--we just created quota table in CATI DB so column ids should be sequential (we are interested in last columns of quota table)
SELECT 
	@quotaFieldsStr = ISNULL(@quotaFieldsStr  + ',','') + 't.' + name,			--for example getting t.q1,t.q2 for a quota based on q1 and q2
 	@CellValues = ISNULL(@CellValues + '+'',''+', '') + 't.' + name,			--for quota cells we get : t.q1+','+t.q2
	@Join =ISNULL(@Join + ' AND ', '') + 't.' + name + '=r.' + name                     	--for join we need t.q1 = r.q1 and t.q2 = r.q2
FROM tempdb.sys.columns WHERE object_id = OBJECT_ID('tempdb..' + @tmpTable) AND column_id > @startColumnIdForQuotaVars

DECLARE @StartDate DATETIME = DATEADD(d, -7, @TargetDate)

SET @sql = N'
;WITH HistoryRecordsByCellId as 
(
 SELECT
    t.quotaid as CellId,' 
	+ @quotaFieldsStr + ',' +
	+ @CellValues + ' AS CellValues,
	DATEDIFF(d,''' +  CONVERT(NVARCHAR(MAX), @StartDate, 20) + ''', h.firedtime) as [Day],
	InterviewId,
	Item as ITS
	FROM ' + @tmpTable + ' t
	LEFT JOIN [BvReplicatedData_' + CAST(@SurveyId AS nvarchar(max)) + '] r
		ON ' + @Join + '
	LEFT JOIN BvHistory h
		ON h.SurveyId =' + CAST(@SurveyId AS nvarchar(max)) + ' AND r.respid = h.InterviewId and h.FiredTime between ''' + CONVERT(NVARCHAR(MAX), @StartDate, 20) + ''' AND ''' + CONVERT(NVARCHAR(MAX), @TargetDate, 20) + '''
	 LEFT JOIN dbo.utilSplitNumbers(''' + @ITSIDs + ''', '','') s
			ON h.ITS = s.Item
)
SELECT   
	t.CellValues AS					[_column0],  
	COUNT(case when [Day] = 0 then t.ITS END) as		[_column1],
	COUNT(case when [Day] = 1 then t.ITS END) as		[_column2],
	COUNT(case when [Day] = 2 then t.ITS END) as		[_column3],
	COUNT(case when [Day] = 3 then t.ITS END) as		[_column4],
	COUNT(case when [Day] = 4 then t.ITS END) as		[_column5],
	COUNT(case when [Day] = 5 then t.ITS END) as		[_column6],
	COUNT(case when [Day] = 6 then t.ITS END) as		[_column7],
	COUNT(case when [Day] < 7 then t.ITS END)/7.0  as	[_column8],
	COUNT(case when [Day] = 7 then t.ITS END) as		[_column9] 
FROM HistoryRecordsByCellId t
GROUP BY ' + @quotaFieldsStr + ',t.CellId, t.CellValues
ORDER BY t.CellId'

exec sp_executesql @sql

SET @sql = N'DROP TABLE ' + @tmpTable
exec sp_executesql @sql

END


GO
PRINT N'Update complete.';


GO
