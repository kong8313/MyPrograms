CREATE PROCEDURE [dbo].[BvSpFilter_CheckSurveyMismatch]
@FilterSID    INTEGER,
@SubFilterSID    INTEGER
AS
SET NOCOUNT ON
        
	DECLARE @Ret  INT

    SELECT @Ret = COUNT( DISTINCT SurveySID )
    FROM BvFilters f
    LEFT JOIN dbo.udf_GetSubFilters(@FilterSID) subFilters ON f.SID = subFilters.SID
    LEFT JOIN dbo.udf_GetSubFilters(@SubFilterSID) subFiltersForSubFilter ON f.SID = subFiltersForSubFilter.SID
    LEFT JOIN dbo.udf_GetParentFilters(@FilterSID) parentFilters ON f.SID = parentFilters.SID
    WHERE SurveySID != 0 AND
          ( subFilters.SID IS NOT NULL OR subFiltersForSubFilter.SID IS NOT NULL OR parentFilters.SID IS NOT NULL)
 
    IF @Ret > 1
        SET @Ret = 1
    ELSE
        SET @Ret = 0

RETURN (@Ret)