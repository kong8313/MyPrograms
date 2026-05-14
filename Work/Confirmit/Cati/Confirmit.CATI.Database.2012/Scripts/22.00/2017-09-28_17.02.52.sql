GO
PRINT N'Creating [dbo].[BvSurveyDialer]...';


GO
CREATE TABLE [dbo].[BvSurveyDialer] (
    [SurveyId]   INT            NOT NULL,
    [DialTypeId] TINYINT        NOT NULL,
    [DialerId]   INT 		NULL,
    CONSTRAINT [PK_BvSurveyDialer_SurveyId_DialTypeId] PRIMARY KEY CLUSTERED ([SurveyId] ASC, [DialTypeId] ASC)
);


GO
PRINT N'Altering [dbo].[BvSpGetNextAvailableDialer]...';


GO

ALTER PROCEDURE [dbo].[BvSpGetNextAvailableDialer]
	@SurveyId int, 
	@DialTypeId int
AS
	SET NOCOUNT ON
BEGIN
       DECLARE @DialerId INT = NULL

	   MERGE BvSurveyDialer WITH (UPDLOCK) as t
			USING ( SELECT @SurveyId, @DialTypeId ) as s( SurveyId, DialTypeId )
			ON (t.SurveyId = s.SurveyId AND t.DialTypeId = s.DialTypeId)
			WHEN MATCHED THEN 
				UPDATE SET @DialerId = t.DialerId
			WHEN NOT MATCHED BY TARGET THEN
				INSERT(SurveyId, DialTypeId, DialerId) VALUES( SurveyId, DialTypeId, 0 );


       SET @DialerId = ( SELECT Id FROM BvDialers d WHERE d.DialTypeId = @DialTypeId AND d.IsActive = 1 AND DialerOperationalStateNotification = 1 AND d.Id = @dialerid)

       IF @DialerId is null
       BEGIN
              ;WITH newdialer AS 
              (
                      SELECT TOP 1 id,IsActive FROM BvDialers
                      WHERE DialTypeId = @DialTypeId AND IsActive = 1 AND DialerOperationalStateNotification = 1
                      ORDER BY LastSelected 
              )
              UPDATE newdialer SET IsActive = 1, @DialerId = id   --just fake update to increase timestamp

              UPDATE BvSurveyDialer SET DialerId = @dialerid WHERE SurveyId = @SurveyId AND DialTypeId = @DialTypeId
       END

       SELECT ISNULL(@DialerId, -1)
END
GO
PRINT N'Altering [dbo].[BvSpTasks_Update_2]...';


GO
ALTER PROCEDURE [dbo].[BvSpTasks_Update_2]
 @PersonSID int,
 @SurveySID int,
 @InterviewID int,
 @InterviewState tinyint,
 @TimeCallDelivered DATETIME,
 @CallOutcome int,
 @TzID int,
 @DiallingMode TINYINT
AS

DECLARE @Now DATETIME = [dbo].GetUtcNow()

IF( @SurveySID = 0 OR @InterviewID = 0 OR @InterviewState = 0)
BEGIN
 UPDATE [dbo].[BvTasks]
  SET SurveySID = @SurveySID,
   InterviewID = 0,
   InterviewState = @InterviewState,
   CallOutcome = (CASE WHEN @SurveySID = 0 THEN -1 ELSE @CallOutcome END),
   TzID = 0,
   CallID  = 0,
   TimeStateChanged = @Now,
   TimeCallDelivered = @TimeCallDelivered,
   DiallingMode = @DiallingMode
 WHERE PersonSID = @PersonSID
END
ELSE BEGIN
 UPDATE [dbo].[BvTasks]
  SET SurveySID = @SurveySID,
   InterviewID = @InterviewID,
   TimeStateChanged = @Now,
   InterviewState = @InterviewState,
   TimeCallDelivered = @TimeCallDelivered,
   CallOutcome = @CallOutcome,
   TzID = @TzID,
   DiallingMode = @DiallingMode
 WHERE PersonSID = @PersonSID
END

SELECT @@ROWCOUNT AS [RowCount], CallId from BvTasks
 WHERE PersonSID = @PersonSID
GO

PRINT N'Altering [dbo].[BvSpTasks_UpdateInterviewState]...';

GO
ALTER PROCEDURE [dbo].[BvSpTasks_UpdateInterviewState]
 @PersonSID int,
 @InterviewState int,
 @DiallingMode tinyint
AS

IF @InterviewState = 0 --NO_CALLS
BEGIN
 
 UPDATE [dbo].[BvTasks]
     SET 
      InterviewID = 0, 
      CallID = 0,
      TzID = 0,
      TimeStateChanged = GETUTCDATE(),
      TimeCallDelivered = NULL,
      InterviewState = @InterviewState,
      DiallingMode = @DiallingMode
 WHERE PersonSID = @PersonSID
END
ELSE IF @InterviewState = 6 --INTERVIEW_WRAP_UP 
BEGIN
 UPDATE [dbo].[BvTasks]
     SET InterviewState = @InterviewState,
      State = null,
      TimeStateChanged = GETUTCDATE()
 WHERE PersonSID = @PersonSID
END
ELSE IF @InterviewState = 5 --OPEN END REVIEW 
BEGIN
 UPDATE [dbo].[BvTasks]
     SET InterviewState = @InterviewState,
     OpenEndReviewStartTime = GETUTCDATE()
 WHERE PersonSID = @PersonSID
END

ELSE
BEGIN
 UPDATE [dbo].[BvTasks]
     SET InterviewState = @InterviewState
 WHERE PersonSID = @PersonSID
END

RETURN @@ROWCOUNT
GO

PRINT N'Altering [dbo].[BvSpTasks_InsertUpdate_2]...';

GO
ALTER PROCEDURE [dbo].[BvSpTasks_InsertUpdate_2]
 @PersonSID int,
 @SurveySID int,
 @ExtensionNumber NVARCHAR(256),
 @LoggedInToDialerState tinyint,
 @IsLoginRCToDialer BIT,
 @DiallingMode TINYINT
AS

DECLARE @Now DATETIME = [dbo].GetUtcNow()

UPDATE [dbo].[BvTasks]
    SET TimeStateChanged = @Now,
	    SurveySID = @SurveySID,
	    InterviewID = 0,
        StatusLogout = 2, --LOGGED_IN
        LoggedInToDialerState = @LoggedInToDialerState,
        IsLoginRCToDialer = @IsLoginRCToDialer,
        DiallingMode = @DiallingMode,
		StationExtensionNumber = @ExtensionNumber
WHERE PersonSID = @PersonSID

RETURN 0
GO
PRINT N'Update complete.';


GO
