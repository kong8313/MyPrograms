GO
PRINT N'Altering [dbo].[BvSpReportInboundCalls]...';


GO
ALTER PROCEDURE [dbo].[BvSpReportInboundCalls]
@SurveySID INT, 
@ITSIDs NVARCHAR (1000),
@StartDateTime DATETIME,
@EndDateTime DATETIME
AS
	IF @SurveySID IS NULL AND @StartDateTime IS NULL AND @EndDateTime IS NULL AND @ITSIDs IS NULL
BEGIN
    SELECT 
       0 as HourInDay,
       0 as TotalCalls,
       0 as HandledCalls,
       0 as DroppedBySystem,
       0 as AbandonedByResp,
       0 as AvgWaitTimeForConnection,
       0 as AvgWaitTimeForAbandons,
       CAST(0 as DECIMAL) as AbandonRate,
       0 as DistinctAgents,
       0 as AvgCallDurationForConnected,
       0 as CompletesCount

    
    RETURN 0
END
	DECLARE @HoursStartTime DATETIME
	DECLARE @HoursEndTime DATETIME

	SET @HoursStartTime = DATEADD(hh, DATEDIFF(hh,0,@StartDateTime), 0)
	SET @HoursEndTime = @EndDateTime;
	CREATE TABLE #hours([Hour] int)
	;WITH DateRange([Hour], Date, Iteration ) AS 
	(
		SELECT DATEPART( HH, DATEADD(hh, DATEDIFF(hh,0,@StartDateTime), 0) ), DATEADD(hh, DATEDIFF(hh,0,@StartDateTime), 0), 0
		UNION ALL
		SELECT ([Hour] + 1 ) % 24, DATEADD(hh, 1, Date), Iteration + 1
		FROM DateRange 
		WHERE Iteration < 23 AND DATEADD(hh, 1, Date) < @EndDateTime
	)
	insert into #hours
	SELECT [Hour] FROM DateRange
	OPTION (MAXRECURSION 0); 

	CREATE TABLE #itses(its int primary key)
	insert into #itses
	SELECT Item
	FROM dbo.utilSplitNumbers( ISNULL(@ITSIDs, ''), ',')
	;WITH Data AS(
		SELECT	DATEPART(HOUR, StartTime ) as HourInDay, * FROM BvDialHistory
			WHERE Type = 1 /*Inbound*/ AND StartTime BETWEEN @StartDateTime AND @EndDateTime AND InitialSurveyId = @SurveySID
	),
	Completes as (
		SELECT	HourInDay, 
				COUNT( DISTINCT i.PersonSID ) as DistinctAgents,
				SUM( CASE WHEN cits.its IS NOT NULL THEN 1 ELSE 0 END ) as CompletesCount
			FROM Data d 
			INNER JOIN [dbo].[BvDialHistoryToInterviewHistory] d2i 
				ON d.ID = d2i.DialHistoryId 
			INNER JOIN BvHistory i 
				ON d2i.InterviewHistoryId = i.ID
			LEFT JOIN #itses cits ON cits.its = i.ITS
			GROUP BY HourInDay
	),
	Groups AS(
		SELECT  HourInDay,
				COUNT(*) as TotalCalls, 
				SUM(CASE WHEN AnswerTime IS NOT NULL THEN 1 ELSE 0 END) as HandledCalls,
				SUM(CASE WHEN AnswerTime IS NULL AND CallCompleteStatus <> 3/*CallCompleteStatus.DropByRespondent*/ THEN 1 ELSE 0 END) as DroppedBySystem,
				SUM(CASE WHEN AnswerTime IS NULL AND CallCompleteStatus = 3/*CallCompleteStatus.DropByRespondent*/ THEN 1 ELSE 0 END) as AbandonedByResp,
				SUM(CASE WHEN AnswerTime IS NOT NULL THEN DATEDIFF(SECOND, AnswerTime, FinishTime ) ELSE 0 END) as SumOfSpeakingTimesForCoonnected,
				SUM(CASE WHEN AnswerTime IS NOT NULL THEN DATEDIFF(SECOND, StartTime, AnswerTime ) ELSE 0 END) as SumOfWaitingTimesForCoonnected,
				SUM(CASE WHEN AnswerTime IS NOT NULL THEN 1 ELSE 0 END) as CountOfCoonnected,
				SUM(CASE WHEN AnswerTime IS NULL THEN DATEDIFF(SECOND, StartTime, FinishTime ) ELSE 0 END) as SumOfWaitingTimesForNotCoonnected,
				SUM(CASE WHEN AnswerTime IS NULL THEN 1 ELSE 0 END) as CountOfNotCoonnected
		FROM Data
		GROUP BY HourInDay
	)
	SELECT	#hours.[Hour] as HourInDay, 
			ISNULL(g.TotalCalls, 0) as TotalCalls,
			ISNULL(g.HandledCalls, 0) as HandledCalls,
			ISNULL(g.DroppedBySystem, 0) as DroppedBySystem,
			ISNULL(g.AbandonedByResp, 0) as AbandonedByResp,
			CASE WHEN g.CountOfCoonnected > 0 THEN g.SumOfWaitingTimesForCoonnected / g.CountOfCoonnected ELSE 0 END as AvgWaitTimeForConnection,
			CASE WHEN g.CountOfNotCoonnected > 0 THEN g.SumOfWaitingTimesForNotCoonnected / g.CountOfNotCoonnected ELSE 0 END as AvgWaitTimeForAbandons,
			CAST( CASE WHEN g.CountOfCoonnected + g.CountOfNotCoonnected > 0 THEN CAST( g.CountOfNotCoonnected AS FLOAT) / (g.CountOfCoonnected + g.CountOfNotCoonnected) * 100 ELSE 0 END AS NUMERIC(5,2)) as AbandonRate,
			ISNULL(c.DistinctAgents, 0) as DistinctAgents,
			CASE WHEN g.CountOfCoonnected > 0 THEN g.SumOfSpeakingTimesForCoonnected / g.CountOfCoonnected ELSE 0 END as AvgCallDurationForConnected,
			ISNULL(c.CompletesCount,0) as CompletesCount
		FROM Groups g
		LEFT JOIN Completes c ON g.HourInDay = c.HourInDay
		RIGHT JOIN #hours on #hours.[Hour] = g.HourInDay
		ORDER BY #hours.[Hour]
GO
PRINT N'Update complete.';


GO
