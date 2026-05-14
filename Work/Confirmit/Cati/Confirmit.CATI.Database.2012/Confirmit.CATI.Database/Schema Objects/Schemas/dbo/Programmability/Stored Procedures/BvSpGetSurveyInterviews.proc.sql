CREATE PROCEDURE [dbo].[BvSpGetSurveyInterviews]
@SurveySID INT, @PersonSID INT, @AssignmentsListmode INT, @ConfirmitVariablePrefix NVARCHAR(MAX),  @filterQuery NVARCHAR (MAX) = NULL, @InterviewsCountShownInManualMode INT
AS
SET NOCOUNT ON	
	
	DECLARE
		@sql          AS NVARCHAR(MAX),
		@selectSql    AS NVARCHAR(MAX),
		@whereSql     AS NVARCHAR(MAX),
		@replicatedColumns	 AS NVARCHAR(MAX),
		@replicatedColumnsAliases   AS NVARCHAR(MAX),
		@replicatedDataTable AS NVARCHAR(MAX),
		@personDialTypeId as TINYINT,
		@personManualSelection as INT
    
	SET @replicatedColumns = ''
	SET @replicatedColumnsAliases = ''
	SET @replicatedDataTable = 'BvReplicatedData_'+ CAST( @SurveySID AS VARCHAR(10) )

	SELECT	@personDialTypeId = BvTasks.DialTypeId, @personManualSelection = ManualSelection
            FROM BvTasks 
			INNER JOIN BvPerson ON PersonSid = SID
            WHERE PersonSID = @PersonSID

	IF ( @personManualSelection != 1 ) 
    BEGIN
        RETURN (0)
    END
	
	CREATE TABLE #replicatedColumnsNames (
        [ColumnName] NVARCHAR(MAX) NOT NULL
    )
    
    INSERT INTO #replicatedColumnsNames 
        SELECT [FieldName]
        FROM [BvSearchableFieldsOrdered]
        WHERE [SurveyId] = @SurveySID AND IsEnabled = 1 AND IsSystem = 0
       	
       	UPDATE #replicatedColumnsNames 
       	SET    @replicatedColumns = @replicatedColumns+ ',' + @replicatedDataTable + '.' + '[' + ColumnName + ']' + ' AS ' + @ConfirmitVariablePrefix + ColumnName + ' '
       	FROM #replicatedColumnsNames
       	       	
       	UPDATE #replicatedColumnsNames 
       	SET    @replicatedColumnsAliases = @replicatedColumnsAliases+ ',' + @ConfirmitVariablePrefix + ColumnName + ' '
       	FROM #replicatedColumnsNames
                           
    SET @selectSql = 'SELECT BvSvySchedule.[InterviewID],
 BvInterview.[RespondentName],
 BvInterview.[TelephoneNumber], 
 BvInterview.[TimezoneID],
 BvState.[Name] as [ITSName],
 BvSvySchedule.[Priority] as [Priority],
 BvSvySchedule.[TimeInShift] as [TimeToCall]'+
  @replicatedColumns+
 'FROM BvSvySchedule
 INNER JOIN BvSurvey ON BvSurvey.SID = BvSvySchedule.SurveySID AND BvSurvey.SID = ' + CAST(@SurveySID AS VARCHAR(16)) +'
 INNER JOIN BvInterview ON BvInterview.SurveySID = BvSvySchedule.SurveySID  AND BvInterview.[ID] = BvSvySchedule.InterviewID  AND ( BvInterview.TransientState <> 13 )'
 IF @AssignmentsListmode = 0
 BEGIN
	SET @selectSql = @selectSql + ' INNER JOIN BvLoginGroup WITH (NOLOCK) ON BvLoginGroup.PersonSID = ' + CAST(@PersonSID AS VARCHAR(16)) + ' AND BvLoginGroup.ObjectSID = BvSvySchedule.ExplicitSID'
 END

 SET @selectSql = @selectSql + ' INNER JOIN BvState ON BvState.StateGroupID = BvSurvey.StateGroupID AND BvState.StateID = BvInterview.TransientState
 LEFT JOIN '+ @replicatedDataTable + ' ON respId = InterviewID 
 WHERE BvSvySchedule.CallState = 2 AND BvSvySchedule.SurveySID = ' + CAST(@SurveySID AS VARCHAR(16)) + ' AND BvInterview.DialTypeId = ' + CAST(@personDialTypeId AS VARCHAR(10))
 
	
	IF(@filterQuery IS NOT NULL AND @filterQuery <> '')			
		SET @whereSql = ' WHERE ' + @filterQuery;
	ELSE
		SET @whereSql = '';
			
	--Need this construction to perform filtration using aliases
	SET @sql = 'SELECT TOP (' + cast(@InterviewsCountShownInManualMode as varchar(10)) + ')
				InterviewID, RespondentName, TelephoneNumber, ITSName, TimeToCall, TimezoneID ' + @replicatedColumnsAliases +
			   'FROM (' + @selectSql + ')S ' + @whereSql + 'ORDER BY Priority DESC'
print @sql
	EXECUTE sp_executesql @sql

RETURN (0)