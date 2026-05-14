CREATE PROCEDURE [dbo].[Bv_ClusterQuotaService_GetCellIdQuery]
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