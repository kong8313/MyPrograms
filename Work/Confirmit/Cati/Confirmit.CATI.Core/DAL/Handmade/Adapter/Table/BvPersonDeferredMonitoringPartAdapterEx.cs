using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.DAL.Handmade.Adapter.Table
{
    public class BvPersonDeferredMonitoringPartAdapterEx
    {
        public static readonly string SelectSql = @"
            SELECT [ID]
                ,[PersonSID]
                ,[InterviewID]
                ,[SurveySID]
                ,[TimeStamp]
                ,[RecordCreationTime]
                ,[IsRecording]
                ,[IsComplete]
                ,[ClientTimeUtc]
                ,[ServerTimeUtc]
                ,[CallID]
                ,[ExtendedStatus]
                ,[InterviewDuration]
            FROM [dbo].[BvPersonDeferredMonitoring]
        ";

        private static List<BvPersonDeferredMonitoringPartEntity> ReadList(IDataReader rd)
        {
            var bvPersonDeferredMonitoringEntityList = new List<BvPersonDeferredMonitoringPartEntity>();

            int idOrdinal = rd.GetOrdinal("ID");
            int personSidOrdinal = rd.GetOrdinal("PersonSID");
            int interviewIdOrdinal = rd.GetOrdinal("InterviewID");
            int surveySidOrdinal = rd.GetOrdinal("SurveySID");
            int timeStampOrdinal = rd.GetOrdinal("TimeStamp");
            int recordCreationTimeOrdinal = rd.GetOrdinal("RecordCreationTime");
            int isRecordingOrdinal = rd.GetOrdinal("IsRecording");
            int isCompleteOrdinal = rd.GetOrdinal("IsComplete");
            int clientTimeUtcOrdinal = rd.GetOrdinal("ClientTimeUtc");
            int serverTimeUtcOrdinal = rd.GetOrdinal("ServerTimeUtc");
            int callIdOrdinal = rd.GetOrdinal("CallID");
            int extendedStatusOrdinal = rd.GetOrdinal("ExtendedStatus");
            int interviewDurationOrdinal = rd.GetOrdinal("InterviewDuration");

            while (true)
            {
                bool isRead = rd.Read();

                if (isRead == false)
                    break;

                var entity = new BvPersonDeferredMonitoringPartEntity();

                if (!rd.IsDBNull(idOrdinal))
                    entity.ID =
                        rd.GetInt32(idOrdinal);

                if (!rd.IsDBNull(personSidOrdinal))
                    entity.PersonSID =
                        rd.GetInt32(personSidOrdinal);

                if (!rd.IsDBNull(interviewIdOrdinal))
                    entity.InterviewID =
                        rd.GetInt32(interviewIdOrdinal);

                if (!rd.IsDBNull(surveySidOrdinal))
                    entity.SurveySID =
                        rd.GetInt32(surveySidOrdinal);

                if (!rd.IsDBNull(timeStampOrdinal))
                    entity.TimeStamp =
                        rd.GetDateTime(timeStampOrdinal);

                if (!rd.IsDBNull(recordCreationTimeOrdinal))
                    entity.RecordCreationTime =
                        rd.GetDateTime(recordCreationTimeOrdinal);

                if (!rd.IsDBNull(isRecordingOrdinal))
                    entity.IsRecording = 
                        rd.GetBoolean(isRecordingOrdinal);

                if (!rd.IsDBNull(isCompleteOrdinal))
                    entity.IsComplete = 
                        rd.GetBoolean(isCompleteOrdinal);

                if (!rd.IsDBNull(clientTimeUtcOrdinal))
                    entity.ClientTimeUtc =
                        rd.GetDateTime(clientTimeUtcOrdinal);

                if (!rd.IsDBNull(serverTimeUtcOrdinal))
                    entity.ServerTimeUtc =
                        rd.GetDateTime(serverTimeUtcOrdinal);

                if (!rd.IsDBNull(callIdOrdinal))
                    entity.CallID =
                        rd.GetInt32(callIdOrdinal);

                if (!rd.IsDBNull(extendedStatusOrdinal))
                    entity.ExtendedStatus =
                        rd.GetInt32(extendedStatusOrdinal);

                if (!rd.IsDBNull(interviewDurationOrdinal))
                    entity.InterviewDuration =
                        rd.GetInt32(interviewDurationOrdinal);
						
                bvPersonDeferredMonitoringEntityList.Add(entity);
            }

            return bvPersonDeferredMonitoringEntityList;
        }

        public static List<BvPersonDeferredMonitoringPartEntity> GetByCondition(
            string condition,
            params SqlParameter[] parameters)
        {
            var query = string.IsNullOrEmpty(condition) ? SelectSql : SelectSql + "\r\nwhere\r\n" + condition;

            return ExecuteQuery(query, parameters);
        }

        public static BvPersonDeferredMonitoringPartEntity GetById(long deferredRecordId)
        {
            return GetByCondition(
                "ID = @ID",
                new[] { new SqlParameter("@ID", deferredRecordId) }).SingleOrDefault();
        }
        
        public static BvPersonDeferredMonitoringPartEntity GetByCallId(int callId)
        {
            var query = SelectSql + " WITH (INDEX(IX_BvPersonDeferredMonitoring_CallID))\r\nwhere\r\n" + "[CallID] = @CallID";
            
            return ExecuteQuery(query, new[] {new SqlParameter("@CallID", callId)}).FirstOrDefault();
        }

        private static List<BvPersonDeferredMonitoringPartEntity> ExecuteQuery(
            string query,
            params SqlParameter[] parameters)
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
                command.CommandText = query;

                if (parameters.Any())
                {
                    command.Parameters.AddRange(parameters);
                }

                var reader = command.ExecuteReader();

                disposableResources.Push(reader);

                return ReadList(reader);
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

        /// <summary>
        /// Returns a current deferred monitoring record.
        /// </summary>
        /// <param name="deferredRecordId"></param>
        /// <param name="interviewerId"></param>
        /// <returns>Deferred monitoring record</returns>
        public static BvPersonDeferredMonitoringPartEntity GetByIdWithCheck(int deferredRecordId, int interviewerId)
        {
            var entity = GetById(deferredRecordId);

            if (entity == null)
            {
                throw new Exception(
                    string.Format("Deferred record [{0}] is not found. /// InterviewerID=[{1}]",
                        deferredRecordId, interviewerId));
            }

            if (entity.PersonSID != interviewerId)
            {
                throw new Exception(
                    string.Format("Deferred record [{0}] contains wrong interviewer id [{1}]. Expected id is [{2}].",
                        deferredRecordId, entity.PersonSID, interviewerId));

            }

            if (entity.IsComplete || !entity.IsRecording)
            {
                throw new Exception(
                    string.Format(
                        "Deferred record [{0}] is not active (IsComplete=[{1}], IsRecording=[{2}]). /// InterviewerID=[{3}]",
                        deferredRecordId, entity.IsComplete, entity.IsRecording, interviewerId));
            }

            return entity;
        }
    }
}
