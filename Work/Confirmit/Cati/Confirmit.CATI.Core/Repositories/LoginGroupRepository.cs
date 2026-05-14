using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class LoginGroupRepository : ILoginGroupRepository
    {
        private const int NoCalls = (int)InterviewState.NO_CALLS;
        private const int Selecting = (int)InterviewState.SELECTING;
        private const int Waiting = (int)InterviewState.WAITING;
        
        public bool IsResourceLoggedIntoSurvey(int personOrGroupId, int surveySid)
        {
            var query = "SELECT TOP(1) * FROM [BvLoginGroup] WHERE [ObjectSID] = @PersonOrGroupId AND ([SurveySID] = @SurveySid OR [SurveySID] = 0)";
            var table = new DatabaseEngine().ExecuteDataTable<DataTable>(query, CommandType.Text,
                new SqlParameter("@PersonOrGroupId", personOrGroupId),
                new SqlParameter("@SurveySid", surveySid));

            return table.Rows.Count > 0;
        }

        public bool IsResourceReadyForCallInSurvey(int personOrGroupId, int surveySid)
        {
            var query = $@"
                SELECT TOP(1) lg.[ObjectSID] FROM [BvLoginGroup]  lg
                LEFT JOIN [BvTasks] t ON t.[PersonSID] = lg.[PersonSID]
                WHERE lg.[ObjectSID] = @PersonOrGroupId 
                  AND (lg.[SurveySID] = @SurveySid OR lg.[SurveySID] = 0) 
                  AND t.[InterviewState] in ({NoCalls}, {Selecting}, {Waiting})";
            var table = new DatabaseEngine().ExecuteDataTable<DataTable>(query, CommandType.Text,
                new SqlParameter("@PersonOrGroupId", personOrGroupId),
                new SqlParameter("@SurveySid", surveySid));

            return table.Rows.Count > 0;
        }
        
        public bool IsAnyoneLoggedIntoSurvey(int surveySid)
        {
            var query = "SELECT TOP(1) * FROM [BvLoginGroup] WHERE [SurveySID] = @SurveySid OR [SurveySID] = 0";
            var table = new DatabaseEngine().ExecuteDataTable<DataTable>(query, CommandType.Text,
                new SqlParameter("@SurveySid", surveySid));

            return table.Rows.Count > 0;
        }
        
        public bool IsAnyoneLoggedIntoSurvey(int surveySid, int agentTypeIndex)
        {
            var query = $@"SELECT TOP(1) * FROM [BvLoginGroup] lg 
                LEFT JOIN [BvPerson] p ON lg.[PersonSID] = p.[SID]
                WHERE ([SurveySID] = @SurveySid OR [SurveySID] = 0) AND p.[Type] = @AgentTypeIndex";
            var table = new DatabaseEngine().ExecuteDataTable<DataTable>(query, CommandType.Text,
                new SqlParameter("@AgentTypeIndex", agentTypeIndex),
                new SqlParameter("@SurveySid", surveySid));

            return table.Rows.Count > 0;
        }
        
        public bool IsAnyoneReadyForCallInSurvey(int surveySid, int agentTypeIndex)
        {
            var query = $@"
                SELECT TOP(1) lg.[ObjectSID] FROM [BvLoginGroup]  lg
                LEFT JOIN [BvPerson] p ON lg.[PersonSID] = p.[SID]
                LEFT JOIN [BvTasks] t ON t.[PersonSID] = lg.[PersonSID]
                WHERE (lg.[SurveySID] = @SurveySid OR lg.[SurveySID] = 0) 
                  AND p.[Type] = @AgentTypeIndex
                  AND t.[InterviewState] in ({NoCalls}, {Selecting}, {Waiting})";
            var table = new DatabaseEngine().ExecuteDataTable<DataTable>(query, CommandType.Text,
                new SqlParameter("@AgentTypeIndex", agentTypeIndex),
                new SqlParameter("@SurveySid", surveySid));

            return table.Rows.Count > 0;
        }
    }
}