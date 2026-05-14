ALTER TABLE [dbo].[BvPersonMonitoring]
    ADD CONSTRAINT [FK_BvPersonMonitoring_BvPerson] FOREIGN KEY ([PersonSID]) REFERENCES [dbo].[BvPerson] ([SID]) ON DELETE CASCADE ON UPDATE NO ACTION;

