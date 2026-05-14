GO
PRINT N'Altering [dbo].[BvSpInterviewerProductivityReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterviewerProductivityReport]
 @SurveySids NVARCHAR (MAX),
 @PersonSids NVARCHAR(MAX),
 @CompletedItses NVARCHAR(MAX),
 @UseDialer BIT,
 @HideEmpty BIT,
 @CalcAllBreakHistory BIT,
 @StartDateTime DATETIME,
 @EndDateTime DATETIME, 
 @SurveyDataFilter NVARCHAR(MAX),
 @StartShiftTime DATETIME,
 @EndShiftTime DATETIME
 
 WITH RECOMPILE
AS 

 if(@SurveySids is null and @PersonSids is null and @CompletedItses is null and @UseDialer is null and @HideEmpty is null and @StartDateTime is null and @EndDateTime is null)
 begin
    select  0 AS PersonId,
		    '' AS PersonName,
		    0 AS LogOnTime,
			0 AS WaitingTime,
			0 AS OnBreakTimeUnpaid,
			0 AS OnBreakTimePaid,
		    0 AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
		    0 AS Completes,
		    0 AverageCompletedInterviewDuration,
			0 AS OpenEndReviewDuration
    return
 end

 DECLARE @DiallerName NVARCHAR(20)
 SET  @DiallerName = N'Dialer';
 
 SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
 SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

 CREATE TABLE #Persons(PersonSid int primary key, Name NVARCHAR (255), DurationPaid int, DurationUnpaid int)
 INSERT INTO #Persons
 SELECT p.SID AS PersonSid,
        p.Name AS Name,
		NULL,
		NULL
 FROM dbo.utilSplitNumbers( ISNULL(@PersonSids, ''), ',') s
 INNER JOIN BvPerson p ON p.SID = s.Item
 UNION 
 SELECT p.Sid AS PersonSid,
        p.Name AS Name,
		NULL,
		NULL
 FROM BvPerson p
 WHERE @PersonSids IS NULL
 UNION
 SELECT DialerSid AS PersonSid,
        @DiallerName AS Name,
		NULL,
		NULL
 FROM (SELECT 0 AS DialerSid) dailerSids
 WHERE @UseDialer = 1

 create table #CompletedItsList(CompletedIts  int primary key)
 insert into #CompletedItsList
 select * from dbo.utilSplitNumbers( ISNULL(@CompletedItses, ''), ',');

 create table #SurveyIdsList(SurveyId  int primary key)
 insert into #SurveyIdsList
 SELECT Item AS SurveyId 
 FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',')

 IF ( @CalcAllBreakHistory = 1 )
	INSERT INTO #SurveyIdsList VALUES(0)

DECLARE @diff TIME

IF CAST(@StartShiftTime AS TIME) > CAST(@EndShiftTime AS TIME)
	select @diff = CAST('00:00:00' - (@StartShiftTime-@EndShiftTime) AS TIME )
else
	select @diff = CAST(@EndShiftTime-@StartShiftTime AS TIME)


 ;WITH TimeBreaksHistory AS
 (
    SELECT ISNULL(SUM(ISNULL(
	   	    	CASE WHEN CAST( '00:00:00' - (DATEADD ( SECOND, Duration, StartTime ) - @endShiftTime)  AS TIME )  < @diff
				THEN
					Duration 
				ELSE
				     DATEDIFF(SECOND,  CAST(StartTime AS TIME), CAST ( @EndShiftTime AS TIME) )
			END, Duration)
	), 0) Duration, 
	InterviewerId,
	ISNULL(bt.IsPaid, 1) as IsPaid
    FROM BvTimeBreaksHistory h
	LEFT JOIN #SurveyIdsList s
	ON h.SurveyId = s.SurveyId
	LEFT JOIN BvBreakType bt on bt.Id = h.BreakTypeId
    WHERE StartTime BETWEEN @StartDateTime AND @EndDateTime AND ( s.SurveyId IS NOT NULL )
	      AND (@StartShiftTime IS NULL OR CAST( StartTime - @StartShiftTime  AS TIME ) <= @diff)
    GROUP BY InterviewerId, IsPaid
 ),
 AggregatedBreaksHistory AS (
	SELECT tbh.InterviewerId,
	DurationPaid = sum(CASE WHEN tbh.IsPaid = 1 THEN tbh.Duration ELSE 0 END),
	DurationUnpaid = sum(CASE WHEN tbh.IsPaid = 0 THEN tbh.Duration ELSE 0 END)
	FROM TimeBreaksHistory tbh
	GROUP BY tbh.InterviewerId
 )
 update #persons
 SET DurationPaid = AggregatedBreaksHistory.DurationPaid,
 DurationUnpaid = AggregatedBreaksHistory.DurationUnpaid
 FROM AggregatedBreaksHistory
 WHERE #persons.PersonSid = AggregatedBreaksHistory.InterviewerId

 CREATE TABLE #respids 
( 
	surveyid INT,
	respid int,
	PRIMARY KEY CLUSTERED 
	(
		[SurveyId] ASC,
		[respid] ASC
	)
)

IF (@SurveyDataFilter IS NOT NULL)
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	SET @sql = N'INSERT INTO #respids SELECT ' +  @SurveySids + ', respid from [dbo].[BvReplicatedData_' + @SurveySids + '] AS CFInterview WHERE ' + @SurveyDataFilter 
	EXEC (@sql)
END

;WITH FilteredHistory AS 
(
	SELECT * FROM BvHistory 
	WHERE (@StartShiftTime IS NULL OR CAST( firedTime - @StartShiftTime  AS TIME ) <= @diff)
)
 SELECT
  p.PersonSid AS PersonId,
  p.Name AS PersonName,
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0) + ISNULL(p.DurationPaid, 0) + ISNULL(p.DurationUnpaid, 0)) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  ISNULL(p.DurationPaid, 0) AS OnBreakTimePaid,
  ISNULL(p.DurationUnpaid, 0) AS OnBreakTimeUnpaid,
  COUNT(h.InterviewId) AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
  COUNT(cil.CompletedIts) AS Completes,
  ISNULL(AVG(CASE WHEN cil.CompletedIts IS NOT NULL THEN h.Duration ELSE NULL END), 0) AS AverageCompletedInterviewDuration,
  ISNULL(SUM(h.OpenEndReviewDuration), 0) AS OpenEndReviewDuration
 FROM #Persons p
 LEFT JOIN FilteredHistory h ON p.PersonSid = h.PersonSid AND
        h.FiredTime >= @StartDateTime AND
        h.FiredTime <= @EndDateTime AND
        h.RoleID = 2 AND --we should not calced calls which were added during sample addition
        h.SurveyId IN (SELECT sil.SurveyId FROM #SurveyIdsList sil)
 LEFT JOIN #CompletedItsList cil ON cil.CompletedIts = h.ITS
	 LEFT JOIN #respids i on i.respid = h.InterviewId AND i.surveyid = h.SurveyId
	 WHERE i.respid IS NOT NULL OR @SurveyDataFilter IS NULL

 GROUP BY p.PersonSid, p.Name, p.DurationPaid, p.DurationUnpaid
 HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0 OR p.PersonSid = 0)
GO
PRINT N'Update complete.';


GO
