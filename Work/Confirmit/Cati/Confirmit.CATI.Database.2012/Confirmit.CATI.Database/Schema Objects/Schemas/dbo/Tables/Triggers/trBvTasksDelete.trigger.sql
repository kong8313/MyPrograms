CREATE TRIGGER trBvTasksDelete ON BvTasks
AFTER DELETE
AS
 DELETE FROM BvPersonMonitoring
  WHERE PersonSID IN ( SELECT PersonSID FROM deleted )