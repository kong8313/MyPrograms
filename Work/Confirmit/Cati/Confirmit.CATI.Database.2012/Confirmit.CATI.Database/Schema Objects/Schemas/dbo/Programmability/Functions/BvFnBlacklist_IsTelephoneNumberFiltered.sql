CREATE FUNCTION BvFnBlacklist_IsTelephoneNumberFiltered( @TelephoneNumber VARCHAR(255))
RETURNS TABLE
AS
	RETURN SELECT CASE WHEN EXISTS( select top(1) 1 as Filtered FROM BvTelephoneBlacklist WHERE Type = 1 /*StartWith*/ AND TelephoneNumber BETWEEN  SUBSTRING(@TelephoneNumber, 0, 1) AND @TelephoneNumber AND TelephoneNumber = SUBSTRING( @TelephoneNumber, 0, LEN(TelephoneNumber) + 1 ) ORDER BY TelephoneNumber DESC ) OR 
							EXISTS( select 1 FROM BvTelephoneBlacklist WHERE Type = 0 /*Equal*/ AND TelephoneNumber = @TelephoneNumber ) THEN 1 ELSE 0 END as IsFiltered
GO
