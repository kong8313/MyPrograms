UPDATE BvSystemSettings
  SET [Value] = 'True'
  WHERE [SystemName] = 'Toggle.EnableSampleUpdate' OR [SystemName] = 'Supervisor.AlwaysOpenNewUI'


GO
PRINT N'Update complete.';


GO
