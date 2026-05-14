PRINT N'Dropping [dbo].[BvCallHistory].[IX_BvCallHistory_BlockByFCD_SurveyId_InterviewId]...';


GO
DROP INDEX [IX_BvCallHistory_BlockByFCD_SurveyId_InterviewId]
    ON [dbo].[BvCallHistory];


GO
PRINT N'Dropping [dbo].[BvInterview].[BvIx_int_Batch]...';


GO
DROP INDEX [BvIx_int_Batch]
    ON [dbo].[BvInterview];


GO
PRINT N'Creating [dbo].[BvCallHistory].[IX_BvCallHistory_BlockByFCD_SurveyId_InterviewId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvCallHistory_BlockByFCD_SurveyId_InterviewId]
    ON [dbo].[BvCallHistory]([BlockedByFcd] ASC, [SurveyId] ASC, [InterviewID] ASC)
    INCLUDE([FiredTime]);


GO
PRINT N'Creating [dbo].[BvInterview].[BvIx_int_Batch]...';


GO
CREATE NONCLUSTERED INDEX [BvIx_int_Batch]
    ON [dbo].[BvInterview]([BatchID] ASC)
    INCLUDE([TransientState]);


GO
PRINT N'Altering [dbo].[GetCountsForSample]...';


GO
ALTER FUNCTION [dbo].[GetCountsForSample]
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
	END	 AS AttemptsPerComplete,
		 
	-- To avoid sorting  partition "PARTITION BY i.surveysid, i.id  ORDER BY i.id" we will use already used partition - diiferent i.id with h.id = nulls will be in one partition but statement below will work anyway
       CASE 
		WHEN LEAD(i.id, 1, 0) OVER(PARTITION BY h.SurveyId, h.InterviewId ORDER BY h.id) <> i.id  AND  i.TransientState = 17 --Blacklist
		THEN 1
		ELSE 0
	END	AS BlockedByBlacklist
     
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
	ISNULL(MAX(RecordsInBatch),0)					[InterviewsCurrent],
	ISNULL(SUM(attempted),0)					[InterviewsAttempted],
	ISNULL(SUM(blockedExcludedAttempted),0)				[BlockedExcludedAttemptedInterviews],
	ISNULL(SUM(blockedAttempted),0)					[BlockedAttemptedInterviews],
	ISNULL(SUM(AttemptedAfterBlocked),0)				[AttemptedAfterBlocked],
	ISNULL(SUM(completed),0)		AS			[InterviewsCompleted],
	ISNULL(CASE 
		WHEN SUM(completed) > 0 
		THEN
			CAST (SUM(attempted)*1.0/SUM(completed) AS REAL)
		ELSE 0
	END, 0)					AS			[AttemptedInterviewsPerComplete],
	ISNULL(CAST(ISNULL(AVG(NULLIF(AttemptsPerComplete,0)*1.0), 0) AS REAL), 0) 
					        AS			[AvgAttemptsPerComplete],
        SUM(BlockedByBlacklist)			AS			[BlockedByBlacklist]
from counts
)
GO
PRINT N'Refreshing [dbo].[BvSpSampleUtilisationReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleUtilisationReport]';


GO
PRINT N'Update complete.';


GO
