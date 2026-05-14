CREATE PROCEDURE [dbo].[BvSpFilter_MoveToSurvey]
   @SourceSurveySid INT,
   @TargetSurveySid INT
AS 

UPDATE [BvFilters]
SET SurveySID = @TargetSurveySid
WHERE SurveySID = @SourceSurveySid