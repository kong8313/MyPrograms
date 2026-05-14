CREATE PROCEDURE [dbo].[BvSpState_ListBySurvey]
	@SurveySID int
AS

SELECT [StateID], [Name], [Priority], [DA], [FcdAction], [AaporCode] FROM [BvState]
     WHERE [StateGroupID] = (
		SELECT [StateGroupID] FROM [BvSurvey] WHERE [SID] = @SurveySID )
     ORDER BY [StateID]