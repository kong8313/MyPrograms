CREATE PROCEDURE [dbo].[BvSpThreshold_List]
    @ObjectSID INT
AS
    IF @ObjectSID <> 0
    BEGIN
        RAISERROR( 'ObjectSID reserved. Must be zero.', 16, 1 )
        RETURN(0)
    END

    SELECT  ObjectSID,
            ThresholdsTypeID,
            Amber,
            Red
        FROM dbo.BvThresholds WHERE ObjectSID = @ObjectSID

    RETURN( 0 )