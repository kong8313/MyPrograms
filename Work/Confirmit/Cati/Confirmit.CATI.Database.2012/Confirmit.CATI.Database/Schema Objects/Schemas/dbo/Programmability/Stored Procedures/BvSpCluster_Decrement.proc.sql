CREATE PROCEDURE [dbo].[BvSpCluster_Decrement]
@SurveyId INT, 
@CellId INT
AS
UPDATE BvClusteredQuotaCell
	SET LiveCount = LiveCount - 1
	WHERE SurveyId = @SurveyId AND CellId = @CellId 

