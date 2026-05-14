CREATE PROCEDURE [dbo].[BvSpFilter_InsertField]
@FilterSID    INTEGER,
@Table        INTEGER,
@Column       NVARCHAR(255),
@Type         INTEGER,
@Sign         INTEGER,
@Value        NVARCHAR(255),
@IsNeedCast BIT
AS
DECLARE @Ret INTEGER
DECLARE @f1 NVARCHAR(255)
DECLARE @f2 NVARCHAR(255)

    IF @Sign = 8 -- subfilter
    BEGIN
        DECLARE @Find         INTEGER
        DECLARE @SubFilterSID INTEGER

        SET @SubFilterSID = CAST( @Value AS INTEGER )

        IF NOT EXISTS ( SELECT * FROM BvFilters WHERE SID = @SubFilterSID )
        BEGIN
            RAISERROR( N'Filter with SID = %u not found.', 16, 1, @SubFilterSID )
            RETURN(-1)
        END
 
        EXEC @Find = BvSpFilter_CheckCircle @FilterSID, @SubFilterSID
        IF @Find <> 0
        BEGIN
            SELECT @f1 = [Name] FROM BvFilters WHERE SID = @SubFilterSID
            SELECT @f2 = [Name] FROM BvFilters WHERE SID = @FilterSID

            RAISERROR( N'Cannot insert subfilter %s into filter %s : circular reference found.', 12, 1, @f1, @f2 )
            RETURN (-1)
        END

        EXEC @Find = BvSpFilter_CheckSurveyMismatch @FilterSID, @SubFilterSID
        IF @Find <> 0
        BEGIN
            SELECT @f1 = [Name] FROM BvFilters WHERE SID = @SubFilterSID
            SELECT @f2 = [Name] FROM BvFilters WHERE SID = @FilterSID

            RAISERROR( N'Cannot insert subfilter %s into filter %s because it is used for another survey(s).', 12, 1, @f1, @f2 )
            RETURN (-1)
        END
        
        DECLARE @SurveySID INT
        SELECT @SurveySID = SurveySID 
        FROM BvFilters
        WHERE SID = @SubFilterSID
        
        IF @SurveySID > 0
			UPDATE BvFilters
			SET SurveySID = @SurveySID
			FROM dbo.udf_GetParentFilters(@FilterSID) parentFilters
			WHERE parentFilters.SID = BvFilters.SID
    END

    INSERT INTO BvFilterFields( [FilterSID],
        [Table],
        [Column],
        [Type],
        [Sign],
        [Value],
        IsNeedCast )
    VALUES( @FilterSID,
            @Table,
            @Column,
            @Type,
            @Sign,
            @Value,
            @IsNeedCast )
 
    SET @Ret = @@IDENTITY

RETURN ( @Ret )