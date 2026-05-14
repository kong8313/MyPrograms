using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class InterviewersAvailabilityService
    {
        private readonly IDatabaseEngineFactory _databaseEngineFactory;

        public InterviewersAvailabilityService(IDatabaseEngineFactory databaseEngineFactory)
        {
            _databaseEngineFactory = databaseEngineFactory;
        }

        public bool IsAnyInterviewerAvailable(int dialerId, int surveyId)
        {
            var dbEngine = _databaseEngineFactory.CreateForCurrentInstanceDatabase();

            var sql = @"SELECT
                        CASE WHEN EXISTS ( SELECT 1 FROM BvTasks
                           WHERE
	                            (LoggedInToDialerState = @LoggedIn OR LoggedInToDialerState = @LoggingIn)
	                            AND DialTypeId = @DialType
	                            AND DialerId = @DialerId
	                            AND SurveySID = @SurveySID
                        )
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT)
                        END";

            return dbEngine.ExecuteScalar<bool>(sql, CommandType.Text,
                new SqlParameter("@LoggedIn", LoginState.LOGGED_IN),
                new SqlParameter("@LoggingIn", LoginState.LOGGING_IN),
                new SqlParameter("@DialType", DialType.Landline),
                new SqlParameter("@DialerId", dialerId),
                new SqlParameter("@SurveySID", surveyId));
        }

        public bool IsAnyInterviewerAvailable(int dialerId, IEnumerable<int> groupIds)
        {
            var dbEngine = _databaseEngineFactory.CreateForCurrentInstanceDatabase();

            var sql = @"SELECT 
                        CASE WHEN EXISTS (
                            SELECT 1 FROM dbo.utilSplitNumbers(@GroupIds, ',') AS groupId
                            LEFT JOIN BvMembership AS member ON member.ContainerSID = CAST(groupId.Item AS INT)
                            LEFT JOIN BvTasks AS task ON task.PersonSID = member.ObjectSID
							WHERE
							    (task.LoggedInToDialerState = @LoggedIn OR task.LoggedInToDialerState = @LoggingIn)
							    AND task.DialTypeId = @DialType 
							    AND task.DialerId = @DialerId
                        )
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT) 
                        END";

            return dbEngine.ExecuteScalar<bool>(sql, CommandType.Text,
                new SqlParameter("@LoggedIn", LoginState.LOGGED_IN),
                new SqlParameter("@LoggingIn", LoginState.LOGGING_IN),
                new SqlParameter("@DialType", DialType.Landline),
                new SqlParameter("@DialerId", dialerId),
                new SqlParameter("@GroupIds", String.Join(",", groupIds)));
        }

        public bool IsInterviewerAvailable(int dialerId, int interviewerId)
        {
            var dbEngine = _databaseEngineFactory.CreateForCurrentInstanceDatabase();

            var sql = @"SELECT 
                        CASE WHEN EXISTS (SELECT 1 FROM BvTasks
                           WHERE
	                            (LoggedInToDialerState = @LoggedIn OR LoggedInToDialerState = @LoggingIn)
	                            AND DialTypeId = @DialType
	                            AND DialerId = @DialerId
	                            AND PersonSID = @PersonSID
                        )
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT) 
                        END";

            return dbEngine.ExecuteScalar<bool>(sql, CommandType.Text,
                new SqlParameter("@LoggedIn", LoginState.LOGGED_IN),
                new SqlParameter("@LoggingIn", LoginState.LOGGING_IN),
                new SqlParameter("@DialType", DialType.Landline),
                new SqlParameter("@DialerId", dialerId),
                new SqlParameter("@PersonSID", interviewerId));
        }
    }
}