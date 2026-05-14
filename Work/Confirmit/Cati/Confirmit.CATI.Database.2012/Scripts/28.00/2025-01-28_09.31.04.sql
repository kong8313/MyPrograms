GO
PRINT N'Altering Procedure [dbo].[BvSpGetReplicatedTable]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetReplicatedTable]
AS
    DECLARE @EnableChangeTracking BIT = 1

	SELECT tables.ID AS TableID,
           survey.CfDbSchemaPath,
           tables.TableName,
           tables.PrimaryKey,
           tables.LastVersion,
           survey.DestinationTableName,
           survey.ReplicationStatus,
           survey.SID AS SurveySid,
           survey.Name AS ProjectId
    FROM BvReplicationTables tables
    INNER JOIN BvSurvey survey ON survey.SID = tables.SurveySid AND
                                  survey.ReplicationStatus = @EnableChangeTracking AND
                                  survey.State != 2
RETURN 0
GO
PRINT N'Update complete.';


GO
