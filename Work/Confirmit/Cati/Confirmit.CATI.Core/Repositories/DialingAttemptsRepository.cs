using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.ManagementService;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Repositories
{
    public class DialingAttemptsRepository
    {
        public List<CatiDialingAttempt> GetDialingAttemptsForLastInterviewAttempt(int surveyId, int interviewId)
        {
            var result = new List<CatiDialingAttempt>();
            var query = @"
                declare @historyId int;

                select top(1) @historyId=Id from BvHistory where SurveyId = @SurveyId and InterviewId = @InterviewId order by firedTime desc

                select dh.ID as DialId, dh.StartTime, dh.FinishTime, dh.DialerCallerId, dh.RingTime, dh.JsonCallOutcomeMetadata, dh.DialerCallOutcome, dh.RespondentTelephoneNumber as TelephoneNumber
                from BvDialHistoryToInterviewHistory dhtih
                left join BvDialHistory dh on dh.ID = dhtih.DialHistoryId
                where dhtih.InterviewHistoryId = @historyId
                order by dhtih.FinishTime";
            
            var db = new DatabaseEngine();
            var reader = db.ExecuteReaderInNewConnection(query, CommandType.Text, new SqlParameter("SurveyId", surveyId), new SqlParameter("InterviewId", interviewId));
            while (reader.Read())
            {
                var json = reader["JsonCallOutcomeMetadata"] == DBNull.Value ? null : (string)reader["JsonCallOutcomeMetadata"];
                var callOutcomeMetadata = json != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(json) : null;
                result.Add(new CatiDialingAttempt() {
                    DialId = (int)reader["DialId"],
                    StartTime = reader["StartTime"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["StartTime"],
                    FinishTime = reader["FinishTime"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["FinishTime"],
                    RingTime = reader["RingTime"] == DBNull.Value ? (int?)null : (int)reader["RingTime"],
                    DialerCallerId = reader["DialerCallerId"] == DBNull.Value ? null : (string)reader["DialerCallerId"],
                    DialerCallOutcome = reader["DialerCallOutcome"] == DBNull.Value ? (int?)null : (int)reader["DialerCallOutcome"],
                    TelephoneNumber = reader["TelephoneNumber"] == DBNull.Value ? null : (string)reader["TelephoneNumber"],
                    CallOutcomeMetadata = callOutcomeMetadata
                });
            }

            return result;
        }
    }
}