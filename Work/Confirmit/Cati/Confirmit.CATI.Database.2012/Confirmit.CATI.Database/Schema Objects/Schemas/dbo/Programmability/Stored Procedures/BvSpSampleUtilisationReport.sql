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

    WHERE s.State = 2 AND s.SampleType = 0 AND s.SurveySID = @SurveySid AND s.StartedTime >= @StartDateTime AND s.FinishedTime <= @EndDateTime
    ORDER BY s.FinishedTime
END