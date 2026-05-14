CREATE PROCEDURE [dbo].[BvSpMembership_Delete]
        @ContainerSID int,
        @ObjectSID int

AS

IF  @ContainerSID = 0
    DELETE  BvMembership WITH(ROWLOCK)
        WHERE   ObjectSID = @ObjectSID
ELSE
    IF  @ObjectSID = 0
        DELETE  BvMembership WITH(ROWLOCK)
            WHERE   ContainerSID = @ContainerSID
    ELSE
        DELETE  BvMembership WITH(ROWLOCK)
            WHERE   ContainerSID = @ContainerSID
            AND ObjectSID = @ObjectSID