CREATE PROCEDURE [dbo].[BvSpCalls_Delete_Batch]
	@surveySid INT,
	@batchId INT
AS    
 DECLARE @InterviewIds TABLE(Id INT)
    
 INSERT INTO @InterviewIds
 SELECT ItemID
 FROM BvTransferArrays ta
 WHERE BatchId = @batchID 
       
 UPDATE BvSvySchedule 
 SET CallState = 0
 FROM @InterviewIds iids
 WHERE SurveySID = @SurveySID AND
       iids.ID = InterviewId