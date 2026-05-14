PRINT N'Change DisplayName and Description for EnableBBCCLogin setting';


GO

  UPDATE BvSystemSettings
  SET [DisplayName] = 'Enable users to login via the Browser Based CATI Console',
  [Description] = 'Enable users to login via the Browser Based CATI Console'
  WHERE [SystemName] = 'Toggle.EnableBBCCLogin'

GO

PRINT N'Update complete.';


GO
