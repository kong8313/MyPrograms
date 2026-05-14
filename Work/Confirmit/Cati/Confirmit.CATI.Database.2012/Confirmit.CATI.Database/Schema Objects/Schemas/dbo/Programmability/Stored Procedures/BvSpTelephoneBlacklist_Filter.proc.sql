CREATE PROCEDURE [dbo].[BvSpTelephoneBlacklist_Filter]
  @TelephoneNumbers BvStringArrayType READONLY
AS
SELECT t.Value AS TelephoneNumber, f.IsFiltered as IsFiltered 
	FROM @TelephoneNumbers AS t 
	CROSS APPLY [dbo].[BvFnBlacklist_IsTelephoneNumberFiltered]([dbo].RemoveNonNumericCharacters(Value)) AS f