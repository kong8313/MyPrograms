CREATE PROCEDURE [dbo].[BvSpPersonGroup_Insert]
        @SID                int,
        @Name               nvarchar( 255 ),
        @Description        nvarchar( 255 ),
        @InboundCallBehavior TINYINT,
        @CallTransferBehavior TINYINT,
        @IsAdministrative BIT = 0

AS
IF EXISTS ( SELECT [SID] FROM BvPersonGroup WHERE [Name] = @Name )
BEGIN
 RAISERROR('Person group with name %s already exists', 12, 2, @Name)
 RETURN -1
END

INSERT  BvPersonGroup( 
        SID,
        [Name],
        [Description],
        [InboundCallBehavior],
        [CallTransferBehavior],
        [IsAdministrative])
    VALUES( 
        @SID, 
        @Name,
        @Description,
        @InboundCallBehavior,
        @CallTransferBehavior,
        @IsAdministrative)

EXEC BvSpPerson_SpinUp @SID