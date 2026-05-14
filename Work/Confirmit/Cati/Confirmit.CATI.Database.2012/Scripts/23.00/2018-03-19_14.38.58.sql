ALTER TABLE [dbo].[BvTasks]
    ADD [JsonContext] NVARCHAR (MAX) NULL;

GO

CREATE TABLE BvActiveDial
(
	Id INT NOT NULL,
	Type TINYINT NOT NULL,
	DialerId INT NOT NULL,
	DdiNumber NVARCHAR(MAX),
	TelephoneNumber NVARCHAR(MAX),
	StartTime DATETIME NOT NULL,
	AnswerTime DATETIME NULL,
	InboundCallId NVARCHAR(MAX),
	InitialSurveyId INT NOT NULL,
	State TINYINT NOT NULL,
	SurveyId INT NULL,
	InterviewId INT NULL,
	CallId INT NULL,
	MainPersonId INT NULL,
    CONSTRAINT PK_BvActiveDial PRIMARY KEY ( ID )
)

GO

CREATE TABLE BvDialHistory
(
	ID INT NOT NULL,
	Type TINYINT NOT NULL,
	DialerId INT NOT NULL,
	InitialSurveyId INT NOT NULL,
	DdiNumber NVARCHAR(MAX),
	TelephoneNumber NVARCHAR(MAX),
	InboundCallId NVARCHAR(MAX),
	CallCompleteStatus TINYINT NOT NULL,
	StartTime DATETIME NOT NULL,
	AnswerTime DATETIME NULL,
	FinishTime DATETIME NOT NULL
    CONSTRAINT PK_BvDialHistory PRIMARY KEY ( ID )
)

GO

CREATE TABLE [dbo].[BvDialHistoryToInterviewHistory] (
    [DialHistoryId]      INT      NOT NULL,
    [InterviewHistoryId] INT      NOT NULL,
    [StartTime]          DATETIME NOT NULL,
    [FinishTime]         DATETIME NOT NULL,
    [PersonId]           INT      NOT NULL,
    CONSTRAINT [PK_BvDialHistoryToInterviewHistory] PRIMARY KEY CLUSTERED ([InterviewHistoryId] ASC, [DialHistoryId] ASC)
);

CREATE NONCLUSTERED INDEX [IX_BvDialHistoryToInterviewHistory_DialHistoryId]
    ON [dbo].[BvDialHistoryToInterviewHistory]([DialHistoryId] ASC);

GO

CREATE SEQUENCE [dbo].[BvDialIdSequence]
    AS INT
    START WITH 1
    INCREMENT BY 1;

GO

ALTER PROCEDURE [dbo].[BvSpHistory_CfData_Insert]
    @ProjectID NVARCHAR(256),
    @RespondentPhone NVARCHAR(256),
    @FiredTime DATETIME,
    @InterviewID INT,
    @Status_CF NVARCHAR(256),
    @AppointmentID INT,
    @OpenEndReviewDuration INT,
    @GrossDuration INT,
    @TotalDuration INT,
    @InterviewerID INT,
    @RoleID INT,
	@WaitingTime INT,
	@CallCenterId INT,
	@LinkedInterviewSessionId INT = NULL
AS
DECLARE @SurveySID INT
DECLARE @InterviewerID_BF INT
DECLARE @StatusBvFEE INT
    -- get survey sid and validate it
    SELECT @SurveySID = [Sid] FROM [BvSurvey] WHERE [Name] = @ProjectID
    IF @SurveySID IS NULL
    BEGIN
        RAISERROR('Survey for project %s does not exist', 16, 1, @ProjectID)
        RETURN -1
    END
    -- get interviewer and validate it
    IF ( @roleID = 2 /* CATI */ )
    BEGIN
        IF NOT EXISTS ( SELECT [Sid] FROM [BvPerson] WHERE [Sid] = @InterviewerID )
        BEGIN
            --We should ingnore wrong interviewer, because interviewer can be alredy deleted
            SET @InterviewerID_BF = 0
        END
        SET @InterviewerID_BF = @InterviewerID
    END
    ELSE IF ( @RoleID = 64 /* CAPI */ )
    BEGIN
        RAISERROR('CAPI data isn''t supported now.', 16, 1)
        RETURN -1
    END
    -- get BvFEE status by CfStatus and validate it
    SELECT @StatusBvFEE = [StatusCode_BvFEE] FROM [BvConfirmitStatus]
        WHERE [StatusCode_Cnf] = @Status_CF OR ( @Status_CF IS NULL AND [StatusCode_Cnf] IS NULL )
    IF @StatusBvFEE IS NULL
    BEGIN
        SET @StatusBvFEE = 30 --ERROR ITS
    END
    --if BvFEE status is appointment we should get latests active appointmentId for the interview
    --because CF does not pass appID but it should be stored in [Hst_Path3] field
    SELECT @AppointmentID = MAX([ID]) FROM [BvAppointment]
		WHERE [SurveySID] = @SurveySID AND InterviewSID = @InterviewID AND [State] = 0 /* has not call*/
	SET @AppointmentID = ISNULL(@AppointmentID, 0) --if appointment does not exist
    INSERT INTO [BvHistory]
    (
            [SurveyId],
            [TelephoneNumber],
            [FiredTime],
            [InterviewID],
            [ITS],
            [AppointmentID],
            [WaitingTime],
            [ConfirmitDuration],
            [Duration],
            BatchId,
            [PersonSID],
            [RoleID],
			[CallCenterID],
			[OpenEndReviewDuration],
			[LinkedInterviewSessionId]
    )
    SELECT
		@SurveySID      /*Hst_ObjID*/,
		@RespondentPhone /*TelephoneNumber*/,
		@FiredTime       /*FiredTime*/,
		@InterviewID     /*InterviewID*/,
		@StatusBvFEE     /*ITS*/,
		@AppointmentID   /*AppointmentID*/,
		@WaitingTime     /*WaitingTime*/,
		@GrossDuration   /*ConfirmitDuration*/,
		@TotalDuration   /*Duration*/,
		0                 /*BatchId*/,
		@InterviewerID_BF /*PersonSID*/,
		@RoleID           /*RoleID*/,
		@CallCenterID,
		@OpenEndReviewDuration,
		@LinkedInterviewSessionId
    FROM (
			SELECT @SurveySID SurveySID,
			       @InterviewID InterviewID
		 ) CfData
RETURN SCOPE_IDENTITY()

GO

CREATE PROCEDURE [dbo].[BvSpActiveDial_Delete]
 @IDs BvIntArrayType READONLY,
 @CallCompleteStatus TINYINT
AS
BEGIN TRAN
	DECLARE @Values TABLE( Id INT, Type TINYINT, DialerId INT, DdiNumber NVARCHAR(MAX), TelephoneNumber NVARCHAR(MAX), StartTime DATETIME, AnswerTime DATETIME, InboundCallId NVARCHAR(MAX), InitialSurveyId INT )
	DELETE FROM BvActiveDial 
		OUTPUT	deleted.Id, deleted.Type, deleted.DialerId, deleted.DdiNumber, deleted.TelephoneNumber,
				deleted.StartTime, deleted.AnswerTime, deleted.InboundCallId, deleted.InitialSurveyId INTO @Values
			WHERE ID IN ( SELECT Value FROM @IDs)
	DECLARE @Now DATETIME = [dbo].GetUtcNow()
	INSERT INTO BvDialHistory( Id, Type, DialerId, DdiNumber, TelephoneNumber, InboundCallId, InitialSurveyId, CallCompleteStatus, StartTime, AnswerTime, FinishTime )
		SELECT Id, Type, DialerId, DdiNumber, TelephoneNumber, InboundCallId, InitialSurveyId, @CallCompleteStatus, StartTime, AnswerTime, @Now
			FROM @Values
COMMIT TRAN

GO

CREATE PROCEDURE [dbo].[BvSpActiveDial_Insert]
 @Type TINYINT,
 @DialerId INT,
 @DdiNumber NVARCHAR(MAX),
 @TelephoneNumber NVARCHAR(MAX),
 @State TINYINT,
 @InboundCallId NVARCHAR(MAX),
 @InitialSurveyId INT
AS
	DECLARE @OldIds BvIntArrayType 
	INSERT INTO @OldIds SELECT ID FROM BvActiveDial WHERE InboundCallId = @InboundCallId 
	IF @@ROWCOUNT <> 0
	BEGIN
		EXEC BvSpActiveDial_Delete 0/*CallCompleteStatus.Error*/, @OldIds
	END
	INSERT INTO [dbo].[BvActiveDial]( [Id] 
			,[Type] ,[DialerId] ,[DdiNumber] ,[TelephoneNumber] ,[StartTime] ,[State], InboundCallId, InitialSurveyId)
		OUTPUT inserted.*
		VALUES( NEXT VALUE FOR [dbo].[BvDialIdSequence]
			,@Type, @DialerId, @DdiNumber, @TelephoneNumber, [dbo].GetUtcNow(), @State, @InboundCallId, @InitialSurveyId)

GO

CREATE PROCEDURE [dbo].[BvSpActiveDial_Update]
 @Id INT,
 @State TINYINT,
 @AnswerTime DATETIME,
 @SurveyId INT,
 @InterviewId INT,
 @CallId INT,
 @MainPersonId INT
AS
	DECLARE @Ids TABLE( ID INT )
	DELETE FROM BvActiveDial OUTPUT deleted.id INTO @Ids WHERE Id <> @Id AND (
		(SurveyId = @SurveyId AND InterviewId = @InterviewId ) 
		OR (CallId = @CallId)
		OR (ISNULL( @MainPersonId, 0)  <> 0 AND @MainPersonId = MainPersonId )
		)
	IF @@ROWCOUNT <> 0 
	BEGIN
		EXEC BvSpActiveDial_Delete 0/*CallCompleteStatus.Error*/, @Ids
	END
	UPDATE BvActiveDial 
		SET State = @State,
			AnswerTime = @AnswerTime,
			SurveyId = @SurveyId,
			InterviewId = @InterviewId,
			CallId = @CallId,
			MainPersonId = @MainPersonId
		WHERE Id = @Id

GO

CREATE PROCEDURE [dbo].[BvSpReportInboundCalls]
@SurveySID INT, 
@ITSIDs NVARCHAR (1000),
@StartDateTime DATETIME,
@EndDateTime DATETIME
AS
    CREATE TABLE #itses(its int primary key)
    insert into #itses
    SELECT Item
    FROM dbo.utilSplitNumbers( ISNULL(@ITSIDs, ''), ',')
    ;WITH Data AS(
        SELECT    DATEPART(HOUR, StartTime ) as HourInDay, * FROM BvDialHistory 
            WHERE Type = 1 /*Inbound*/ AND StartTime BETWEEN @StartDateTime AND @EndDateTime AND InitialSurveyId = @SurveySID
    ),
    Completes as (
        SELECT    HourInDay, 
                COUNT( DISTINCT i.PersonSID ) as DistinctAgents,
                SUM( CASE WHEN cits.its IS NOT NULL THEN 1 ELSE 0 END ) as CompletesCount
            FROM Data d 
            INNER JOIN [dbo].[BvDialHistoryToInterviewHistory] d2i 
                ON d.ID = d2i.DialHistoryId 
            INNER JOIN BvHistory i 
                ON d2i.InterviewHistoryId = i.ID
            LEFT JOIN #itses cits ON cits.its = i.ITS
            GROUP BY HourInDay
    ),
    Groups AS(
        SELECT  HourInDay,
                COUNT(*) as TotalCalls, 
                SUM(CASE WHEN AnswerTime IS NOT NULL THEN 1 ELSE 0 END) as HandledCalls,
                SUM(CASE WHEN AnswerTime IS NULL AND CallCompleteStatus <> 3/*CallCompleteStatus.DropByRespondent*/ THEN 1 ELSE 0 END) as DroppedBySystem,
                SUM(CASE WHEN AnswerTime IS NULL AND CallCompleteStatus = 3/*CallCompleteStatus.DropByRespondent*/ THEN 1 ELSE 0 END) as AbandonedByResp,
                SUM(CASE WHEN AnswerTime IS NOT NULL THEN DATEDIFF(SECOND, AnswerTime, FinishTime ) ELSE 0 END) as SumOfSpeakingTimesForCoonnected,
                SUM(CASE WHEN AnswerTime IS NOT NULL THEN DATEDIFF(SECOND, StartTime, AnswerTime ) ELSE 0 END) as SumOfWaitingTimesForCoonnected,
                SUM(CASE WHEN AnswerTime IS NOT NULL THEN 1 ELSE 0 END) as CountOfCoonnected,
                SUM(CASE WHEN AnswerTime IS NULL THEN DATEDIFF(SECOND, StartTime, FinishTime ) ELSE 0 END) as SumOfWaitingTimesForNotCoonnected,
                SUM(CASE WHEN AnswerTime IS NULL THEN 1 ELSE 0 END) as CountOfNotCoonnected
        FROM Data
        GROUP BY HourInDay
    )
    SELECT    g.HourInDay as HourInDay, 
            g.TotalCalls as TotalCalls,
            g.HandledCalls as HandledCalls,
            g.DroppedBySystem as DroppedBySystem,
            g.AbandonedByResp as AbandonedByResp,
            CASE WHEN g.CountOfCoonnected > 0 THEN g.SumOfWaitingTimesForCoonnected / g.CountOfCoonnected ELSE 0 END as AvgWaitTimeForConnection,
            CASE WHEN g.CountOfNotCoonnected > 0 THEN g.SumOfWaitingTimesForNotCoonnected / g.CountOfNotCoonnected ELSE 0 END as AvgWaitTimeForAbandons,
            CAST( CASE WHEN g.CountOfCoonnected + g.CountOfNotCoonnected > 0 THEN CAST( g.CountOfNotCoonnected AS FLOAT) / (g.CountOfCoonnected + g.CountOfNotCoonnected) * 100 ELSE 0 END AS NUMERIC(5,2)) as AbandonRate,
            c.DistinctAgents as DistinctAgents,
            CASE WHEN g.CountOfCoonnected > 0 THEN g.SumOfSpeakingTimesForCoonnected / g.CountOfCoonnected ELSE 0 END as AvgCallDurationForConnected,
            c.CompletesCount as CompletesCount
        FROM Groups g
        LEFT JOIN Completes c ON g.HourInDay = c.HourInDay
        ORDER BY g.HourInDay
GO
PRINT N'Update complete.';


GO
