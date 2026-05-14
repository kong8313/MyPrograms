CREATE PROCEDURE [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]
@surveySid INT, @batchId INT
AS
-- Delete appointments

 DELETE BvAppointment
 FROM BvTransferArrays
 WHERE SurveySID = @SurveySID AND
       BvTransferArrays.BatchId = @batchId AND
       ItemId = BvAppointment.InterviewSID

-- Delete interviews
 DELETE BvInterview 
 FROM BvTransferArrays
 WHERE SurveySID = @surveySid AND
       BvTransferArrays.BatchId = @batchId AND
       ID = ItemID