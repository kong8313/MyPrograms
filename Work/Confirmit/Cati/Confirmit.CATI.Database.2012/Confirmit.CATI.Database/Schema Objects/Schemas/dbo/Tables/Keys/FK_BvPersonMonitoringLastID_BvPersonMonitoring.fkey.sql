ALTER TABLE [dbo].[BvPersonMonitoringLastID]
    ADD CONSTRAINT [FK_BvPersonMonitoringLastID_BvPersonMonitoring] FOREIGN KEY ([PersonSID]) REFERENCES [dbo].[BvPersonMonitoring] ([PersonSID]) ON DELETE CASCADE ON UPDATE NO ACTION;

