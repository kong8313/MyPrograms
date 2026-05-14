PRINT N'Altering [dbo].[BvSpCluster_TryIncrenent]...';


GO
ALTER PROCEDURE [dbo].[BvSpCluster_TryIncrenent]
@SurveyId INT, 
@CallId INT,
@Force BIT
AS

DECLARE @CellId INT = (SELECT CellId FROM BvSvySchedule WHERE ID = @CallId )
DECLARE @Result BIT = 1

UPDATE BvClusteredQuotaCell
	SET LiveCount = LiveCount + 1
	WHERE SurveyId = @SurveyId AND CellId = @CellId AND ( LiveCount < LiveLimit OR @Force = 1 )


IF @@ROWCOUNT = 0
BEGIN
	IF EXISTS( SELECT 1 FROM BvClusteredQuotaCell WHERE SurveyId = @SurveyId AND CellId = @CellId )
	BEGIN
		SET @Result = 0
	END
END

RETURN @Result
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Update complete.';


GO
