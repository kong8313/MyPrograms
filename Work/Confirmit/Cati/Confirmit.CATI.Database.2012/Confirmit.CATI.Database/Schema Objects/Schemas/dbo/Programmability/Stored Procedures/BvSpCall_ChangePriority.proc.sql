CREATE  PROCEDURE [dbo].[BvSpCall_ChangePriority]
    @SurveySID INTEGER,
    @Priority INTEGER,
    @BatchID INTEGER
AS
   UPDATE BvSvySchedule 
   SET Priority = @Priority,
       OldPriority = 0
   FROM BvTransferArrays ta
   WHERE ta.BatchID = @BatchID AND 
         ta.ItemID = [InterviewID] AND
		 [SurveySID] = @SurveySID AND
         CallState > 0
RETURN(0)