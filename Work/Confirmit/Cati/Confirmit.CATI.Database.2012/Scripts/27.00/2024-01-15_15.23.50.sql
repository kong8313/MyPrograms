PRINT N'Update Dialer.SettingsTemplatesJson and Dialer.DefaultSurveyParameters system settings...';

GO
UPDATE BvSystemSettings 
SET [Value] = REPLACE([Value], 'No reply timeout (no. of rings)', 'No reply timeout (seconds)') 
WHERE SystemName in ('Dialer.SettingsTemplatesJson', 'Dialer.DefaultSurveyParameters')

GO
PRINT N'Update complete.';


GO
