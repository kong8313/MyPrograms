CREATE PROCEDURE [dbo].[BvSpCallCenter_Insert]
	@Name NVARCHAR(MAX),
	@Description NVARCHAR(MAX),
	@LocalTimezoneId INT,
	@DialerId INT,
	@HidePii BIT = 0
AS

	DECLARE @Count INT = (SELECT COUNT(*) FROM BvCallCenter )
	IF @Count >= 255
	BEGIN
		RAISERROR( 'Count of call centers can''t be greater 255', 12, 1 )
		RETURN 0
	END

	INSERT INTO BvCallCenter( Name, Description, LocalTimezoneId, DialerId, HidePii ) VALUES( @Name, @Description, @LocalTimezoneId, @DialerId, @HidePii )

	RETURN SCOPE_IDENTITY()
