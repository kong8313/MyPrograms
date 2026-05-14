CREATE PROCEDURE [dbo].[Bv_ClusterQuotaService_GetCellNameQuery]
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