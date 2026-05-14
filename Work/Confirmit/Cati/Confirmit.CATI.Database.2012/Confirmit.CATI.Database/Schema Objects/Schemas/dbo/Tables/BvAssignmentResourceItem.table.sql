CREATE TABLE [dbo].[BvAssignmentResourceItem]
(
	[AssignmentID] INT NOT NULL,
	[ResourceID] INT NOT NULL
)
GO

CREATE UNIQUE CLUSTERED INDEX [PK_BvAssignmentResourceItem] ON BvAssignmentResourceItem( [AssignmentID], [ResourceID] )
GO
CREATE INDEX [IX_BvAssignmentResourceItem_ResourceId] ON BvAssignmentResourceItem( [ResourceID] )
GO