CREATE TRIGGER [BvTrBvPersonRel_Insert] ON [dbo].[BvPersonRel] 
AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON
	
	INSERT INTO BvLoginGroup(PersonSID, ObjectSID, SurveySID, DialTypeId ) 
	SELECT i.PersonSID, i.ObjectSID, CASE WHEN p.ManualSelection = 2 /*is survey selection*/ THEN t.SurveySID ELSE 0 END, t.DialTypeId  FROM inserted i
	INNER JOIN BvTasks t ON i.PersonSID = t.PersonSID
	INNER JOIN BvPerson p ON i.PersonSID = p.SID
	
END
