PRINT N'Altering [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]
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

DECLARE @ReplicationTable NVARCHAR(256) = (SELECT DestinationTableName FROM BvSurvey WHERE SID = @surveySid )
DECLARE @Query NVARCHAR(MAX) = 
	'DELETE FROM [' + @ReplicationTable +'] FROM [' + @ReplicationTable + '] r
		INNER JOIN BvTransferArrays ta 
			ON r.respid = ta.ItemId and ta.BatchID = ' + CAST( @batchId AS NVARCHAR(64))

EXEC( @Query )
GO
PRINT N'Update complete.';


GO
