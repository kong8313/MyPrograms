CREATE PROCEDURE [dbo].[BvSpFilter_Update]
@SID           INTEGER,
@Name          NVARCHAR(255),
@Description   NVARCHAR(255),
@AndOrOperator TINYINT,
@SurveySID     INTEGER
AS
    IF EXISTS ( SELECT * FROM BvFilters WHERE [Name] = @Name AND 
        [SID] <> @SID)
    BEGIN
        RAISERROR( N'Filter with name %s already exists', 12, 1, @Name )
        RETURN (-1)
    END
    
    IF @SurveySID > 0
    
		BEGIN		
			IF EXISTS( SELECT 1
					   FROM BvFilters f
					   LEFT JOIN dbo.udf_GetSubFilters(@SID) subFilters ON subFilters.SID = f.SID
					   LEFT JOIN dbo.udf_GetParentFilters(@SID) parentFilters ON parentFilters.SID = f.SID
					   WHERE f.SurveySID > 0 AND
							 f.SurveySID != @SurveySID AND
							 ( subFilters.SID IS NOT NULL OR
							   parentFilters.SID IS NOT NULL ) )
			BEGIN
				RAISERROR( N'Cannot update filter %s because it is used for another survey(s).', 12, 1, @Name )
				RETURN (-1)
			END
			
			UPDATE BvFilters
			SET SurveySID = @SurveySID
			FROM dbo.udf_GetParentFilters(@SID) pe
			WHERE BvFilters.SID = pe.SID
		END
	
	ELSE	
		BEGIN
		
			UPDATE BvFilters
			SET SurveySID = 0
			FROM dbo.udf_GetParentFilters(@SID) parentFilters
			WHERE BvFilters.SID = parentFilters.SID AND
				  NOT EXISTS( SELECT 1
							  FROM dbo.udf_GetSubFilters(parentFilters.SID) subFilters
							  INNER JOIN BvFilters f ON f.SID = subFilters.SID
							  INNER JOIN BvFilterFields ff ON ff.FilterSid = f.Sid AND
															  ff.[Table] = 512 --cf table
							  WHERE f.SID != @SID)
		END
		                  
	UPDATE BvFilters 
		SET    [Name] = @Name,
			   [Description] = @Description,
			   [AndOrOperator] = @AndOrOperator,
			   [SurveySID] = @SurveySID
		WHERE [SID] = @SID
		                  
RETURN (0)