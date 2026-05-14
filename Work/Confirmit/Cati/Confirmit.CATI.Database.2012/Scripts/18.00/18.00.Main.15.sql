PRINT N'Deleting data from the [BvAsyncOperationQueue] table to add new [CallCenterId] column';

GO
DELETE FROM [BvAsyncOperationQueue]

GO
PRINT N'Add [CallCenterId] column';

GO
ALTER TABLE [BvAsyncOperationQueue] ADD [CallCenterId] INT NOT NULL CONSTRAINT __Temp__DF_BvAsyncOperationQueue_CallCenterID DEFAULT (0);
ALTER TABLE [dbo].[BvAsyncOperationQueue] DROP CONSTRAINT __Temp__DF_BvAsyncOperationQueue_CallCenterID


