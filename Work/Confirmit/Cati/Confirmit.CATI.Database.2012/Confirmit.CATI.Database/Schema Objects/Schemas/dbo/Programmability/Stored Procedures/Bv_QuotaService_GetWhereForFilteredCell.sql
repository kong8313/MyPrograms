CREATE PROCEDURE [dbo].[Bv_QuotaService_GetWhereForFilteredCell]
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