GO
PRINT N'Dropping [dbo].[BvSvySchedule].[IX_BvSvySchedule_Rel]...';


GO
DROP INDEX [IX_BvSvySchedule_Rel]
    ON [dbo].[BvSvySchedule];


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvSvySchedule_Rel]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvSvySchedule_Rel]
    ON [dbo].[BvSvySchedule]([ExplicitSID] ASC, [DialTypeId] ASC);


GO
PRINT N'Update complete.';


GO
