GO
PRINT N'Altering [dbo].[BvActiveDial]...';


GO
ALTER TABLE [dbo].[BvActiveDial]
    ADD [TransferType] TINYINT NULL;


GO
PRINT N'Refreshing [dbo].[BvSpActiveDial_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpActiveDial_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpActiveDial_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpActiveDial_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpActiveDial_InsertOutboundBatch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpActiveDial_InsertOutboundBatch]';


GO
PRINT N'Refreshing [dbo].[BvSpActiveDial_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpActiveDial_Update]';


GO
PRINT N'Update complete.';


GO
