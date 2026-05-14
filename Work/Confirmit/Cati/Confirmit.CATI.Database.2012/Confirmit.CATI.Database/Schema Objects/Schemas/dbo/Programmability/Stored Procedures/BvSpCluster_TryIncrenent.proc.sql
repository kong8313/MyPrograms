CREATE PROCEDURE [dbo].[BvSpCluster_TryIncrenent]
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

