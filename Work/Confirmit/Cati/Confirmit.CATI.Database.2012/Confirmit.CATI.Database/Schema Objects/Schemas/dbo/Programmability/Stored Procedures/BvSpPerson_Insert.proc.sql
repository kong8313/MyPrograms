CREATE PROCEDURE [dbo].[BvSpPerson_Insert]
        @SID INT, 
        @Name NVARCHAR( 255 ),  
        @FullName NVARCHAR( 255 ),
        @Description NVARCHAR( 255 ),
        @ManualSelection INT,
        @AssignmentsListMode INT,
        @PwdSaltTxt NVARCHAR(256),
		@CallGroupId INT,
		@CallCenterID INT,
		@Location NVARCHAR(256),
		@DialTypeId TINYINT,
		@Type TINYINT,
        @EnableSoftphoneIntegration BIT,
        @PasswordNeedsChange BIT = 0,
		@Attribute1 NVARCHAR(50) = '',
		@Attribute2 NVARCHAR(50) = '',
		@Attribute3 NVARCHAR(50) = '',
		@Attribute4 NVARCHAR(50) = '',
		@Attribute5 NVARCHAR(50) = ''
AS

IF (EXISTS(SELECT 1 FROM BvPerson WHERE [Name]=@Name))
BEGIN
    RAISERROR( 'Person with name %s already exists', 12, 1, @Name )
    RETURN -1
END

INSERT  BvPerson( 
        SID,
        [Name], 
        FullName,
        [Description],
        ManualSelection, 
        AssignmentsListMode,
        PwdSaltTxt,
		CallGroupID,
		CallCenterID,
        Location,
		DialTypeId,
		Type,
        EnableSoftphoneIntegration,
        PasswordNeedsChange,
		Attribute1,
		Attribute2,
		Attribute3,
		Attribute4,
		Attribute5)
    VALUES ( 
        @SID,
        @Name, 
        @FullName,
        @Description,
        @ManualSelection,
        @AssignmentsListMode, 
        @PwdSaltTxt,
		@CallGroupId,
		@CallCenterID,
        @Location,
		@DialTypeId,
		@Type,
        @EnableSoftphoneIntegration,
        @PasswordNeedsChange,
		@Attribute1,
		@Attribute2,
		@Attribute3,
		@Attribute4,
		@Attribute5)

INSERT BvPersonFailedLoginAttempts( PersonId, Count ) VALUES( @SID, 0 )

RETURN 0