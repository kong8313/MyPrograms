CREATE PROCEDURE [dbo].[BvSpThresholdITS_Set]
    @SurveySID INT,
    @ITS       INT,
    @Amber     INT,
    @Red       INT
AS
    IF @SurveySID <> 0 
    BEGIN
        RAISERROR( 'SurveySID reserved, must be zero', 16, 1 )
        RETURN (-1)
    END

    INSERT INTO BvThresholdITS( SurveySID, ITS, Amber, Red ) 
        SELECT @SurveySID, @ITS, @Amber, @Red 
            WHERE NOT EXISTS( SELECT 1 FROM BvThresholdITS WHERE SurveySID = @SurveySID AND ITS = @ITS )

    IF @@ROWCOUNT = 0 
        UPDATE BvThresholdITS
            SET Amber = @Amber,
                Red   = @Red
            WHERE SurveySID = @SurveySID AND ITS = @ITS

    UPDATE BvSampleStatusSummary
        SET alertStatus = dbo.udf_AlertStatus_INT( BvSampleStatusSummary.Cnt, @Amber, @Red )
        WHERE ITS = @ITS