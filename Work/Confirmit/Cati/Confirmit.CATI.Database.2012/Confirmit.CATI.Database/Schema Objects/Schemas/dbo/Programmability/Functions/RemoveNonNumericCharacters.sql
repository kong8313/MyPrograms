CREATE FUNCTION dbo.RemoveNonNumericCharacters (@strText VARCHAR(1000))
    RETURNS VARCHAR(1000)
AS
BEGIN
    IF @strText IS NULL RETURN NULL;
    
    DECLARE @result VARCHAR(1000) = '';

    ;WITH N AS (
        SELECT TOP (LEN(@strText)) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
        FROM sys.objects
    )
     SELECT @result = @result + SUBSTRING(@strText, n, 1)
     FROM N
     WHERE SUBSTRING(@strText, n, 1) LIKE '[0-9]';

    RETURN @result;
END
