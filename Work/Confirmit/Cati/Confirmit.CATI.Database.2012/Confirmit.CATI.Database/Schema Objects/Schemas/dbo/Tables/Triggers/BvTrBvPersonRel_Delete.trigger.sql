CREATE TRIGGER [BvTrBvPersonRel_Delete] ON [dbo].[BvPersonRel] 
AFTER DELETE
AS
BEGIN
	SET NOCOUNT ON
	
	DELETE FROM BvLoginGroup
	FROM deleted 
	WHERE BvLoginGroup.PersonSID = deleted.PersonSID AND BvLoginGroup.ObjectSID = deleted.ObjectSID 
	
END
