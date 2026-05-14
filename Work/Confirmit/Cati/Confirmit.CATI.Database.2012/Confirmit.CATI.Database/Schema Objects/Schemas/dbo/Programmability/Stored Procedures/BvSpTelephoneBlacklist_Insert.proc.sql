CREATE PROCEDURE [dbo].[BvSpTelephoneBlacklist_Insert]
@Type TINYINT,
@TelephoneNumber varchar(255),
@Comment varchar(74) = NULL
AS
SET NOCOUNT ON

DECLARE @ID TABLE( ID INT )
  
INSERT INTO [dbo].[BvTelephoneBlacklist]([Type], [TelephoneNumber], [Timestamp], [Comment])
	OUTPUT inserted.ID INTO @ID
    VALUES (@Type, @TelephoneNumber, GETUTCDATE(), @Comment)

RETURN ISNULL(( SELECT ID FROM @ID ), 0)