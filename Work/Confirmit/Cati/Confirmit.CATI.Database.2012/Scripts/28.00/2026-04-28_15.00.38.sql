PRINT N'Altering Procedure [dbo].[BvSpTask_UpdateActiveQuestion]...';


GO
ALTER PROCEDURE BvSpTask_UpdateActiveQuestion
 @projectId NVARCHAR(256),
 @catiInterviewerId INT,
 @qID NVARCHAR(256),
 @showTime DATETIME
AS

BEGIN TRY
    --Answer submission alert thresholds
    DECLARE @AmberOfAnswerSubmissionAlert INT
    DECLARE @RedOfAnswerSubmissionAlert INT
    SELECT @AmberOfAnswerSubmissionAlert = Amber, @RedOfAnswerSubmissionAlert = Red
    FROM BvThresholds 
    WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 1/*Task alert*/


    --Quick answer submission alert thresholds
    DECLARE @AmberOfQuickAnswerSubmissionAlert INT
    DECLARE @RedOfQuickAnswerSubmissionAlert INT
    SELECT @AmberOfQuickAnswerSubmissionAlert = Amber, @RedOfQuickAnswerSubmissionAlert = Red
    FROM BvThresholds 
    WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 17/*QuickAnswerSubmission alert*/


    DECLARE @AnswerDuration INT
    DECLARE @SubmissionTime DateTime
    DECLARE @surveyId INT
    DECLARE @interviewId INT
    DECLARE @personId INT
    DECLARE @questionId NVARCHAR(256)
    DECLARE @InterviewState TINYINT
    
    DECLARE @IsIncorrectOrder BIT = 0   --if previous question come in later.

    SELECT
        @IsIncorrectOrder = IIF(TimeStateChanged > @showTime, 1, 0),
        @AnswerDuration = DATEDIFF(s, TimeStateChanged, @showTime),   --in this case TimeStateChanged will be previous value not @showTime
        @SubmissionTime = TimeStateChanged,
        @surveyId = surveySid,
        @interviewId = interviewId,
        @personId = PersonSID,
        @questionId = State,
        @InterviewState = InterviewState
    FROM BvTasks
    WHERE PersonSID = @catiInterviewerId;
        
    IF (@IsIncorrectOrder = 0)
    BEGIN
        SET LOCK_TIMEOUT 500
        UPDATE BvTasks
        SET State = @qID,
            TimeStateChanged = @showTime
        WHERE PersonSID = @catiInterviewerId AND TimeStateChanged <= @showTime;
        SET LOCK_TIMEOUT -1
    END
                       
    IF @questionId IS NULL --first question
    BEGIN
       RETURN
    END
    
    IF @IsIncorrectOrder = 1
    BEGIN
       SET @questionId = @qID
       SET @AnswerDuration = -@AnswerDuration
       SET @SubmissionTime = @showTime
    END
                       
    DECLARE @AnswerSubmissionAlert BIT
    DECLARE @QuickAnswerSubmissionAlert BIT

    if ( @AnswerDuration >= @AmberOfAnswerSubmissionAlert  )
       SET @AnswerSubmissionAlert = 0
    if ( @AnswerDuration >= @RedOfAnswerSubmissionAlert  )
       SET @AnswerSubmissionAlert = 1
    if ( @AnswerDuration <= @AmberOfQuickAnswerSubmissionAlert )
       SET @QuickAnswerSubmissionAlert = 0
    if ( @AnswerDuration <= @RedOfQuickAnswerSubmissionAlert  )
       SET @QuickAnswerSubmissionAlert = 1


    if((@QuickAnswerSubmissionAlert IS NOT NULL OR @AnswerSubmissionAlert IS NOT NULL) AND @questionId != 'Internal_Stop')
    BEGIN
        INSERT INTO BvAnswerSubmissionAlertHistory
        VALUES(@personId, @SubmissionTime, @questionId, @surveyId, @interviewId, @AnswerDuration, @AnswerSubmissionAlert, @QuickAnswerSubmissionAlert, @InterviewState)
    END
END TRY
BEGIN CATCH
END CATCH;
GO
PRINT N'Update complete.';


GO
