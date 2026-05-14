CREATE PROCEDURE [dbo].[BvSpAggregateInterviewerPerformance]

 @StartDateTime DATETIME,
 @CompletedItses NVARCHAR(MAX) 
 
AS
 
Declare  @DateMinusOneHourTime DATETIME;
Set @DateMinusOneHourTime  = DATEADD(Hour,-1, dbo.GetUtcNow());

DELETE FROM BvInterviewerPerformance;
 
WITH Persons AS
	(
	SELECT 	
		p.SID AS PersonSid,
		p.Name as PersonName
		FROM BvPerson p 	  	  	
	),
	CompletedItsList AS
	(
	SELECT Item AS CompletedIts 
	FROM dbo.utilSplitNumbers( ISNULL(@CompletedItses, ''), ',')
	)
	INSERT INTO BvInterviewerPerformance(
	[InterviewerId],
	[InterviewerName],
	[SurveyId],
	[InterviewingTime],
	[TotalInterviewCount],
	[CompletedInterviewCount],
	[CompletedInLastHourCount]
	)
	SELECT 
	p.PersonSid AS InterviewerId,
	p.PersonName AS InterviewerName,
	h.SurveyId,
	(ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) AS InterviewingTime,  
	COUNT(h.ITS) AS TotalInterviewCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)  
	ISNULL(SUM(CASE WHEN cil.CompletedIts IS NOT NULL  THEN 1 ELSE 0 END), 0) AS CompletedInterviewCount,  
	ISNULL(SUM(CASE WHEN h.FiredTime >= @DateMinusOneHourTime and cil.CompletedIts IS NOT NULL THEN 1 ELSE 0 END), 0) AS CompletedInLastHourCount      
	FROM Persons p 
	INNER JOIN BvHistory h ON p.PersonSid = h.PersonSid AND
		h.FiredTime >= @StartDateTime AND
		h.RoleID = 2  --we should not calculate calls which were added during sample addition                          
	LEFT JOIN CompletedItsList cil ON cil.CompletedIts = h.ITS
	GROUP BY p.PersonSid, p.PersonName, h.SurveyId