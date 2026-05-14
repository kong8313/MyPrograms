CREATE PROCEDURE [dbo].[BvSpThresholdITS_Delete]
    @SurveySID INT,
    @ITS       INT
AS
    IF @SurveySID <> 0 
    BEGIN
        RAISERROR( 'SurveySID reserved, must be zero', 16, 1 )
        RETURN (-1)
    END

    IF @SurveySID = 0 
    BEGIN
        EXEC BvSpThresholdITS_Set 0, @ITS, 2147483647, 2147483647
    END
    ELSE
    BEGIN
        DELETE FROM BvThresholdITS 
            WHERE SurveySID = @SurveySID AND ITS = @ITS

        DECLARE @DefAmber INT
        DECLARE @DefRed INT

        SELECT @DefAmber = Amber, @DefRed = Red FROM BvThresholdITS 
            WHERE SurveySID = 0 AND ITS = @ITS

        UPDATE BvSampleStatusSummary
            SET alertStatus = dbo.udf_AlertStatus_INT( BvSampleStatusSummary.Cnt, @DefAmber, @DefRed )
            WHERE SurveySID = @SurveySID AND ITS = @ITS
    END