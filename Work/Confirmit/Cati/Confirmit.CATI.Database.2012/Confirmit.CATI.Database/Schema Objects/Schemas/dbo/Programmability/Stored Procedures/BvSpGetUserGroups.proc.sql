CREATE PROCEDURE [dbo].[BvSpGetUserGroups]
    @PersonSID INT
AS
    IF NOT EXISTS( SELECT 1 FROM BvPerson WHERE SID = @PersonSID )
    BEGIN
        RAISERROR( 'The person with SID="%u" not found', 16, 1, @PersonSID )
        RETURN -1
    END

    SELECT rel.ObjectSID AS GroupSID FROM bvpersonrel AS rel
        LEFT JOIN BvPersonGroup AS gr ON rel.ObjectSID = gr.SID
    WHERE 
        rel.PersonSID = @PersonSID AND rel.RoleID = 2 AND rel.Type = 1 AND (gr.IsAdministrative = 0 OR gr.SID IS NULL)
    
    RETURN @@ROWCOUNT