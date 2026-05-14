GO
PRINT N'Altering [dbo].[BvState]...';


GO
ALTER TABLE [dbo].[BvState]
    ADD [AaporCode] NVARCHAR (10) NULL;

GO
PRINT N'Indert default AaporCode values to [dbo].[BvState]...';


GO
BEGIN

UPDATE BvState
SET [AaporCode] = '3.121'
WHERE [StateId] = 2;

UPDATE BvState
SET [AaporCode] = '3.122'
WHERE [StateId] = 3;

UPDATE BvState
SET [AaporCode] = '4.8'
WHERE [StateId] = 4;

UPDATE BvState
SET [AaporCode] = '2.11'
WHERE [StateId] = 5;

UPDATE BvState
SET [AaporCode] = '3.123'
WHERE [StateId] = 7;

UPDATE BvState
SET [AaporCode] = '4.2'
WHERE [StateId] = 8;

UPDATE BvState
SET [AaporCode] = '4.2'
WHERE [StateId] = 9;

UPDATE BvState
SET [AaporCode] = '3.1255'
WHERE [StateId] = 11;

UPDATE BvState
SET [AaporCode] = '1.1'
WHERE [StateId] = 13;

UPDATE BvState
SET [AaporCode] = '4.7'
WHERE [StateId] = 14;

UPDATE BvState
SET [AaporCode] = '3.11'
WHERE [StateId] = 16;

UPDATE BvState
SET [AaporCode] = '4.8'
WHERE [StateId] = 27;

UPDATE BvState
SET [AaporCode] = '3.2155'
WHERE [StateId] = 29;

END

GO
PRINT N'Altering [dbo].[BvSpState_List]...';


GO
ALTER PROCEDURE [dbo].[BvSpState_List]
@ObjectID     INTEGER
AS
DECLARE @StateGroupID INTEGER

SET @StateGroupID = 0

-- if default group
IF @ObjectID = 0
BEGIN
     DECLARE @MinOrder INTEGER
     SELECT @MinOrder = MIN([Order]) FROM BvStateGroup 
     SELECT @StateGroupID = [ID] FROM BvStateGroup WHERE [Order] = @MinOrder
END
-- if @ObjectID is a SurveySID
ELSE IF EXISTS( SELECT * FROM BvSurvey WHERE SID = @ObjectID AND State <> 2 )
     SELECT @StateGroupID = StateGroupID FROM BvSurvey WHERE SID = @ObjectID
-- if bad id
ELSE IF NOT EXISTS( SELECT * FROM BvStateGroup WHERE [ID] = @ObjectID )
     RETURN -1
ELSE
    SET @StateGroupID = @ObjectID

SELECT StateID, [Name], Priority, [DA], FcdAction, [AaporCode]  FROM BvState 
     WHERE StateGroupID = @StateGroupID  ORDER BY StateID
GO
PRINT N'Altering [dbo].[BvSpState_ListByGroup]...';


GO
ALTER PROCEDURE [dbo].[BvSpState_ListByGroup]
	@StateGroupID int
AS

-- if default group
IF @StateGroupID = 0
BEGIN
     DECLARE @MinOrder INTEGER
     SELECT @MinOrder = MIN([Order]) FROM [BvStateGroup] 
     SELECT @StateGroupID = [ID] FROM [BvStateGroup] WHERE [Order] = @MinOrder
END

SELECT [StateID], [Name], [Priority], [DA], [FcdAction], [AaporCode]  FROM [BvState] 
     WHERE [StateGroupID] = @StateGroupID  
     ORDER BY [StateID]
GO
PRINT N'Altering [dbo].[BvSpState_ListBySurvey]...';


GO
ALTER PROCEDURE [dbo].[BvSpState_ListBySurvey]
	@SurveySID int
AS

SELECT [StateID], [Name], [Priority], [DA], [FcdAction], [AaporCode] FROM [BvState]
     WHERE [StateGroupID] = (
		SELECT [StateGroupID] FROM [BvSurvey] WHERE [SID] = @SurveySID )
     ORDER BY [StateID]
GO
PRINT N'Altering [dbo].[BvSpState_Update]...';


GO
ALTER PROCEDURE [dbo].[BvSpState_Update]
 @ObjectID INT,
 @StateGroupID INT,
 @Name VARCHAR(255),
 @Priority INT,
 @DA BIT,
 @FcdAction INT,
 @AaporCode VARCHAR(10) = NULL
AS

DECLARE @OldPriority INT

SELECT @OldPriority = Priority
 FROM BvState 
 WHERE StateID = @ObjectID AND StateGroupID = @StateGroupID

UPDATE BvState 
 SET Priority = @Priority, [Name] = @Name, DA = @DA, FcdAction = @FcdAction, [AaporCode] = @AaporCode
 WHERE StateID = @ObjectID AND StateGroupID = @StateGroupID

IF ( @OldPriority <> @Priority )
BEGIN

 DECLARE crSurveys CURSOR LOCAL FOR SELECT [SID] FROM [BvSurvey]

 DECLARE @SurveySID INT
 DECLARE @SurveyProcedureName NVARCHAR(128)
 DECLARE @SurveysProcessed INT

 OPEN crSurveys
 FETCH NEXT FROM crSurveys INTO @SurveySID

 WHILE ( @@fetch_status = 0 )
 BEGIN
  SET @SurveyProcedureName = 'BvSpSurveyState_Update'

  EXEC @SurveyProcedureName @ObjectID, @StateGroupID, @Priority

  SET @SurveysProcessed = @SurveysProcessed + 1

  FETCH NEXT FROM crSurveys INTO @SurveySID
 END

 CLOSE crSurveys
 DEALLOCATE crSurveys

END

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpStateGroup_CopyToDefault]...';


GO
ALTER PROCEDURE [dbo].[BvSpStateGroup_CopyToDefault]
 @DefaultStateGroupId INT,
 @SourceStateGroupId INT
 AS
  UPDATE d
  SET
  [Priority] = s.[Priority],
  [Name] = s.[Name],
  [DA] = s.[DA],
  [FcdAction] = s.[FcdAction],
  [AaporCode] = s.[AaporCode]
  FROM BvState d INNER JOIN BvState s ON d.StateID = s.StateID
  WHERE d.StateGroupID = @DefaultStateGroupId AND s.StateGroupID = @SourceStateGroupId
GO
PRINT N'Altering [dbo].[BvSpStateGroup_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpStateGroup_Insert]
    @SID     INT,
    @CopyID  INT,
    @Name    VARCHAR(255)
AS
DECLARE @Order INTEGER

    IF NOT EXISTS( SELECT * FROM BvStateGroup )
    BEGIN
		RAISERROR('Default state group not found.', 16, 1)
		RETURN -1
	END

    -- if @ParentSID = 0 then find default group
    IF @CopyID = 0
    BEGIN
        SELECT @Order = MIN([Order] ) FROM BvStateGroup
        SELECT @CopyID = ISNULL( ID, 0 ) FROM BvStateGroup WHERE [Order] =@Order
    END

     SELECT @Order = MAX([Order] ) FROM BvStateGroup    
     SET @Order = @Order + 1

    -- Insert new state group
    INSERT INTO BvStateGroup(
        [ID],
        [Name],
        [Order],
        [Deleted])
    VALUES (
        @SID, 
        @Name,
        @Order,
        0)

    -- Copy States   
     INSERT INTO BvState( StateID, [Name], StateGroupID, Priority, DA, [FcdAction], [AaporCode] )
         SELECT StateID, [Name], @SID, Priority, DA, [FcdAction], [AaporCode] FROM BvState WHERE StateGroupID = @CopyID

RETURN 0
GO
PRINT N'Refreshing [dbo].[BvSpAttemptsByDispositionReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAttemptsByDispositionReport]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Activate]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Activate]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_MoveToITS]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_MoveToITS]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistory_List]';


GO
PRINT N'Refreshing [dbo].[BvSpGetAppointmentActivity]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAppointmentActivity]';


GO
PRINT N'Refreshing [dbo].[BvSpGetAppointmentActivityExtStatuses]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAppointmentActivityExtStatuses]';


GO
PRINT N'Refreshing [dbo].[BvSpGetExtendedCallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetExtendedCallHistory]';


GO
PRINT N'Refreshing [dbo].[BvSpReportSampleStatusSummaryForDatesRange]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportSampleStatusSummaryForDatesRange]';


GO
PRINT N'Refreshing [dbo].[BvSpSampleStatusSummary_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleStatusSummary_Get]';


GO
PRINT N'Refreshing [dbo].[BvSpStateGroup_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpStateGroup_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyModifyStateGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyModifyStateGroup]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyProductivityReport]';


GO
PRINT N'Refreshing [dbo].[BvSpThresholdITS_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpThresholdITS_List]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Update]';


GO
PRINT N'Update complete.';


GO
