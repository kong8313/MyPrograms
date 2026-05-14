CREATE  PROCEDURE [dbo].[BvSpSurvey_Insert]
        @SID int,
        @Name nvarchar( 255 ),
        @Description nvarchar( 255 ),
        @QuotaType tinyint,
		@DialMode tinyint,
        @State int,
        @forceOpnRev int,
        @StateGroupID int,
        @RecWholeInt int,
		@InterviewScreenRecording bit,
        @RouteAddress NVARCHAR(255),
        @CfDbSchemaPath NVARCHAR(255),
        @DestinationTableName NVARCHAR (255), 
		@ReplicationStatus BIT,
		@ScheduleID INT,
		@DialerParameters NVARCHAR(MAX),
		@IsTelephoneBlacklistSupported BIT,
		@NotificationEmail NVARCHAR(MAX),
		@EnforceHttps BIT,
		@SurveySchedulingMode SMALLINT,
		@SurveySqlServerName NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON


	IF @StateGroupID = 0
	BEGIN
		DECLARE @MinOrder INTEGER
		SELECT @MinOrder = MIN([Order]) FROM BvStateGroup
		SELECT @StateGroupID = [ID] FROM BvStateGroup WHERE [Order] = @MinOrder
	END


	IF ISNULL( @ScheduleID, 0 ) = 0
	BEGIN
		SELECT @ScheduleID = MIN( ScheduleID ) FROM BvSchedule
	END

	INSERT  BvSurvey( 
			SID, 
			[Name], 
			[Description],
			QuotaType,
			DialMode,
			State,
			ForceOpnRev,
			StateGroupID,
			RecWholeInt,
			InterviewScreenRecording,
			CfDbSchemaPath,
			DestinationTableName, 
			ReplicationStatus,
			ScheduleID,
			DialerParameters,
			IsTelephoneBlacklistSupported,
			[NotificationEmail],
			[EnforceHttps],
			SurveySchedulingMode,
			SurveySqlServerName
			)
		VALUES
		(
			@SID,
			@Name,
			@Description,
			@QuotaType,
			@DialMode,
			@State,
			@forceOpnRev,
			@StateGroupID,
			@RecWholeInt,
			@InterviewScreenRecording,
			@CfDbSchemaPath,
			@DestinationTableName, 
			@ReplicationStatus,
			@ScheduleID,
			@DialerParameters,
			@IsTelephoneBlacklistSupported,
			@NotificationEmail,
			@EnforceHttps,
			@SurveySchedulingMode,
			@SurveySqlServerName	
		)
        
	INSERT BvAggregateSurvey (SID) VALUES(@SID)
	INSERT BvAggregateSurveyAlertStatus (SID, Name, Description) VALUES(@SID, @Name, @Description)

	INSERT BvAppointmentCounters (SurveySID, SurveyName, ProjectID, CountForShortInterval, CountForLongInterval)
	VALUES(@SID, @Description, @Name, 0, 0)

	INSERT INTO BvSampleStatusSummary( SurveySID, ITS, IsCati ) 
			SELECT @SID, StateID, 0 FROM BvState WHERE StateGroupID = @StateGroupID
	
	INSERT INTO BvSampleStatusSummary( SurveySID, ITS, IsCati ) 
			SELECT @SID, StateID, 1 FROM BvState WHERE StateGroupID = @StateGroupID

	-- Add default schedule param of current scheduling script to BvScheduleParam table
	INSERT INTO BvScheduleParam( ScheduleID, SurveySID, ParamID, [Name], Description, Type, Value ) 
		SELECT sp.ScheduleID, @SID, sp.ParamID, sp.Name, sp.Description, sp.Type, sp.Value
					 FROM BvScheduleParam sp 
							WHERE sp.SurveySID = 0 AND sp.ScheduleID = @ScheduleID

	RETURN (0)
END
