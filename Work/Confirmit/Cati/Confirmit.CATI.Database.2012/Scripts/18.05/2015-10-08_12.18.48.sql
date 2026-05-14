PRINT N'Dropping [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_CallID]...';


GO
DROP INDEX [IX_BvPersonDeferredMonitoring_CallID]
    ON [dbo].[BvPersonDeferredMonitoring];


GO
PRINT N'Creating [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_CallID]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_CallID]
    ON [dbo].[BvPersonDeferredMonitoring]([CallID] ASC) WHERE [CallID] IS NOT NULL;


GO
PRINT N'Update complete.';


GO
