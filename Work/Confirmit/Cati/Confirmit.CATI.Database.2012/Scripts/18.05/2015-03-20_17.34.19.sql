GO
PRINT N'Altering [dbo].[BvMembership].[IX_BvMembership_ContainerSID]...';


GO
ALTER INDEX [IX_BvMembership_ContainerSID]
    ON [dbo].[BvMembership] SET (ALLOW_PAGE_LOCKS = ON);


GO
PRINT N'Altering [dbo].[BvMembership].[IX_BvMembership_ObjectSID]...';


GO
ALTER INDEX [IX_BvMembership_ObjectSID]
    ON [dbo].[BvMembership] SET (ALLOW_PAGE_LOCKS = ON);


GO
PRINT N'Update complete.';


GO
