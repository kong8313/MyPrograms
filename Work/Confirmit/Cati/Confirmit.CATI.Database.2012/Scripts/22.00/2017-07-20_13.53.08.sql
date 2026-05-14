PRINT N'Altering [dbo].[BvSpInterviews_UpdateIsSentToReview_Batch]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterviews_UpdateIsSentToReview_Batch]
@SurveySID INT, @BatchID INT, @ReviewStatus INT
AS
UPDATE BvInterview
   SET ReviewStatus = @ReviewStatus
   FROM BvInterview i
   INNER JOIN BvTransferArrays ta ON 
   i.ID = ta.ItemID AND
   i.SurveySID = @SurveySID AND
   ta.BatchID = @BatchID
GO
PRINT N'Update complete.';


GO
