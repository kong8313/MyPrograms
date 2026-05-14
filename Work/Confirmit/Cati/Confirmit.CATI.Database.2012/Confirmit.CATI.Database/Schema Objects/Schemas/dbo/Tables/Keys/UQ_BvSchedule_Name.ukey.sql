ALTER TABLE [dbo].[BvSchedule]
    ADD CONSTRAINT [UQ_BvSchedule_Name] UNIQUE NONCLUSTERED ([Name])