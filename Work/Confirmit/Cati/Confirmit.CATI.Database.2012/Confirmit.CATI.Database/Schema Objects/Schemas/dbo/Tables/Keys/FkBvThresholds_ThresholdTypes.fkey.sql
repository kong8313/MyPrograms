ALTER TABLE [dbo].[BvThresholds]
    ADD CONSTRAINT [FkBvThresholds_ThresholdTypes] FOREIGN KEY ([ThresholdsTypeID]) REFERENCES [dbo].[BvThresholdTypes] ([ID]) ON DELETE CASCADE ON UPDATE NO ACTION;

