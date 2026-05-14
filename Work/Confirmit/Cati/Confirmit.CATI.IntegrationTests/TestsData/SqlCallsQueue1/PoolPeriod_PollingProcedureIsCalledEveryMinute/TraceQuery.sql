-- Create a Queue
declare @rc int
declare @TraceID int
declare @maxfilesize bigint
set @maxfilesize = 5 

declare @currentTime datetime;
declare @stopTime datetime;

set @currentTime = (select GETDATE())
set @stopTime = DATEADD(minute, {1}, @currentTime)

exec @rc = sp_trace_create @TraceID output, 2, N'{0}', @maxfilesize, @stopTime
if (@rc != 0) RAISERROR( 'Internal error occured', 16, 1 ) 

-- Set the events
declare @on bit
set @on = 1
exec sp_trace_setevent @TraceID, 10, 34, @on
exec sp_trace_setevent @TraceID, 10, 12, @on

-- Set the Filters
declare @intfilter int
declare @bigintfilter bigint

-- Set the trace status to start
exec sp_trace_setstatus @TraceID, 1