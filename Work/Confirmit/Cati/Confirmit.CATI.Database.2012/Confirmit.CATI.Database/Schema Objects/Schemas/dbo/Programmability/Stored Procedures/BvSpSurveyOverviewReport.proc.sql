CREATE PROCEDURE [dbo].[BvSpSurveyOverviewReport]
 @SurveySids NVARCHAR (MAX),
 @PersonSids NVARCHAR(MAX),
 @CompletedItses NVARCHAR(MAX),
 @UseDialer BIT,
 @HideEmpty BIT,
 @StartDateTime DATETIME,
 @EndDateTime DATETIME,
 @SurveyDataFilter NVARCHAR(MAX),
 @StartShiftTime DATETIME,
 @EndShiftTime DATETIME,
 @IncludeOpenEndReviewTimeInInterviewDuration BIT = 1,
 @CallCenterId INTEGER = NULL

 
 WITH RECOMPILE
AS 

 if(@SurveySids is null and @PersonSids is null and @CompletedItses is null and @UseDialer is null and @HideEmpty is null and @StartDateTime is null and @EndDateTime is null)
 begin
    select  0 AS SurveyId,
		    '' AS ProjectId,
		    '' AS Title,
		    0 AS LogOnTime,
		    0 AS WaitingTime,
			0 AS OnBreakTimePaid,
			0 AS OnBreakTimeUnpaid,
		    0 AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
		    0 AS Completes,
		    0 AverageCompletedInterviewDuration
    return
 end

 if( @PersonSids is null)
 BEGIN
    exec BvSpSurveyOverviewReportForAllPersons @SurveySids, @CompletedItses, @UseDialer, @HideEmpty, @StartDateTime, @EndDateTime, @SurveyDataFilter, @StartShiftTime, @EndShiftTime, @IncludeOpenEndReviewTimeInInterviewDuration, @CallCenterId
	return
 END

 SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
 SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

 create table #CompletedItsList(CompletedIts  int primary key)
 insert into #CompletedItsList
 select * from dbo.utilSplitNumbers( ISNULL(@CompletedItses, ''), ',');

 create table #Surveys(SurveyId  int primary key, ProjectId NVARCHAR (255), Title NVARCHAR (255),  DurationPaid int, DurationUnpaid int)
 insert into #Surveys
 SELECT s.SID AS SurveyId ,
	    s.Name AS ProjectId,
	    s.Description AS Title,
		NULL,
		NULL
 FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',') ss
 INNER JOIN BvSurvey s ON s.SID = ss.Item

 create table #Persons(PersonSid int primary key)
 insert into #Persons
 SELECT p.SID AS PersonSid
  FROM dbo.utilSplitNumbers( ISNULL(@PersonSids, ''), ',') s
  INNER JOIN BvPerson p ON p.SID = s.Item
 UNION
 SELECT DialerSid AS PersonSid
 FROM (SELECT 0 AS DialerSid) dailerSids
 WHERE @UseDialer = 1
 

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

DECLARE @diff TIME

IF CAST(@StartShiftTime AS TIME) > CAST(@EndShiftTime AS TIME)
	select @diff = CAST('00:00:00' - (@StartShiftTime-@EndShiftTime) AS TIME )
else
	select @diff = CAST(@EndShiftTime-@StartShiftTime AS TIME)


 ;WITH TimeBreaksHistory AS
 (
    SELECT ISNULL(SUM(ISNULL(
	   	    	CASE WHEN CAST( '00:00:00' - (DATEADD ( SECOND, Duration, StartTime ) - @EndShiftTime)  AS TIME )  < @diff
				THEN
					Duration 
				ELSE
				     DATEDIFF(SECOND,  CAST(StartTime AS TIME), CAST ( @EndShiftTime AS TIME) )
			END, Duration)
	), 0) Duration, 
	SurveyId,
	ISNULL(bt.IsPaid, 1) as IsPaid
    FROM BvTimeBreaksHistory h
	LEFT JOIN #Persons p
	ON h.InterviewerId = p.PersonSid
	LEFT JOIN BvBreakType bt on bt.Id = h.BreakTypeId
    WHERE StartTime BETWEEN @StartDateTime AND @EndDateTime AND ( p.PersonSid IS NOT NULL )
	      AND (@StartShiftTime IS NULL OR CAST( StartTime - @StartShiftTime  AS TIME ) <= @diff)
    GROUP BY SurveyId, IsPaid
 ),
 AggregatedBreaksHistory AS (
	SELECT tbh.SurveyId,
	DurationPaid = sum(CASE WHEN tbh.IsPaid = 1 THEN tbh.Duration ELSE 0 END),
	DurationUnpaid = sum(CASE WHEN tbh.IsPaid = 0 THEN tbh.Duration ELSE 0 END)
	FROM TimeBreaksHistory tbh
	GROUP BY tbh.SurveyId
 )
 update #Surveys
 SET 
 DurationPaid = AggregatedBreaksHistory.DurationPaid,
 DurationUnpaid = AggregatedBreaksHistory.DurationUnpaid
 FROM AggregatedBreaksHistory
 WHERE #Surveys.SurveyId = AggregatedBreaksHistory.SurveyId


;WITH FilteredHistory AS 
(
	SELECT 
	FiredTime,
	ConfirmitDuration,
	WaitingTime,
	SurveyId,
	InterviewID,
	RoleID,
	PersonSID,
	ITS,
	Duration,
    IIF(@IncludeOpenEndReviewTimeInInterviewDuration = 0, ISNULL(OpenEndReviewDuration, 0), 0) as OpenEndReviewDurationForLogonTime
	FROM BvHistory 
	WHERE (@StartShiftTime IS NULL OR CAST( firedTime - @StartShiftTime  AS TIME ) <= @diff)
)
SELECT
  s.SurveyId AS SurveyId,
  s.ProjectId AS ProjectId,
  s.Title AS Title,
  
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0) + ISNULL(s.DurationPaid, 0) + 
  ISNULL(s.DurationUnpaid, 0)) + ISNULL(SUM(h.OpenEndReviewDurationForLogonTime), 0) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  ISNULL(s.DurationPaid, 0) AS OnBreakTimePaid,
  ISNULL(s.DurationUnpaid, 0) AS OnBreakTimeUnpaid,
  COUNT(h.InterviewID) AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
  ISNULL(SUM(cil.CompletedIts/cil.CompletedIts), 0) AS Completes,
  ISNULL(AVG(CASE WHEN cil.CompletedIts IS NOT NULL THEN h.Duration ELSE NULL END), 0) AS AverageCompletedInterviewDuration
  FROM #Surveys s 
  LEFT JOIN FilteredHistory h 
        ON s.SurveyId = h.SurveyId AND
           h.FiredTime >= @StartDateTime AND
           h.FiredTime <= @EndDateTime AND
           h.RoleID = 2 --we should not calced calls whuch were added during sample addition
        AND h.PersonSID IN (SELECT p.PersonSid FROM #Persons p)
    LEFT JOIN #CompletedItsList cil ON cil.CompletedIts = h.ITS
	LEFT JOIN #respids i on i.respid = h.InterviewId AND i.surveyid = h.SurveyId
	WHERE i.respid IS NOT NULL OR @SurveyDataFilter IS NULL
	GROUP BY s.SurveyId, s.ProjectId, s.Title, s.DurationPaid, s.DurationUnpaid
    HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0)
    