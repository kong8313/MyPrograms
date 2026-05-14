
GO
PRINT N'Dropping [dbo].[BvAsyncOperationQueue].[IX_BvAsyncOperationQueue_State]...';


GO
DROP INDEX [IX_BvAsyncOperationQueue_State]
    ON [dbo].[BvAsyncOperationQueue];


GO
PRINT N'Creating [dbo].[BvAsyncOperationQueue].[IX_BvAsyncOperationQueue_State_Priority_Id]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvAsyncOperationQueue_State_Priority_Id]
    ON [dbo].[BvAsyncOperationQueue]([State] ASC, [Priority] ASC, [Id] ASC);


GO
PRINT N'Update complete.';


GO
