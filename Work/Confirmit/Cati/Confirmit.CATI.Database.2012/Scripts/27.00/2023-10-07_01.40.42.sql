GO
PRINT N'Add values to BvDialerToCallCenter table.';

INSERT INTO [dbo].[BvDialerToCallCenter] (CallCenterId, DialerId)
    SELECT ID, DialerId FROM [dbo].[BvCallCenter]
    WHERE DialerId <> 0 AND EXISTS 
(
   SELECT Id 
   FROM BvDialers 
   WHERE Id = DialerId
);


GO
PRINT N'Update complete.';


GO
