CREATE TABLE BvUserNotification 
(
    Id INT IDENTITY(1,1) NOT NULL,
    Type INT NOT NULL, -- value of UserNotificationType enum
    ObjectId INT NOT NULL, -- It will surveySid, PersonSid or something else and will depend from ObjectType
    SendDate DATETIME NOT NULL,
    Subject NVARCHAR(MAX) NOT NULL,
    Body NVARCHAR(MAX)

)