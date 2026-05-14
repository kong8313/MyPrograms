PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpStateGroup_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpStateGroup_Insert]
    @SID     INT,
    @CopyID  INT,
    @Name    VARCHAR(255)
AS
DECLARE @Order INTEGER

    IF NOT EXISTS( SELECT * FROM BvStateGroup )
    BEGIN
		RAISERROR('Default state group not found.', 16, 1)
		RETURN -1
	END

    -- if @ParentSID = 0 then find default group
    IF @CopyID = 0
    BEGIN
        SELECT @Order = MIN([Order] ) FROM BvStateGroup
        SELECT @CopyID = ISNULL( ID, 0 ) FROM BvStateGroup WHERE [Order] =@Order
    END

     SELECT @Order = MAX([Order] ) FROM BvStateGroup    
     SET @Order = @Order + 1

    -- Insert new state group
    INSERT INTO BvStateGroup(
        [ID],
        [Name],
        [Order],
        [Deleted])
    VALUES (
        @SID, 
        @Name,
        @Order,
        0)

    -- Copy States   
     INSERT INTO BvState( StateID, [Name], StateGroupID, Priority, DA, [FcdAction] )
         SELECT StateID, [Name], @SID, Priority, DA, [FcdAction] FROM BvState WHERE StateGroupID = @CopyID

RETURN 0
GO
PRINT N'Update complete.';


GO
