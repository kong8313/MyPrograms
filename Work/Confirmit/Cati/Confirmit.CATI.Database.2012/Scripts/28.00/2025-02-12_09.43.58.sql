GO
PRINT N'Altering Procedure [dbo].[BvSpCfUpdateSurveyReplicationStatus]...';


GO
ALTER PROCEDURE [dbo].[BvSpCfUpdateSurveyReplicationStatus]
	@ProjectId NVARCHAR( 255 ),
	@IsReplicationEnabled BIT
AS
	
	
RETURN 0
GO
PRINT N'Update complete.';


GO
PRINT N'Set ReplicationStatus=1 for all Surveys.';

UPDATE BvSurvey SET ReplicationStatus = 1

PRINT N'Update complete.';