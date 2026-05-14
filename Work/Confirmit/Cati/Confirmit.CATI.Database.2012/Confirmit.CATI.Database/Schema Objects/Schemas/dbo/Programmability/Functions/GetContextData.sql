CREATE FUNCTION [dbo].[GetContextData]( )
RETURNS @Context TABLE(ITS SMALLINT, OperationId INT, OperationType TINYINT, CallCenterId INT, DialingMode TINYINT) 
AS
BEGIN

DECLARE @contextStr NVARCHAR(MAX)

DECLARE @commaPos1 INT 
DECLARE @commaPos2 INT
DECLARE @commaPos3 INT
DECLARE @commaPos4 INT

SET @contextStr = RTRIM(REPLACE(CONVERT(VARCHAR(128),CONTEXT_INFO()), CHAR(0), CHAR(32) )); 

SET @commaPos1 = CHARINDEX(',', @contextStr) 
SET @commaPos2 = CHARINDEX(',', @contextStr, @commaPos1 + 1)
SET @commaPos3 = CHARINDEX(',', @contextStr, @commaPos2 + 1)
SET @commaPos4 = CHARINDEX(',', @contextStr, @commaPos3 + 1)

INSERT INTO @Context
SELECT	SUBSTRING(@contextStr, 1, @commaPos1 - 1), 
		SUBSTRING(@contextStr, @commaPos1+1, @commaPos2 - @commaPos1 - 1),
		SUBSTRING(@contextStr, @commaPos2+1, @commaPos3 - @commaPos2 - 1),
		SUBSTRING(@contextStr, @commaPos3+1, @commaPos4 - @commaPos3 - 1),	
		SUBSTRING(@contextStr, @commaPos4+1, len(@contextStr) - @commaPos4)	

RETURN
END