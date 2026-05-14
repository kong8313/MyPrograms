GO
DROP TABLE [dbo].[BvInterviewerPerformance];

CREATE TABLE [dbo].[BvInterviewerPerformance] (
    [InterviewerId]            INT            NOT NULL,
    [InterviewerName]          NVARCHAR (255) NOT NULL,
    [SurveyId]                 INT            NOT NULL,
    [TotalInterviewCount]      INT            NOT NULL,
    [CompletedInterviewCount]  INT            NOT NULL,
    [CompletedInLastHourCount] INT            NOT NULL,
    [InterviewingTime]         INT            NOT NULL,
    CONSTRAINT [PK_BvInterviewerPerformance_InterviewerId_SurveyId] PRIMARY KEY CLUSTERED ([InterviewerId] ASC, [SurveyId] ASC)
);

GO
ALTER PROCEDURE [dbo].[BvSpAggregateInterviewerPerformance]

 @StartDateTime DATETIME,
 @CompletedItses NVARCHAR(MAX) 
 
AS
 
Declare  @DateMinusOneHourTime DATETIME;
Set @DateMinusOneHourTime  = DATEADD(Hour,-1, GETUTCDATE());

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

GO
ALTER PROCEDURE [dbo].[BvSpGetInterviewerPerformanceList] 
 @CallCenterId INT,
 @onlyLoggedIn bit,
 @bySurveys bit
AS 

IF(@onlyLoggedIn = 0)	
	BEGIN
		IF(@bySurveys = 0)
			SELECT InterviewerId, 
				   InterviewerName,
				   '' AS ProjectID,
				   '' AS ProjectName,
				   SUM(InterviewingTime) AS InterviewingTime,
				   SUM(TotalInterviewCount) AS TotalInterviewCount, 
				   SUM(CompletedInterviewCount) AS CompletedInterviewCount,
				   SUM(CompletedInLastHourCount) AS  CompletedInLastHourCount
			FROM BvInterviewerPerformance ip INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
			GROUP BY InterviewerId, InterviewerName
		ELSE
			SELECT InterviewerId, 
				   InterviewerName,
				   s.Name AS ProjectID,
				   s.[Description] AS ProjectName,
				   InterviewingTime,
				   TotalInterviewCount, 
				   CompletedInterviewCount,
				   CompletedInLastHourCount 
			FROM BvInterviewerPerformance ip INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
											 INNER JOIN BvSurvey s ON ip.SurveyId = s.[SID]
	END
ELSE
	BEGIN
		IF(@bySurveys = 0)
			SELECT InterviewerId, 
				   InterviewerName,
				   '' AS ProjectID,
				   '' AS ProjectName,
				   SUM(InterviewingTime) AS InterviewingTime,
				   SUM(TotalInterviewCount) AS TotalInterviewCount, 
				   SUM(CompletedInterviewCount) AS CompletedInterviewCount,
				   SUM(CompletedInLastHourCount) AS  CompletedInLastHourCount
			FROM BvTasks INNER JOIN BvInterviewerPerformance ip ON BvTasks.PersonSID = ip.[InterviewerId]
						 INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
			GROUP BY InterviewerId, InterviewerName
		ELSE
			SELECT InterviewerId, 
				   InterviewerName,
				   s.Name AS ProjectID,
				   s.[Description] AS ProjectName,
				   InterviewingTime,
				   TotalInterviewCount, 
				   CompletedInterviewCount,
				   CompletedInLastHourCount 
			FROM BvTasks INNER JOIN BvInterviewerPerformance ip ON BvTasks.PersonSID = ip.[InterviewerId]
						 INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
						 INNER JOIN BvSurvey s ON ip.SurveyId = s.[SID]
	END
GO
