CREATE PROCEDURE [dbo].[BvSpStateGroup_Update]
@ObjectSID INTEGER,
@Name      VARCHAR(255)
AS

     UPDATE BvStateGroup SET [Name] = @Name WHERE [ID] = @ObjectSID

RETURN ( 0 )