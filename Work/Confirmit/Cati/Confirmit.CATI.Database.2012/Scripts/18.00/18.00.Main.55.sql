UPDATE [dbo].[BvPerson] 
SET [PwdSetDate] = GETUTCDATE()
WHERE [PwdSetDate] is NULL

GO
ALTER TABLE [dbo].[BvPerson] ALTER COLUMN [PwdSetDate] DATETIME NOT NULL;

GO
ALTER TABLE [dbo].[BvPerson]
    ADD DEFAULT GETUTCDATE() FOR [PwdSetDate];

GO
