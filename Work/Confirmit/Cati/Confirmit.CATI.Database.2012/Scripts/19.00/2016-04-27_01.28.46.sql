PRINT N'Dropping [dbo].[BvCallHistory].[IX_BvCallHistorySurveyID_InterviewID]...';


GO
DROP INDEX [IX_BvCallHistorySurveyID_InterviewID]
    ON [dbo].[BvCallHistory];


GO
PRINT N'Altering [dbo].[BvCallHistory]...';


GO
ALTER TABLE [dbo].[BvCallHistory]
    ADD [BlockedByFcd] AS (CASE WHEN [OperationType] = (9) THEN CONVERT (BIT, (1)) WHEN [OperationType] = (11) THEN CONVERT (BIT, (1)) WHEN [OperationType] = (28) THEN CONVERT (BIT, (1)) WHEN [OperationType] = (29) THEN CONVERT (BIT, (1)) ELSE CONVERT (BIT, (0)) END) PERSISTED NOT NULL;


GO
PRINT N'Creating [dbo].[BvCallHistory].[IX_BvCallHistory_BlockByFCD_SurveyId_InterviewId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvCallHistory_BlockByFCD_SurveyId_InterviewId]
    ON [dbo].[BvCallHistory]([BlockedByFcd] ASC, [SurveyId] ASC, [InterviewID] ASC);


GO
PRINT N'Creating [dbo].[BvCallHistory].[IX_BvCallHistorySurveyID_InterviewID_i_FiredTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvCallHistorySurveyID_InterviewID_i_FiredTime]
    ON [dbo].[BvCallHistory]([SurveyId] ASC, [InterviewID] ASC)
    INCLUDE([FiredTime]);


GO
PRINT N'Creating [dbo].[GetCountsForSample]...';


GO
CREATE FUNCTION [dbo].[GetCountsForSample]
(
	@BatchId int,
	@Its varchar(max)
)
RETURNS TABLE
AS
RETURN
(
WITH counts AS
(
SELECT  
	DENSE_RANK() OVER (ORDER BY i.SurveySId, i.Id) as RecordsInBatch,
	
	CASE
		WHEN h.InterviewId IS NOT NULL AND LEAD(h.InterviewId, 1, 0) OVER (PARTITION BY h.SurveyId, h.InterviewId ORDER BY h.id) <> h.InterviewId 
		THEN 1
		ELSE 0 
	END AS attempted,

	CASE 
		WHEN 
			LEAD(ch.Interviewid, 1, 0 ) OVER (PARTITION BY ch.SurveyId, ch.InterviewId ORDER BY ch.id, h.id) <> ch.InterviewId AND h.InterviewId IS NULL
		THEN 1 
		ELSE 0
	END AS blockedExcludedAttempted,

	CASE 
		WHEN  LEAD(ch.Interviewid, 1, 0 ) OVER (PARTITION BY ch.SurveyId, ch.InterviewId ORDER BY ch.id, h.id) <> ch.InterviewId AND ch.FiredTime > h.FiredTime
		THEN 1 
		ELSE 0
	END AS blockedAttempted,

	CASE 
		WHEN  LEAD(ch.Interviewid, 1, 0 ) OVER (PARTITION BY ch.SurveyId, ch.InterviewId ORDER BY ch.id, h.id) <> ch.InterviewId AND ch.FiredTime < h.FiredTime
		THEN 1 
		ELSE 0
	END AS AttemptedAfterBlocked,


	CASE 
		WHEN its.item IS NOT NULL AND LEAD(h.InterviewId, 1, 0) OVER (PARTITION BY h.SurveyId, h.InterviewId ORDER BY h.id) <> h.InterviewId 
		THEN 1
		ELSE 0
	END AS Completed,

	CASE
		WHEN its.item is not null AND LEAD(h.InterviewId, 1, 0) OVER (PARTITION BY h.SurveyId, h.InterviewId ORDER BY h.id) <> h.InterviewId
			THEN count(h.interviewID) OVER(PARTITION BY h.SurveyId, h.InterviewId ORDER BY h.id) ELSE 0
	END	 AS AttemptsPerComplete
		 
FROM BvInterview i
LEFT JOIN BvHistory h
	ON i.id = h.InterviewId AND i.SurveySID = h.SurveyId
LEFT JOIN dbo.utilSplitNumbers(@its, ',') its
	ON h.ITS = its.item
LEFT JOIN BvCallHistory ch	
	ON i.ID = ch.InterviewID AND i.SurveySID = ch.SurveyId AND BlockedByFcd = 1
WHERE i.batchid = @BatchId AND ISNULL(h.RoleID, 2) = 2
)
select 
	MAX(RecordsInBatch)						[InterviewsCurrent],
	SUM(attempted)							[InterviewsAttempted],
	SUM(blockedExcludedAttempted)					[BlockedExcludedAttemptedInterviews],
	SUM(blockedAttempted)						[BlockedAttemptedInterviews],
	SUM(AttemptedAfterBlocked)					[AttemptedAfterBlocked],
	SUM(completed) AS						[InterviewsCompleted],
		CASE 
		WHEN SUM(completed) > 0 
		THEN
			CAST (SUM(attempted)*1.0/SUM(completed) AS REAL)
		ELSE 0
	END									 AS	[AttemptedInterviewsPerComplete],
	CAST(ISNULL(AVG(NULLIF(AttemptsPerComplete,0)*1.0), 0) AS REAL) 
									AS		[AvgAttemptsPerComplete]
from counts
)
GO
PRINT N'Creating [dbo].[BvSpSampleUtilisationReport]...';


GO
CREATE PROCEDURE [dbo].[BvSpSampleUtilisationReport]
(
 @SurveySid INT,
 @CompletedItses NVARCHAR(MAX),
 @StartDateTime DATETIME,
 @EndDateTime DATETIME
)
AS
BEGIN

SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

SELECT  
	s.BatchId		[Batchid], 
	ss.Name			[SurveyName], 
	s.FinishedTime		[BatchAddedAt],
	s.CountInterviews 	[InterviewsAdded],
	counts.*,
	s.CountInterviews - counts.[InterviewsCurrent] as [InterviewsDeleted]
 FROM bvsamples s 
 JOIN bvsurvey ss
	on s.SurveySID = ss.SID
	CROSS APPLY dbo.GetCountsForSample(s.batchid, @CompletedItses) counts

WHERE s.State = 2 AND s.SurveySID = @SurveySid AND s.StartedTime >= @StartDateTime AND s.FinishedTime <= @EndDateTime
ORDER BY s.FinishedTime
END
GO
PRINT N'Refreshing [dbo].[BvSpCall_MoveToITS]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_MoveToITS]';


GO
PRINT N'Refreshing [dbo].[BvSpGetExtendedCallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetExtendedCallHistory]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Clean]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Clean]';


GO
PRINT N'Update complete.';


GO
