GO
PRINT N'Creating [dbo].[BvAssignmentResourceItem].[IX_BvAssignmentResourceItem_ResourceId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvAssignmentResourceItem_ResourceId]
    ON [dbo].[BvAssignmentResourceItem]([ResourceID] ASC);


GO
PRINT N'Creating [dbo].[BvAssignmentResourceItem].[PK_BvAssignmentResourceItem]...';


GO
CREATE UNIQUE CLUSTERED INDEX [PK_BvAssignmentResourceItem]
    ON [dbo].[BvAssignmentResourceItem]([AssignmentID] ASC, [ResourceID] ASC);


GO
PRINT N'Update complete.';


GO
