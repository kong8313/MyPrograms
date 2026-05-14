CREATE FUNCTION dbo.udf_GetParentFilters
(
    @FilterSid INT
)
RETURNS TABLE
AS RETURN(
    WITH ParentFilters AS(
		--initialization
		SELECT SID
		FROM BvFilters
		WHERE SID = @FilterSid
		
		UNION ALL
		
		--recursive execution
		SELECT bff.FilterSid
		FROM BvFilterFields bff 
		INNER JOIN ParentFilters pf ON CAST(bff.Value AS INT) = pf.SID
		WHERE bff.Sign = 8 --sub filter
	)
	SELECT DISTINCT * FROM ParentFilters
)