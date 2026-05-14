PRINT N'Altering [dbo].[BvSpNumberOfAttemptsReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpNumberOfAttemptsReport]
   @SurveySid INT,
   @StartDateTime DATETIME,
   @EndDateTime DATETIME,
   @TotalSampleSize INT OUTPUT
AS
	SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
	SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

   IF @SurveySid IS NULL AND @StartDateTime IS NULL AND @EndDateTime IS NULL AND @TotalSampleSize IS NULL
   BEGIN
      SELECT 0 as Attempts, 0 as Records, 0 as [SampleSize]
    
      RETURN 0
   END

   --1) should we check state here?
   --2) should we that time is necessary for sample here?
   SELECT @TotalSampleSize = COUNT(*)
   FROM BvInterview
   WHERE SurveySID = @SurveySid;
   
   CREATE TABLE #temp( Attempts INT, Records INT);

   WITH NotEmptyAttempts AS
   (
      SELECT COUNT(*) AS Attempts, 
             1 AS InterviewCount 
      FROM BvHistory h
      WHERE h.SurveyId = @SurveySid AND
            h.RoleID = 2 AND --don't calc sample calls
            h.FiredTime BETWEEN @StartDateTime AND @EndDateTime AND
			h.ITS <> 15 AND h.ITS <> 25 AND		-- 15 returned not dialed, 25-expired
            h.InterviewId IS NOT NULL
      GROUP BY h.InterviewId
   ),
   NotEmptyOutputList AS
   (
	   SELECT nea.Attempts AS Attempts,
			  COUNT(nea.InterviewCount) AS Records
	   FROM NotEmptyAttempts nea
	   GROUP BY nea.Attempts
   )
   INSERT INTO #temp
   SELECT neol.Attempts Attempts,
          neol.Records Records
   FROM NotEmptyOutputList neol;
   
   WITH AllAttempts AS
   (
      SELECT MAX(Attempts) AS Attempts
      FROM #temp
      
      UNION ALL
      
      SELECT Attempts-1
      FROM AllAttempts
      WHERE Attempts > 1
   )
   SELECT aa.Attempts,
          ISNULL(t.Records, 0) Records,
		  @TotalSampleSize as [SampleSize]
   FROM AllAttempts aa
   LEFT JOIN #temp t ON t.Attempts = aa.Attempts
   WHERE aa.Attempts IS NOT NULL
   ORDER BY aa.Attempts
   OPTION (MAXRECURSION 500)
GO
PRINT N'Update complete.';


GO
