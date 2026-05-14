ALTER TABLE [dbo].[BvReportParam]
    ADD CONSTRAINT [FK_BvReportParam_BvReportBatch] FOREIGN KEY ([BatchID]) REFERENCES [dbo].[BvReportBatch] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

