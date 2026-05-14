CREATE PROCEDURE [dbo].[SetDialerSurveyParametersWhereIsNull]
	@dialerParameters nvarchar(max) 
AS
	UPDATE BvSurvey set DialerParameters = @dialerParameters
		WHERE DialerParameters IS NULL
RETURN 0