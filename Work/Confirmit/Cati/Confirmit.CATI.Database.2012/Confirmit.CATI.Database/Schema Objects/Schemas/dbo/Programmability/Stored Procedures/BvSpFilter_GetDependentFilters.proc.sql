CREATE PROCEDURE [dbo].[BvSpFilter_GetDependentFilters]
@FilterSID INTEGER
AS
 SELECT DISTINCT BvFilters.Name, BvFilters.SID
    FROM BvFilters, BvFilterFields  
        WHERE BvFilterFields.FilterSID = BvFilters.SID
  AND BvFilterFields.[Sign] = 8 -- subfilter
        AND CAST( BvFilterFields.[Value] AS INTEGER ) = @FilterSID
RETURN (0)