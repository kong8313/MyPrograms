PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpSurveyOverviewReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurveyOverviewReport]
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
    select  0 AS SurveyId,
		    '' AS ProjectId,
		    '' AS Title,
		    0 AS LogOnTime,
		    0 AS WaitingTime,
		    0 AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
		    0 AS Completes,
		    0 AverageCompletedInterviewDuration
    return
 end

 if( @PersonSids is null)
 BEGIN
    exec BvSpSurveyOverviewReportForAllPersons @SurveySids, @CompletedItses, @UseDialer, @HideEmpty, @StartDateTime, @EndDateTime
	return
 END

 SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
 SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

 create table #CompletedItsList(CompletedIts  int primary key)
 insert into #CompletedItsList
 select * from dbo.utilSplitNumbers( ISNULL(@CompletedItses, ''), ',');

 create table #Surveys(SurveyId  int primary key, ProjectId NVARCHAR (255), Title NVARCHAR (255))
 insert into #Surveys
 SELECT s.SID AS SurveyId ,
	    s.Name AS ProjectId,
	    s.Description AS Title
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
 
 SELECT
  s.SurveyId AS SurveyId,
  s.ProjectId AS ProjectId,
  s.Title AS Title,
  
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  COUNT(h.InterviewID) AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
  ISNULL(SUM(cil.CompletedIts/cil.CompletedIts), 0) AS Completes,
  ISNULL(AVG(CASE WHEN cil.CompletedIts IS NOT NULL THEN h.Duration ELSE NULL END), 0) AS AverageCompletedInterviewDuration
  FROM #Surveys s 
    LEFT JOIN BvHistory h 
        ON s.SurveyId = h.SurveyId AND
           h.FiredTime >= @StartDateTime AND
           h.FiredTime <= @EndDateTime AND
           h.RoleID = 2 --we should not calced calls whuch were added during sample addition
        AND h.PersonSID IN (SELECT p.PersonSid FROM #Persons p)
    LEFT JOIN #CompletedItsList cil ON cil.CompletedIts = h.ITS
    GROUP BY s.SurveyId, s.ProjectId, s.Title
    HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0)
GO
PRINT N'Update complete.';


GO
