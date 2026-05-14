ALTER TABLE [dbo].[BvPersonMonitoringEvents]
    ADD CONSTRAINT [FK_BvPersonMonitoringEvents_BvPersonMonitoring] FOREIGN KEY ([PersonSID]) REFERENCES [dbo].[BvPersonMonitoring] ([PersonSID]) ON DELETE CASCADE ON UPDATE NO ACTION;

