PRINT N'Altering [dbo].[BvSpCall_MoveToITS]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_MoveToITS]
@SurveySID   INTEGER,
@BatchID    INTEGER,
@StateID     INTEGER
AS
   DECLARE @CfDbSchemaPath NVARCHAR(255)
   DECLARE @ProcessedCalls INT = 0
   DECLARE @SurveySchedulingMode INT 
   SELECT @CfDbSchemaPath = CfDbSchemaPath,
		  @SurveySchedulingMode = SurveySchedulingMode
   FROM BvSurvey
   WHERE SID = @SurveySID
   
   CREATE TABLE #InterviewIds(Id INT, DialingMode TINYINT, its TINYINT)
   CREATE TABLE #ids(Id INT)

   UPDATE BvInterview
   SET TransientState = @StateID 
   OUTPUT inserted.Id, inserted.DialingMode, inserted.TransientState
   INTO #InterviewIds
   FROM BvInterview i
   INNER JOIN BvTransferArrays ta ON i.ID = ta.ItemID AND
									 ta.BatchID = @BatchID AND
									 i.SurveySID = @SurveySID
   LEFT JOIN BvSvySchedule s ON i.Id = s.InterviewId AND
                                s.SurveySid = @SurveySID
   WHERE ISNULL(s.CallState, 1) > 0
         
   SET @ProcessedCalls = @@ROWCOUNT
   
   UPDATE BvSvySchedule 
   SET Priority = BvState.Priority,
       OldPriority = 0,
	   ConditionValue = CASE WHEN @SurveySchedulingMode = 1 THEN @StateID ELSE 0 END
   OUTPUT inserted.id INTO #ids
   FROM #InterviewIds ids
   INNER JOIN BvState ON BvState.StateID = @StateID
   INNER JOIN BvSurvey ON BvSurvey.SID = @SurveySID AND
                          BvState.StateGroupID = BvSurvey.StateGroupID
   WHERE BvSvySchedule.SurveySID = @SurveySID AND 
         BvSvySchedule.InterviewId = ids.Id AND
         BvSvySchedule.CallState > 0
   
   IF (@@ROWCOUNT < @ProcessedCalls AND CONTEXT_INFO() IS NOT NULL) 
   BEGIN
	
		DECLARE @OperationType TINYINT
		DECLARE @CallCenterId INT
		DECLARE @OperationId INT

		SELECT @OperationID = OperationId, @OperationType = OperationType, @CallCenterId = CallCenterId from dbo.GetContextData()
	 
		INSERT INTO BvCallhistory
			SELECT GETUTCDATE(), c.ApptID, c.ShiftTypeID, i.Id, @SurveySID, i.its, i.Dialingmode, c.CallState, c.[Priority], c.TimeInShift, c.ExpireTime, c.ExplicitSid, c.ExplicitType, c.CellId, 
                @OperationId, @OperationType, @CallCenterId, c.DialTypeId
			FROM #InterviewIds i
			LEFT JOIN BvSvySchedule c ON c.InterviewID = i.ID AND c.SurveySID = @SurveySId 
			WHERE i.ID NOT IN (SELECT ID FROM #ids) 
   END

   IF((@ProcessedCalls != 0) AND (@CfDbSchemaPath IS NOT NULL) AND (@CfDbSchemaPath != ''))
   BEGIN
	   DECLARE @Query NVARCHAR(1024)
	   SET @Query = 'UPDATE '+@CfDbSchemaPath+'.response_control '+
					'SET ITS = '+cast(@StateID as nvarchar(10))+ ' ' +
					'FROM #InterviewIds as ids '+
					'WHERE respid = ids.ID '
	   EXECUTE( @Query )
   END

   EXEC BvSpDeleteTransfer @BatchID

RETURN @ProcessedCalls
GO
PRINT N'Update complete.';


GO
