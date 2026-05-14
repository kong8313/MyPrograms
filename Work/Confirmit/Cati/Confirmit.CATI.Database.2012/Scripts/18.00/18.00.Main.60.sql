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
	CAST( 0 AS BIGINT) as [Index],
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
PRINT N'Update complete.';


GO
