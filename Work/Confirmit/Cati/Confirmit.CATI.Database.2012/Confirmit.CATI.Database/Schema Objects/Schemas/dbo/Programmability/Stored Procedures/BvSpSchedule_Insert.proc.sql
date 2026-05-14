CREATE PROCEDURE [dbo].[BvSpSchedule_Insert]
       @ScheduleID INT,
       @Name NVARCHAR(255),
       @XmlUnderDev NVARCHAR(MAX),
       @ScriptSource NVARCHAR(MAX),
       @DesignStateGroupID INT
AS

INSERT INTO BvSchedule (
       [ScheduleID],
       [Name],
       [CreateDate],
       [ModifyDate],
       [XmlUnderDev],
       [ScriptSource],
       [DesignStateGroupID] )
    VALUES (
       @ScheduleID,
       @Name,
       GETUTCDATE(),
       GETUTCDATE(),
       @XmlUnderDev,
       @ScriptSource,
       @DesignStateGroupID )