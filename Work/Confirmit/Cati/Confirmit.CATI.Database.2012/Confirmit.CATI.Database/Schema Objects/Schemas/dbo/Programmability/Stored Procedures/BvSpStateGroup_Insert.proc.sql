CREATE PROCEDURE [dbo].[BvSpStateGroup_Insert]
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
     INSERT INTO BvState( StateID, [Name], StateGroupID, Priority, DA, [FcdAction], [AaporCode] )
         SELECT StateID, [Name], @SID, Priority, DA, [FcdAction], [AaporCode] FROM BvState WHERE StateGroupID = @CopyID

RETURN 0