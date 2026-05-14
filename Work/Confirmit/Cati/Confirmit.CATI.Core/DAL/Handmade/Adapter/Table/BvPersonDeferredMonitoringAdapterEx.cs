using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;

namespace Confirmit.CATI.Core.DAL.Handmade.Adapter.Table
{
    public static class BvPersonDeferredMonitoringAdapterEx
    {
        public static readonly string InsertSql = @"

INSERT INTO [dbo].[BvPersonDeferredMonitoring]
           ([PersonSID]
           ,[InterviewID]
           ,[SurveySID]
           ,[TimeStamp]
           ,[HasAudio]
           ,[EventsFile]
           ,[StartingFile]
           ,[IsRecording]
           ,[IsComplete]
           ,[ClientTimeUtc]
           ,[ServerTimeUtc]
           ,[CallID]
           ,[CallCenterId]
           ,[RespondentName]
           ,[TelephoneNumber]
           ,[InterviewDuration]
           ,[RecordCreationTime]
           ,[IsOldInterface]
)
     VALUES
           (@PersonSID,
            @InterviewID,
            @SurveySID,
            @TimeStamp,
            @HasAudio,
            @EventsFile,
            @StartingFile,
            @IsRecording,
            @IsComplete,
            @ClientTimeUtc,
            @ServerTimeUtc,
            @CallID,
            @CallCenterID,
            @RespondentName,
            @TelephoneNumber,
            @InterviewDuration,
            @RecordCreationTime,
            @IsOldInterface
);

SELECT SCOPE_IDENTITY();
";

        public static readonly string UpdateSql = @"
UPDATE dbo.[BvPersonDeferredMonitoring] SET
    [PersonSID] = @PersonSID,
    [InterviewID] = @InterviewID,
    [SurveySID] = @SurveySID,
    [TimeStamp] = @TimeStamp,
    [EventsFile] = @EventsFile,
    [StartingFile] = @StartingFile,
    [IsRecording] = @IsRecording,
    [IsComplete] = @IsComplete,
    [ClientTimeUtc] = @ClientTimeUtc,
    [ServerTimeUtc] = @ServerTimeUtc,
    [RequestAudio] = @RequestAudio,
    [InterviewDuration] = @InterviewDuration
    WHERE [ID]=@ID";

        public static BvPersonDeferredMonitoringPartEntity Insert(BvPersonDeferredMonitoringEntity entity)
        {
            var res = new BvPersonDeferredMonitoringPartEntity(entity);

            var disposableResources = new Stack<IDisposable>();

            try
            {
                var command = new SqlCommand();
                var connectionScope = new ConnectionScope();
                disposableResources.Push(connectionScope);
                command.Connection = connectionScope.Connection;

                if (DatabaseTransactionScope.Current != null)
                {
                    command.Transaction = DatabaseTransactionScope.Current.Transaction;
                }

                disposableResources.Push(command);

                command.CommandTimeout = Framework.Constants.DefaultDatabaseCommandTimeout;

                command.CommandType = CommandType.Text;
                command.CommandText = InsertSql;

                command.Parameters.Add(new SqlParameter("PersonSID", SqlDbType.Int)).Value = (object)entity.PersonSID ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("InterviewID", SqlDbType.Int)).Value = (object)entity.InterviewID ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("SurveySID", SqlDbType.Int)).Value = (object)entity.SurveySID ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("TimeStamp", SqlDbType.DateTime)).Value = (object)entity.TimeStamp ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("HasAudio", SqlDbType.Bit)).Value = (object)entity.HasAudio ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("EventsFile", SqlDbType.VarBinary)).Value = (object)entity.EventsFile ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("StartingFile", SqlDbType.NVarChar)).Value = (object)entity.StartingFile ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("IsRecording", SqlDbType.Bit)).Value = (object)entity.IsRecording ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("IsComplete", SqlDbType.Bit)).Value = (object)entity.IsComplete ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("ClientTimeUtc", SqlDbType.DateTime)).Value = (object)entity.ClientTimeUtc ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("ServerTimeUtc", SqlDbType.DateTime)).Value = (object)entity.ServerTimeUtc ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("CallID", SqlDbType.Int)).Value = (object)entity.CallID ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("CallCenterId", SqlDbType.Int)).Value = entity.CallCenterId;
                command.Parameters.Add(new SqlParameter("RespondentName", SqlDbType.NVarChar)).Value = (object)entity.RespondentName ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("TelephoneNumber", SqlDbType.VarChar)).Value = (object)entity.TelephoneNumber ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("InterviewDuration", SqlDbType.Int)).Value = (object)entity.InterviewDuration ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("RecordCreationTime", SqlDbType.DateTime)).Value = (object)entity.RecordCreationTime ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("IsOldInterface", SqlDbType.Bit)).Value = (object)entity.IsOldInterface ?? DBNull.Value;

                object o = command.ExecuteScalar();
                if (o == null) throw new Exception("BvPersonDeferredMonitoring isn't inserted.");
                res.ID = Convert.ToInt32(o);
            }
            finally
            {
                while (disposableResources.Count != 0)
                {
                    var resource2Dispose = disposableResources.Pop();

                    resource2Dispose.Dispose();
                }
            }

            return (res);
        }

        public static void Update([NotNull] BvPersonDeferredMonitoringEntity entity)
        {
            var disposableResources = new Stack<IDisposable>();

            try
            {
                var command = new SqlCommand();

                var connectionScope = new ConnectionScope();
                disposableResources.Push(connectionScope);
                command.Connection = connectionScope.Connection;

                if (DatabaseTransactionScope.Current != null)
                {
                    command.Transaction = DatabaseTransactionScope.Current.Transaction;
                }

                disposableResources.Push(command);

                command.CommandTimeout = Framework.Constants.DefaultDatabaseCommandTimeout;

                command.CommandType = CommandType.Text;

                command.CommandText = UpdateSql;

                command.Parameters.Add(new SqlParameter("ID", SqlDbType.Int)).Value = (object)entity.ID ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("PersonSID", SqlDbType.Int)).Value = (object)entity.PersonSID ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("InterviewID", SqlDbType.Int)).Value = (object)entity.InterviewID ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("SurveySID", SqlDbType.Int)).Value = (object)entity.SurveySID ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("TimeStamp", SqlDbType.DateTime)).Value = (object)entity.TimeStamp ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("EventsFile", SqlDbType.VarBinary)).Value = (object)entity.EventsFile ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("StartingFile", SqlDbType.NVarChar)).Value = (object)entity.StartingFile ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("IsRecording", SqlDbType.Bit)).Value = (object)entity.IsRecording ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("IsComplete", SqlDbType.Bit)).Value = (object)entity.IsComplete ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("ClientTimeUtc", SqlDbType.DateTime)).Value = (object)entity.ClientTimeUtc ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("ServerTimeUtc", SqlDbType.DateTime)).Value = (object)entity.ServerTimeUtc ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("RequestAudio", SqlDbType.Bit)).Value = (object)entity.RequestAudio ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("InterviewDuration", SqlDbType.Int)).Value = (object)entity.InterviewDuration ?? DBNull.Value;
                command.Parameters.Add(new SqlParameter("IsOldInterface", SqlDbType.Int)).Value = (object)entity.IsOldInterface ?? DBNull.Value;

                command.ExecuteScalar();
            }
            finally
            {
                while (disposableResources.Count != 0)
                {
                    var resource2Dispose = disposableResources.Pop();

                    resource2Dispose.Dispose();
                }
            }
        }

        public static void UpdateHasAudioAndRequestAudio(int id, bool hasAudio, bool requestAudio)
        {
            new DatabaseEngine().ExecuteNonQuery(
                @"
UPDATE [dbo].[BvPersonDeferredMonitoring]
SET [HasAudio] = @HasAudio,
    [RequestAudio]=@RequestAudio
WHERE ID=@ID
",
                CommandType.Text,
                new[] {
                    new SqlParameter("@ID", id),
                    new SqlParameter("@HasAudio", hasAudio),
                    new SqlParameter("@RequestAudio", requestAudio)
                }
            );

        }

        public static void UpdateIsComplete(int id, bool isComplete)
        {
            new DatabaseEngine().ExecuteNonQuery(
                @"
UPDATE [dbo].[BvPersonDeferredMonitoring]
SET [IsComplete] = @IsComplete
WHERE ID=@ID
",
                CommandType.Text,
                new[] {
                    new SqlParameter("@ID", id),
                    new SqlParameter("@IsComplete", isComplete?1:0)
                }
             );

        }

        public static void UpdateIsRecording(int id, bool isRecording)
        {
            new DatabaseEngine().ExecuteNonQuery(
                @"
UPDATE [dbo].[BvPersonDeferredMonitoring]
SET [IsRecording] = @IsRecording
WHERE ID=@ID
",
                CommandType.Text,
                new[] {
                    new SqlParameter("@ID", id),
                    new SqlParameter("@IsRecording", isRecording?1:0)
                }
             );

        }

        public static void AppendToEventsFile(int id, byte[] packet)
        {
            new DatabaseEngine().ExecuteNonQuery(
                @"
UPDATE [dbo].[BvPersonDeferredMonitoring]
SET [EventsFile].Write(@EventsFile, null, 0)
WHERE ID=@ID
",
                CommandType.Text,
                new[] {
                    new SqlParameter("@ID", id),
                    new SqlParameter("@EventsFile", packet)
                }
             );

        }

        public static void CompleteDeferredMonitoringRecord(int id, byte[] packet, bool hasAudio, bool requestAudio, int interviewDuration)
        {
            new DatabaseEngine().ExecuteNonQuery(
                @"
UPDATE [dbo].[BvPersonDeferredMonitoring]
SET [EventsFile].Write(@EventsFile, null, 0),
    [IsComplete]=1,
    [HasAudio]=@HasAudio,
    [RequestAudio]=@RequestAudio,
    [IsRecording]=0,
    [interviewDuration]=@InterviewDuration
WHERE ID=@ID
",
                CommandType.Text,
                new[] {
                    new SqlParameter("@ID", id),
                    new SqlParameter("@EventsFile", packet),
                    new SqlParameter("@HasAudio", hasAudio?1:0),
                    new SqlParameter("@RequestAudio", requestAudio?1:0),
                    new SqlParameter("@InterviewDuration", interviewDuration)
                }
             );
        }

        public static void CompleteDeferredMonitoringRecord(int id, DateTime endUtcTime)
        {
            new DatabaseEngine().ExecuteNonQuery(
                @"UPDATE [dbo].[BvPersonDeferredMonitoring] SET 
                        [CallID] = NULL, 
                        [InterviewDuration] = DATEDIFF( SECOND, RecordCreationTime, @EndUtcTime ),
                        [IsRecording] = 0,
                        [IsComplete] = 1
                    WHERE [ID] = @ID AND [IsComplete] = 0 AND EventsFile IS NOT NULL AND EventsFile <> 0x",
                CommandType.Text,
                new[] {
                    new SqlParameter("@ID", id),
                    new SqlParameter("@EndUtcTime", endUtcTime)
                }
            );
        }

        public static bool AreThereAnyNonEmptyMonitoringRecords(int interviewerId, int monitoringRecordId)
        {
            var o = new DatabaseEngine().ExecuteScalar<object>(
                @"
SELECT count([ID]) FROM [dbo].[BvPersonDeferredMonitoring]
WHERE (PersonSID=@PersonSID) AND (ID=@ID) AND [ClientTimeUtc]<>@EmptyDate
",
                CommandType.Text,
                new[] {
                        new SqlParameter("@PersonSID", interviewerId),
                        new SqlParameter("@ID", monitoringRecordId),
                        new SqlParameter("@EmptyDate", SqlDateTime.MinValue.Value)
                }
             );
            if (o is int)
            {
                return ((int)o > 0);
            }
            return false;
        }

        public static void UpdateExtendedStatusAndClearCallId(int id, int? extendedStatus)
        {
            new DatabaseEngine().ExecuteNonQuery(
                @"
UPDATE [dbo].[BvPersonDeferredMonitoring]
SET [ExtendedStatus]=@ExtendedStatus,
    [CallID] = NULL
WHERE ID=@ID
",
                CommandType.Text,
                new[] {
                    new SqlParameter("@ID", id),
                    new SqlParameter("@ExtendedStatus", extendedStatus)
                }
             );
        }

        public static void ClearCallId(int deferredRecordId)
        {
            new DatabaseEngine().ExecuteNonQuery(
                @"
UPDATE [dbo].[BvPersonDeferredMonitoring]
SET [CallID] = NULL
WHERE ID=@ID
",
                CommandType.Text,
                new[] {
                    new SqlParameter("@ID", deferredRecordId)
                }
             );
        }

        public static void RemoveRecord(int deferredRecordId)
        {
            new DatabaseEngine().ExecuteNonQuery(
                @"
DELETE FROM [dbo].[BvPersonDeferredMonitoring]
WHERE ID=@ID
",
                CommandType.Text,
                new[] {
                    new SqlParameter("@ID", deferredRecordId)
                }
            );
        }
    }
}
