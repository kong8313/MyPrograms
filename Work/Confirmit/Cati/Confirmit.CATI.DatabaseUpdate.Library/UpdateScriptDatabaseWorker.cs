using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary
{
    public class UpdateScriptDatabaseWorker : IUpdateScriptDatabaseWorker
    {
        private readonly ILogger _logger;
        private readonly IQueryExecutor _queryExecutor;

        public UpdateScriptDatabaseWorker(ILogger logger, IQueryExecutor queryExecutor)
        {
            _logger = logger;
            _queryExecutor = queryExecutor;
        }

        public UpdateScriptInfo[] GetAppliedUpdateScriptInfos(string databaseName)
        {
            _logger.WriteLog("Start GetAppliedUpdateScriptInfos method");

            try
            {
                const string query = "select * from BvVersionHistory";
                var bvVersionInfo = _queryExecutor.ExecuteDataTable<DataTable>(databaseName, query);

                var list = new List<UpdateScriptInfo>();
                foreach (DataRow row in bvVersionInfo.Rows)
                {
                    int scriptNumber = Convert.ToInt32(row["ScriptNumber"]);

                    string name;
                    string description;
                    string extension = "sql";
                    if (scriptNumber == -1)
                    {
                        string[] descriptoinParts = row["Description"].ToString().Split(new[] { ':' }, 2);
                        name = descriptoinParts[0];
                        description = descriptoinParts[1];
                        if (name.ToLower().EndsWith(".ps1"))
                        {
                            extension = "ps1";
                            name = name.Substring(name.Length - 4);
                        }
                        else if (name.ToLower().EndsWith(".sql"))
                        {
                            extension = "sql";
                            name = name.Substring(name.Length - 4);
                        }
                    }
                    else
                    {
                        name = string.Format("_{0}_{1}_{2}_{3}",
                            Convert.ToInt32(row["Major"]).ToString("00"),
                            Convert.ToInt32(row["Minor"]).ToString("00"),
                            row["BranchName"],
                            scriptNumber.ToString("00"));
                        description = row["Description"].ToString();
                    }

                    list.Add(new UpdateScriptInfo(
                        name,
                        extension,
                        description,
                        false,
                        Convert.ToDateTime(row["ScriptAppliedDate"]),
                        Convert.ToInt32(row["Duration"]),
                        row["ScriptText"].ToString(),
                        row["ScriptOutput"].ToString(),
                        Convert.ToBoolean(row["IsAppliedDuringDBCreation"]),
                        row["DbUpateUtilityVersion"].ToString(),
                        row["ActiveUser"].ToString()));
                }

                return list.ToArray();
            }
            finally 
            {
                _logger.WriteLog("Finish GetAppliedUpdateScriptInfos method");
            }
        }

        public void AddAppliedUpdateScriptInfo(string databaseName, UpdateScriptInfo updateScriptInfo)
        {
            _logger.WriteLog("Start GetAppliedUpdateScriptInfos method");

            const string query = @"insert into BvVersionHistory 
                                       ([Major], [Minor], [BranchName], [ScriptNumber], [Description], [ScriptAppliedDate], [Duration], [ScriptText], [ScriptOutput], [IsAppliedDuringDBCreation], [DbUpateUtilityVersion], [ActiveUser])
                                   values
                                       (@Major, @Minor, @BranchName, @ScriptNumber, @Description, @ScriptAppliedDate, @Duration, @ScriptText, @ScriptOutput, @IsAppliedDuringDBCreation, @DbUpateUtilityVersion, @ActiveUser)";
            
            string description = updateScriptInfo.Description;
            if(updateScriptInfo.ScriptNumber == -1 )
            {
                description = updateScriptInfo.Name + ": " + description;
            }

            var parameters = new[]
            {
                new SqlParameter("Major", updateScriptInfo.Major),
                new SqlParameter("Minor", updateScriptInfo.Minor),
                new SqlParameter("BranchName", updateScriptInfo.BranchName),
                new SqlParameter("ScriptNumber", updateScriptInfo.ScriptNumber),
                new SqlParameter("Description", description),
                new SqlParameter("ScriptAppliedDate", updateScriptInfo.ScriptAppliedDate),
                new SqlParameter("Duration", updateScriptInfo.Duration),
                new SqlParameter("ScriptText", updateScriptInfo.ScriptText),
                new SqlParameter("ScriptOutput", updateScriptInfo.ScriptOutput),
                new SqlParameter("IsAppliedDuringDBCreation", updateScriptInfo.IsAppliedDuringDBCreation),
                new SqlParameter("DbUpateUtilityVersion", updateScriptInfo.DbUpateUtilityVersion),
                new SqlParameter("ActiveUser", updateScriptInfo.ActiveUser)
            };

            _queryExecutor.ExecuteNonQuery(databaseName, query, parameters);

            _logger.WriteLog("Finish GetAppliedUpdateScriptInfos method");
        }
    }
}