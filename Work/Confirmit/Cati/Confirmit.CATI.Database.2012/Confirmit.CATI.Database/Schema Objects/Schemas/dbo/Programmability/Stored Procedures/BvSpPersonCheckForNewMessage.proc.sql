
CREATE PROCEDURE [dbo].[BvSpPersonCheckForNewMessage]
@PersonSID INT
AS

BEGIN
 
	SELECT ISNULL(HasNewMessage, 0) AS HasNewMessage
	FROM bvPerson 
	WHERE SID = @PersonSID

END
