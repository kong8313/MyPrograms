PRINT N'Updating CallManagement.MaximumConfirmitVariables system setting ...';
GO

UPDATE BvSystemSettings SET [Value] = '50' WHERE SystemName='CallManagement.MaximumConfirmitVariables'


GO
PRINT N'Update complete.';


GO