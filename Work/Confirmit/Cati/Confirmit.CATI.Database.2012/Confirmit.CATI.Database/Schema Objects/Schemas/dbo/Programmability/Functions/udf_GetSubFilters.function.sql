CREATE FUNCTION dbo.udf_GetSubFilters
(
    @FilterSid INT
)
RETURNS TABLE
AS RETURN(
    WITH SubFilters AS(
		--initialization
		SELECT SID
		FROM BvFilters
		WHERE SID = @FilterSid
		
		UNION ALL
		
		--recursive execution
		SELECT CAST( CASE WHEN bff.Sign = 8 THEN [Value] ELSE 0 END AS INT ) SubFilterSid
		FROM BvFilterFields bff 
		INNER JOIN SubFilters sf ON bff.FilterSid = sf.SID
		WHERE bff.Sign = 8 --sub filter
	)
	SELECT DISTINCT * FROM SubFilters  
)