GO
PRINT N'Altering [dbo].[BvCallGroup]...';


GO
ALTER TABLE [dbo].[BvCallGroup]
    ADD [DesignStateGroupID] INT NULL;


GO
PRINT N'Creating FK_BvCallGroup_BvStateGroup...';


GO
ALTER TABLE [dbo].[BvCallGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_BvCallGroup_BvStateGroup] FOREIGN KEY ([DesignStateGroupID]) REFERENCES [dbo].[BvStateGroup] ([ID]) ON DELETE SET NULL;


GO
PRINT N'Checking existing data against newly created constraints';


GO



GO
ALTER TABLE [dbo].[BvCallGroup] WITH CHECK CHECK CONSTRAINT [FK_BvCallGroup_BvStateGroup];


GO
PRINT N'Update complete.';


GO
