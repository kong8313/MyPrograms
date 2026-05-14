ALTER PROCEDURE [dbo].[BvSpInterviewerProductivityReport]
 @SurveySids NVARCHAR (MAX),
 @PersonSids NVARCHAR(MAX),
 @CompletedItses NVARCHAR(MAX),
 @UseDialer BIT,
 @HideEmpty BIT,
 @StartDateTime DATETIME,
 @EndDateTime DATETIME
 
 WITH RECOMPILE
AS 

 if(@SurveySids is null and @PersonSids is null and @CompletedItses is null and @UseDialer is null and @HideEmpty is null and @StartDateTime is null and @EndDateTime is null)
 begin
    select  0 AS PersonId,
		    '' AS PersonName,
		    0 AS LogOnTime,
			0 AS WaitingTime,
			0 AS OnBreakTime,
		    0 AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
		    0 AS Completes,
		    0 AverageCompletedInterviewDuration
    return
 end

 DECLARE @DiallerName NVARCHAR(20)
 SET  @DiallerName = N'Dialer';
 
 SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
 SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

 CREATE TABLE #Persons(PersonSid int primary key, Name NVARCHAR (255), Duration int)
 INSERT INTO #Persons
 SELECT p.SID AS PersonSid,
        p.Name AS Name,
		NULL
 FROM dbo.utilSplitNumbers( ISNULL(@PersonSids, ''), ',') s
 INNER JOIN BvPerson p ON p.SID = s.Item
 UNION 
 SELECT p.Sid AS PersonSid,
        p.Name AS Name,
		NULL
 FROM BvPerson p
 WHERE @PersonSids IS NULL OR @PersonSids = ''
 UNION
 SELECT DialerSid AS PersonSid,
        @DiallerName AS Name,
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

 ;WITH TimeBreaksHistory AS
 (
    SELECT ISNULL(SUM(Duration), 0) Duration, InterviewerId
    FROM BvTimeBreaksHistory
    WHERE StartTime >= @StartDateTime AND
          StartTime <= @EndDateTime
    GROUP BY InterviewerId
 )
 update #persons
 set duration = TimeBreaksHistory.Duration
 from TimeBreaksHistory
 where #persons.PersonSid = TimeBreaksHistory.interviewerId

 SELECT
  p.PersonSid AS PersonId,
  p.Name AS PersonName,
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0) + ISNULL(p.Duration, 0)) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  ISNULL(p.Duration, 0) AS OnBreakTime,
  COUNT(h.InterviewId) AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
  COUNT(cil.CompletedIts) AS Completes,
  ISNULL(AVG(CASE WHEN cil.CompletedIts IS NOT NULL THEN h.Duration ELSE NULL END), 0) AS AverageCompletedInterviewDuration
 FROM #Persons p
 LEFT JOIN BvHistory h ON p.PersonSid = h.PersonSid AND
        h.FiredTime >= @StartDateTime AND
        h.FiredTime <= @EndDateTime AND
        h.RoleID = 2 AND --we should not calced calls which were added during sample addition
        h.SurveyId IN (SELECT sil.SurveyId FROM #SurveyIdsList sil)
 LEFT JOIN #CompletedItsList cil ON cil.CompletedIts = h.ITS
 GROUP BY p.PersonSid, p.Name, p.Duration
 HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0 OR p.PersonSid = 0)

GO
PRINT N'Update complete.';


GO
