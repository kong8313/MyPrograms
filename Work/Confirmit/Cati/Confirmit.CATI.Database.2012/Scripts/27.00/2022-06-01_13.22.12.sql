

GO
PRINT N'Altering [dbo].[Bv_ClusterQuotaService_GetCellNameQuery]...';


GO
ALTER PROCEDURE [dbo].[Bv_ClusterQuotaService_GetCellNameQuery]
	@surveyId INT,
	@cfQuotaName NVARCHAR(100),
	@tableAlias NVARCHAR(100),
	@query NVARCHAR(max) OUT
AS

CREATE TABLE #QuotaFields(
 Field NVARCHAR(100)
)

DECLARE @fields XML
SELECT @fields = XmlData FROM BvSurveyQuota WHERE SurveyID = @surveyId AND Name = @cfQuotaName

IF @fields IS NULL
BEGIN
	SET @query = ''
	RETURN
END

DECLARE @hdoc INT
EXEC sp_xml_preparedocument @hdoc OUTPUT, @fields
INSERT INTO #QuotaFields SELECT text AS Field
FROM OPENXML (@hdoc, '/QuotaData/FieldNames/string' , 2) WHERE nodetype=3
EXEC sp_xml_removedocument @hdoc

IF (SELECT COUNT(*) FROM #QuotaFields) = 0
BEGIN
	SET @query = ''
	RETURN
END


SELECT @query = STRING_AGG('''' + field + '='' + ISNULL( CAST( ' + @tableAlias + '.'+ field +' AS NVARCHAR(MAX)),'''' )',' + ') FROM #QuotaFields
RETURN
GO
PRINT N'Altering [dbo].[Bv_QuotaService_GetWhereForFilteredCell]...';


GO
ALTER PROCEDURE [dbo].[Bv_QuotaService_GetWhereForFilteredCell]
	@surveyId INT,
	@quotaId INT,
	@quotaCellId INT,
	@tableAlias NVARCHAR(100),
	@query NVARCHAR(max) OUT
AS

CREATE TABLE #FilterFields(
 Field NVARCHAR(100)
)
CREATE TABLE #CellFields(
 Field NVARCHAR(100),
 Value NVARCHAR(100)
)

INSERT INTO #FilterFields SELECT FieldName AS Field  FROM BvQuotaFilter WHERE surveyId = @surveyId

DECLARE @fields XML
SELECT @fields = XmlData FROM BvSurveyQuotaCell WHERE SurveyID=@surveyId AND QuotaID = @quotaId and CellID = @quotaCellId

IF @fields IS NULL
BEGIN
	SET @query = ''
	RETURN
END

DECLARE @hdoc INT
EXEC sp_xml_preparedocument @hdoc OUTPUT, @fields
INSERT INTO  #CellFields
SELECT * FROM OPENXML (@hdoc, 'QuotaCellData/FieldValues/QuotaCellFieldValue' , 2) 
WITH ( 
	Field NVARCHAR(100),
	Value NVARCHAR(100)
) 

EXEC sp_xml_removedocument @hdoc

SELECT @query = STRING_AGG('['+@tableAlias+'].['+cell.Field+'] = ''' + cell.Value+'''' ,' AND ')
FROM #CellFields AS cell JOIN #FilterFields AS filter
ON filter.field = cell.field

IF @query = NULL OR @query = ''
BEGIN 
SET @query = '1 = 1'
END

RETURN
GO
PRINT N'Altering [dbo].[Bv_ClusterQuotaService_GetCellIdQuery]...';


GO
ALTER PROCEDURE [dbo].[Bv_ClusterQuotaService_GetCellIdQuery]
	@surveyId INT,
	@cfQuotaName NVARCHAR(100),
	@tableAlias NVARCHAR(100),
	@query NVARCHAR(max) OUT
AS

DECLARE @cellName NVARCHAR(MAX) 

exec Bv_ClusterQuotaService_GetCellNameQuery @surveyId, @cfQuotaName, @tableAlias, @cellName output

IF @cellName = ''
BEGIN
	SET @query = '0'
	RETURN
END

SET @query = 'ISNULL( ( SELECT CellId FROM BvClusteredQuotaCell cqc WHERE cqc.SurveyId = ' + CAST(@surveyId AS NVARCHAR(20)) + ' AND cqc.Name = ' + @cellName + ' ), 0 )'
RETURN
GO
PRINT N'Refreshing [dbo].[BvSpPromoteCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPromoteCalls]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Activate]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Activate]';


GO
PRINT N'Refreshing [dbo].[BvSpSvySch_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvySch_Insert]';


GO
PRINT N'Update complete.';


GO
