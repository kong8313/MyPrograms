
GO
PRINT N'Altering [dbo].[BvSurvey]...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD [DisableClrForFcd] BIT CONSTRAINT [DF_BvSurvey_DisableClrForFcd] DEFAULT (0) NOT NULL;


GO
PRINT N'Creating [dbo].[BvInterviewQuotaCell]...';


GO
CREATE TABLE [dbo].[BvInterviewQuotaCell] (
    [SurveyID]    INT NOT NULL,
    [InterviewId] INT NOT NULL,
    [QuotaID]     INT NOT NULL,
    [CellID]      INT NOT NULL,
    CONSTRAINT [PK_BvInterviewQuotaCell] PRIMARY KEY CLUSTERED ([SurveyID] ASC, [InterviewId] ASC, [QuotaID] ASC, [CellID] ASC)
);


GO
PRINT N'Creating [dbo].[BvInterviewQuotaCell].[IX_BvInterviewQuotaCell_SurveyId_QuotaId_CellID_InterviewId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInterviewQuotaCell_SurveyId_QuotaId_CellID_InterviewId]
    ON [dbo].[BvInterviewQuotaCell]([SurveyID] ASC, [QuotaID] ASC, [CellID] ASC, [InterviewId] ASC);


GO
PRINT N'Creating [dbo].[FK_BvInterviewQuotaCell_SurveyQuotaCell]...';


GO
ALTER TABLE [dbo].[BvInterviewQuotaCell] WITH NOCHECK
    ADD CONSTRAINT [FK_BvInterviewQuotaCell_SurveyQuotaCell] FOREIGN KEY ([SurveyID], [QuotaID], [CellID]) REFERENCES [dbo].[BvSurveyQuotaCell] ([SurveyID], [QuotaID], [CellID]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_BvInterviewQuotaCell_Interview]...';


GO
ALTER TABLE [dbo].[BvInterviewQuotaCell] WITH NOCHECK
    ADD CONSTRAINT [FK_BvInterviewQuotaCell_Interview] FOREIGN KEY ([SurveyID], [InterviewId]) REFERENCES [dbo].[BvInterview] ([SurveySID], [ID]) ON DELETE CASCADE;


GO
PRINT N'Altering [dbo].[BvSpSurvey_Update]...';


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
		@HiddenSearchableFields NVARCHAR(256),
		@DialerId INT,
		@Target INT,
		@InternalTransferType TINYINT,
		@ExternalTransferType TINYINT,
		@IsLiveMonitoringEnabled BIT,
		@IsQuotaInCatiDb	  BIT,
		@InboundCallBehavior TINYINT,
		@DisableClrForFcd BIT = 0
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
		HiddenSearchableFields = @HiddenSearchableFields,
		DialerId			   = @DialerId,
		Target				   =@Target,
		InternalTransferType = @InternalTransferType,
		ExternalTransferType = @ExternalTransferType,
		IsLiveMonitoringEnabled = @IsLiveMonitoringEnabled,
		IsQuotaInCatiDb		 = @IsQuotaInCatiDb,
		InboundCallBehavior = @InboundCallBehavior,
		DisableClrForFcd = @DisableClrForFcd
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
ALTER TABLE [dbo].[BvInterviewQuotaCell] WITH CHECK CHECK CONSTRAINT [FK_BvInterviewQuotaCell_SurveyQuotaCell];

ALTER TABLE [dbo].[BvInterviewQuotaCell] WITH CHECK CHECK CONSTRAINT [FK_BvInterviewQuotaCell_Interview];


GO
PRINT N'Update complete.';


GO
