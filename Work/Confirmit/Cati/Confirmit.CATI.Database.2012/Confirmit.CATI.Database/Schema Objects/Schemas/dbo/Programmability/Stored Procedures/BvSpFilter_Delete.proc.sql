CREATE PROCEDURE [dbo].[BvSpFilter_Delete] 
@SID    INTEGER
AS

    DECLARE @SurveySID INT
    SELECT @SurveySID = SurveySID
    FROM BvFilters
    WHERE SID = @SID
    
    DECLARE @changedFilters TABLE(SID INT)

    DELETE FROM BvFilterFields 
    OUTPUT DELETED.FilterSid
    INTO @changedFilters
    WHERE [Sign] = 8 AND 
          CAST( Value AS INTEGER ) = @SID
    
    IF(@SurveySID > 0)
       UPDATE BvFilters
       SET SurveySID = 0
       FROM @changedFilters changes
       CROSS APPLY dbo.udf_GetParentFilters(changes.SID) parentFilters
       WHERE BvFilters.SID = parentFilters.SID AND
		     NOT EXISTS( SELECT 1
		                 FROM dbo.udf_GetSubFilters(parentFilters.SID) subFilters
		                 INNER JOIN BvFilters f ON f.SID = subFilters.SID
		                 INNER JOIN BvFilterFields ff ON ff.FilterSid = f.Sid AND
		                                                  ff.[Table] = 512 --cf table
		                 WHERE f.SID != @SID)
    
    DELETE FROM BvFilterFields WHERE FilterSID = @SID    
    
    DELETE FROM BvFilters WHERE SID = @SID

RETURN(0)