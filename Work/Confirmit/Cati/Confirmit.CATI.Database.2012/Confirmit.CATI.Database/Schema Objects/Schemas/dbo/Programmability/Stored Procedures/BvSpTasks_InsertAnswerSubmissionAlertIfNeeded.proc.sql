CREATE PROCEDURE [dbo].[BvSpTasks_InsertAnswerSubmissionAlertIfNeeded]
	@PersonSid int
AS
    --Answer submission alert thresholds
    DECLARE @AmberOfAnswerSubmissionAlert INT
    DECLARE @RedOfAnswerSubmissionAlert INT
    
    SELECT 
        @AmberOfAnswerSubmissionAlert = Amber,
        @RedOfAnswerSubmissionAlert = Red
    FROM 
        BvThresholds 
    WHERE 
        ObjectSID = 0 /*Default value*/ AND 
        ThresholdsTypeID = 1/*Task alert*/
    
    DECLARE @AnswerDuration INT
    DECLARE @SubmissionTime DateTime
    DECLARE @surveyId INT
    DECLARE @interviewId INT
    DECLARE @personId INT
    DECLARE @questionId NVARCHAR(256)
    DECLARE @InterviewState TINYINT
    
    SELECT 
        @AnswerDuration = DATEDIFF(s, TimeStateChanged, GETUTCDATE()),   --in this case TimeStateChanged will be previous value not @showTime
        @SubmissionTime = TimeStateChanged,
        @surveyId = surveySid,
        @interviewId = interviewId,
        @personId = PersonSID,
        @questionId = State,
        @InterviewState = InterviewState
    FROM
        BvTasks
    WHERE
        PersonSID = @PersonSid
    
    IF @questionId IS NULL
    BEGIN
       RETURN
    END
    
    DECLARE @AnswerSubmissionAlert BIT

    if ( @AnswerDuration >= @AmberOfAnswerSubmissionAlert  )
       SET @AnswerSubmissionAlert = 0
    if ( @AnswerDuration >= @RedOfAnswerSubmissionAlert  )
       SET @AnswerSubmissionAlert = 1


    if(@AnswerSubmissionAlert IS NOT NULL)
    BEGIN
        INSERT INTO BvAnswerSubmissionAlertHistory
        VALUES(@personId, @SubmissionTime, @questionId, @surveyId, @interviewId, @AnswerDuration, @AnswerSubmissionAlert, NULL, @InterviewState)
    END
    
RETURN 0