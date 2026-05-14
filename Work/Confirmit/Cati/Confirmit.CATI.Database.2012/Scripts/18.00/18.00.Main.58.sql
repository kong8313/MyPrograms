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
   SELECT @TotalSampleSize = SUM(CountInterviews)
   FROM BvSamples
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


PRINT N'Altering [dbo].[BvSpReportSampleStatusSummary]...';


GO
ALTER PROCEDURE [dbo].[BvSpReportSampleStatusSummary]
@SurveySID INT, 
@PersonsSIDs NVARCHAR (2000), 
@ITSIDs NVARCHAR (1000),
@HideZero BIT
AS
IF @SurveySID IS NULL AND @PersonsSIDs IS NULL AND @ITSIDs IS NULL
BEGIN
    SELECT 
	0 as [Index],
    0 as [StateID],
    '' as [StateName],
    0 as [Count],
    '' as [SurveyName],
    0 as [SampleSize],
    0 as [Calls],
    '' as [Person]
    
    RETURN 0
END

DECLARE @StrSurveySID NVARCHAR (16)
SET @StrSurveySID = CAST(@SurveySID AS NVARCHAR(16))

DECLARE @SurveyQreName NVARCHAR (255), @SurveyDescription NVARCHAR (255)
SELECT @SurveyQreName = ISNULL(Name, '''') FROM BvSurvey WHERE SID = @SurveySID AND State <> 2
SELECT @SurveyDescription = ISNULL(Description, '''') FROM BvSurvey WHERE SID = @SurveySID AND State <> 2
Set @SurveyDescription = REPLACE(@SurveyDescription,'''','''''') --escape single apostrophe

SET @SurveyQreName = @SurveyDescription + ' (' + @SurveyQreName + ')'

DECLARE @PersonsStatement NVARCHAR (1000)
DECLARE @PersonsFilter NVARCHAR (4000)
DECLARE @PersonsGroup NVARCHAR (255)
IF @PersonsSIDs IS NULL OR @PersonsSIDs = '' BEGIN
 SET @PersonsStatement = ' ''ALL_PERSONS'' '
 SET @PersonsFilter = ''
 SET @PersonsGroup = ''
 SET @PersonsSIDs = ''
END
ELSE BEGIN
 SET @PersonsStatement = 
  ' IsNull((SELECT Name FROM BvPerson WHERE SID = 
  BvInterview.LastCallPersonSID), ''NO_CALLS'') '
 SET @PersonsFilter = 
  ' AND BvInterview.LastCallPersonSID in (' +
  @PersonsSIDs + ') '
 SET @PersonsGroup = ', BvInterview.LastCallPersonSID '
END

DECLARE @ITSFilter NVARCHAR (2000)
IF @ITSIDs = ''
 SET @ITSFilter = ''
ELSE
 SET @ITSFilter = ' AND bvstate.stateid IN (' + @ITSIDs + ') '

DECLARE @Query NVARCHAR (4000)
SET @Query=
 'SELECT
  (ROW_NUMBER() OVER(PARTITION BY ' + @PersonsStatement + ' ORDER BY BvState.StateID) - 1) as [Index],
  bvstate.stateid ''StateID'',
  bvstate.name ''StateName'',
  count( BvInterview.transientstate ) ''Count'',
  ''' + @SurveyQreName + ''' ''SurveyName'',
  (SELECT count(*) 
   FROM BvInterview 
   WHERE (SurveySID = ' + @StrSurveySID + ') ' +
   ') ''SampleSize'',
  0 ''Calls'',
   ' + @PersonsStatement + ' ''Person''
 FROM bvstate LEFT JOIN BvInterview 
 ON (bvstate.stateid = BvInterview.transientstate) 
 AND (SurveySID = ' + @StrSurveySID + ') ' +
 'LEFT JOIN BvSurvey ON
 bvsurvey.SID = ' + @StrSurveySID + '
 WHERE bvstate.StateGroupID = bvsurvey.StateGroupID 
  ' + @PersonsFilter + ' 
  ' + @ITSFilter + ' 
 GROUP BY bvstate.stateid, bvstate.name ' + @PersonsGroup

IF @HideZero = 1
BEGIN
  SET @Query = @Query + ' HAVING count( BvInterview.transientstate ) > 0'
END 

SET @Query = @Query + 'ORDER BY BvState.StateID'
/*print @Query*/
exec sp_executesql @Query
GO

PRINT N'Adding ReportGenerationTimeout system setting...'

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());
IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
 ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
 (
  SELECT 'Reports.ReportGenerationTimeout', 'ReportGenerationTimeout', 'System', 'This timeout is used for reports.', 1, 0, '120'
 )
 INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  SELECT d.* FROM Data d LEFT JOIN BvSystemSettings ss ON d.[SystemName] = ss.[SystemName] WHERE ss.[SystemName] IS NULL
END

GO
PRINT N'Update complete.';


GO
