PRINT N'Remove items from BvSearchableFields table realted to BBCC';


GO
DELETE FROM [BvSearchableFields] WHERE UseMode = 0


GO
PRINT N'Altering Procedure [dbo].[BvSpSurvey_Update]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurvey_Update]
        @SID            int,
        @Name           nvarchar( 255 ),
        @Description        nvarchar( 255 ),
        @QuotaType      tinyint,
        @DialMode tinyint,
        @forceOpnRev int,
        @StateGroupID int,
        @RecWholeInt int,
        @InterviewScreenRecording bit,
        @DestinationTableName NVARCHAR (255), 
        @ReplicationStatus BIT,
        @ScheduleID INT,
        @DialerParameters NVARCHAR(MAX),
        @IsTelephoneBlacklistSupported BIT,
        @IsRespondentsDynamicCreationAllowed BIT,
        @NotificationEmail NVARCHAR(MAX),
        @EnforceHttps BIT,
        @LastTouchTime SMALLDATETIME,
        @SurveySchedulingMode SMALLINT,
        @ClusteredQuotaName NVARCHAR(256),
        @ClusteredQuotaThreshold INT,
        @HiddenSearchableFields NVARCHAR(256) = '',
        @DialerId INT,
        @Target INT,
        @InternalTransferType TINYINT,
        @ExternalTransferType TINYINT,
        @IsLiveMonitoringEnabled BIT,
        @IsQuotaInCatiDb	  BIT,
        @InboundCallBehavior TINYINT,
        @Comment NVARCHAR(100) = NULL,
        @IsStateLocked BIT = 0
AS
SET NOCOUNT ON

EXEC   BvSpSurveyModifyStateGroup @SID, @StateGroupID

DECLARE @OldSurveyDescription NVARCHAR( 255 )
DECLARE @OldScheduleID INT
DECLARE @OldSurveySchedulingMode INT

UPDATE  BvSurvey
    SET [Name]               = @Name,     
        @OldSurveyDescription = [Description],
        [Description]        = @Description,       
        QuotaType            = @QuotaType,
		DialMode             = @DialMode,         
        ForceOpnRev          = @forceOpnRev,
        StateGroupID         = @StateGroupID,
        RecWholeInt          = @RecWholeInt,
		InterviewScreenRecording = @InterviewScreenRecording,
        DestinationTableName = @DestinationTableName,
        ReplicationStatus    = @ReplicationStatus,
        ScheduleID           = @ScheduleID,
        @OldScheduleID       = ScheduleID,
        DialerParameters	 = @DialerParameters,
        IsTelephoneBlacklistSupported = @IsTelephoneBlacklistSupported,
		IsRespondentsDynamicCreationAllowed = @IsRespondentsDynamicCreationAllowed,
        NotificationEmail	 = @NotificationEmail,
		[EnforceHttps]       = @EnforceHttps,
        [LastTouchTime]      = @LastTouchTime,
		@OldSurveySchedulingMode = [SurveySchedulingMode],
        [SurveySchedulingMode] = @SurveySchedulingMode,
		ClusteredQuotaName   = @ClusteredQuotaName,
		ClusteredQuotaThreshold = @ClusteredQuotaThreshold,
		DialerId			   = @DialerId,
		Target				   =@Target,
		InternalTransferType = @InternalTransferType,
		ExternalTransferType = @ExternalTransferType,
		IsLiveMonitoringEnabled = @IsLiveMonitoringEnabled,
		IsQuotaInCatiDb		 = @IsQuotaInCatiDb,
		InboundCallBehavior = @InboundCallBehavior,
		Comment = @Comment,
        IsStateLocked = @IsStateLocked
    WHERE SID = @SID

-- SL. Should we use such optimization here? It works incorrectly with NULLs. BvSurvey allows NULL for the Description field.
IF (@OldSurveyDescription != @Description) 
BEGIN
   UPDATE BvAggregateSurveyAlertStatus
   SET Description = @Description
   WHERE SID = @SID
   
   UPDATE BvAppointmentsAlertStatus
   SET SurveyName = @Description
   WHERE SurveySID = @SID
   
   UPDATE BvAppointmentCounters
   SET SurveyName = @Description
   WHERE SurveySID = @SID
END

EXEC    BvSpMembership_Delete 0, @SID


IF @OldScheduleID <> @ScheduleID
BEGIN
    /*
     * change scheduling parameters
     */
    --delete specific survey schedule params
    DELETE FROM BvScheduleParam WHERE SurveySID = @SID
    -- Add default schedule param of current scheduling script to BvScheduleParam table
    INSERT INTO BvScheduleParam( ScheduleID, SurveySID, ParamID, [Name], Description, Type, Value ) 
        SELECT sp.ScheduleID, @SID, sp.ParamID, sp.[Name], sp.Description, sp.Type, sp.Value
            FROM BvScheduleParam sp 
                WHERE sp.SurveySID = 0 AND sp.ScheduleID = @ScheduleID
END

IF @OldSurveySchedulingMode <> @SurveySchedulingMode
BEGIN
	IF @SurveySchedulingMode = 0 
	BEGIN
		UPDATE BvSvySchedule SET ConditionValue = 0 WHERE SurveySID = @SID
	END
	ELSE
	BEGIN
		UPDATE BvSvySchedule 
			SET ConditionValue = TransientState
		FROM BvInterview 
			WHERE BvSvySchedule.SurveySID = @SID AND BvInterview.SurveySID = @SID AND BvSvySchedule.InterviewID = BvInterview.ID
	END
END

return 0
GO


PRINT N'Altering Procedure [dbo].[BvSpGetSurveyInterviews]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetSurveyInterviews]
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
GO


PRINT N'Update complete.';

