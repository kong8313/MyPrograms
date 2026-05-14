PRINT N'Enable Toggle.DirectlyInsertResponses for all companies.';

GO

UPDATE BvSystemSettings
  SET [Value] = 'True'
  WHERE [SystemName] = 'Toggle.DirectlyInsertResponses'

GO
PRINT N'Update complete.';


GO
