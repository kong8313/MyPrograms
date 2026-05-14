UPDATE BvSystemSettings
  SET [Value] = 'True'
  WHERE [SystemName] = 'Toggle.EnableAutomaticSetCampaign'

GO
PRINT N'Update complete.';


GO
