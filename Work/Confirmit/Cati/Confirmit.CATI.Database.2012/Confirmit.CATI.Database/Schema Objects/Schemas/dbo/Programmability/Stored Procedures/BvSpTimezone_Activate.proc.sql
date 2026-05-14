CREATE PROCEDURE [dbo].[BvSpTimezone_Activate]
    @TzID INT
AS
    IF NOT EXISTS( SELECT 1 FROM BvTimezoneMaster WHERE ID = @TzID )
    BEGIN
        RAISERROR( 'Timezone with ID = ''%d'' not found in master list', 16, 1, @TzID )
        RETURN -1
    END

    INSERT INTO BvTimezone 
        SELECT *, NULL as ParentID FROM BvTimezoneMaster 
            WHERE ID = @TzID AND ID NOT IN( SELECT ID FROM BvTimezone )

    RETURN @@ROWCOUNT