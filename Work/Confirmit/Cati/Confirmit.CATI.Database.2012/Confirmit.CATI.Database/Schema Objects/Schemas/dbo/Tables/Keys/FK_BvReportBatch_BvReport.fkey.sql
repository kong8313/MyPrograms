ALTER TABLE [dbo].[BvReportBatch]
    ADD CONSTRAINT [FK_BvReportBatch_BvReport] FOREIGN KEY ([ReportID]) REFERENCES [dbo].[BvReport] ([Rpt_ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

