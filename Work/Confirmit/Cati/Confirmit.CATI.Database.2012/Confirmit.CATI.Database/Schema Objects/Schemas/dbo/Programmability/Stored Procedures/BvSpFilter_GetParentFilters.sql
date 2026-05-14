CREATE PROCEDURE [dbo].[BvSpFilter_GetParentFilters]
@ObjectSID INTEGER
AS
SET NOCOUNT ON
DECLARE @FilterSID INTEGER
DECLARE @Rows INTEGER

 IF @ObjectSID IS NULL
 BEGIN
 /* Looks like we're generating code using FMTONLY. So lets return metadata*/
	SELECT 0 as [SID]
    RETURN 0;
 END
 
    CREATE TABLE #temp (
        SID [int] NOT NULL
    )

    CREATE TABLE #look(
        SID [int] NOT NULL
    )

    CREATE TABLE #find(
        SID [int] NOT NULL
    )

    INSERT INTO #look 
        SELECT BvFilters.SID
        FROM BvFilters, BvFilterFields 
        WHERE BvFilterFields.FilterSID = BvFilters.SID
              AND BvFilterFields.[Sign] = 8 -- subfilter
              AND CAST( BvFilterFields.[Value] AS INTEGER ) = @ObjectSID

    INSERT INTO #find SELECT SID FROM #look

    SET @Rows = @@ROWCOUNT

    WHILE @Rows <> 0
    BEGIN
       INSERT INTO #temp 
           SELECT BvFilters.SID
           FROM BvFilterFields, #look, BvFilters
           WHERE BvFilterFields.FilterSID = BvFilters.SID
              AND BvFilterFields.[Sign] = 8 -- subfilter
              AND CAST( BvFilterFields.[Value] AS INTEGER ) = #look.SID
              AND BvFilters.SID NOT IN
              ( SELECT SID FROM #find )
       SET @Rows = @@ROWCOUNT

       INSERT INTO #find SELECT SID FROM #temp
       
       TRUNCATE TABLE #look

       INSERT INTO #look SELECT SID FROM #temp

       TRUNCATE TABLE #temp
    END

    DROP TABLE #temp
    DROP TABLE #look

    SELECT SID FROM #find

RETURN (0)