CREATE PROCEDURE [dbo].[BvSpThresholdITS_List]
    @SurveySID INT
AS

    IF @SurveySID <> 0 
    BEGIN
        RAISERROR( 'SurveySID reserved. Must be zero', 16, 1 )
        RETURN (-1 )
    END

    DECLARE @DefaultStateGroupSID INT
    SELECT @DefaultStateGroupSID = ID FROM BvStateGroup WHERE Name = 'Default group'

    SELECT  BvThresholdITS.SurveySID as SurveySID,
            BvThresholdITS.ITS AS ITS,
            BvState.Name as Name,
            BvThresholdITS.Amber as Amber,
            BvThresholdITS.Red as Red
        FROM BvThresholdITS 
        INNER JOIN BvState
        ON BvThresholdITS.ITS = BvState.StateID AND BvState.StateGroupID = @DefaultStateGroupSID
       WHERE SurveySID = @SurveySID

    RETURN (0)