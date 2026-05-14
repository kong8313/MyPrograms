CREATE PROCEDURE [dbo].[BvSpAssignment_Insert2]
@SurveySID INT, 
@PersonSID INT,
@BatchID INT
AS
SET NOCOUNT ON

    UPDATE BvSvySchedule 
    SET ExplicitSID = @PersonSID, 
        ExplicitType = 2, --Person type
        Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
        OldPriority = 0
    FROM BvTransferArrays
    WHERE BvTransferArrays.BatchID = @BatchID AND
          BvSvySchedule.SurveySID = @SurveySID AND
          BvSvySchedule.InterviewID = BvTransferArrays.ItemID AND
          BvSvySchedule.CallState > 0

RETURN (0)