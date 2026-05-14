CREATE PROCEDURE [dbo].[BvSpInterviews_UpdateState_Batch]
@SurveySID INT, @BatchID INT, @StateID INT
AS
UPDATE BvInterview
   SET TransientState = @StateID 
   FROM BvInterview i
   INNER JOIN BvTransferArrays ta ON 
   i.ID = ta.ItemID AND
   i.SurveySID = @SurveySID AND
   ta.BatchID = @BatchID 
