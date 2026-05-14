using System.Data;
using System.Data.SqlClient;
using System.IO;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace CustomActionLibrary
{
    public class CatiDatabaseEngine : DatabaseEngine
    {
        private readonly ILogger _logger;

        public CatiDatabaseEngine(ILogger logger, string serverName, string login, string password)
            : base(logger, serverName, login, password)
        {
            _logger = logger;
        }

        public void CreateDatabase(string databaseName, string mdfPath, string ldfPath, string catiDefaultDbRecoveryModel)
        {
            if (string.IsNullOrEmpty(mdfPath))
            {
                ExecuteNonQuery("CREATE DATABASE " + databaseName);
            }
            else
            {
                string mdfFile = Path.Combine(mdfPath, databaseName + ".mdf");
                string ldfFile = Path.Combine(ldfPath, databaseName + "_log.ldf");

                _logger.WriteLog("mdfFile={0}\r\nldfFile={1}\r\ndatabase name={2}", mdfFile, ldfFile, databaseName);

                string sqlQuery = string.Format(
                    @"CREATE DATABASE [{0}] ON ( NAME = {0}, FILENAME = '{1}' ) 
					        LOG ON ( NAME = {0}_log, FILENAME = '{2}' )",
                    databaseName,
                    mdfFile,
                    ldfFile);

                ExecuteNonQuery(sqlQuery);
            }

            ExecuteNonQuery(string.Format("ALTER DATABASE {0} SET RECOVERY {1}", databaseName, catiDefaultDbRecoveryModel.ToUpperInvariant()));
        }

        /// <summary>
        /// Set database setting
        /// 1. Update 'MNDialerURL' field of 'BvSite' tables
        /// </summary>
        /// <param name="databaseName">Database name</param>        
        public void SetDatabaseSettings(string databaseName)
        {
            _logger.WriteLog("Begin SetDatabaseSettings");

            try
            {
                var defaultSchedule = new SqlParameter("@XmlUnderDev", SqlDbType.NVarChar)
                {
                    Value = Properties.Resources.Schedule
                };

                ExecuteNonQuery(databaseName, "UPDATE BvSchedule SET XmlUnderDev=@XmlUnderDev WHERE ScheduleID = (SELECT MIN(ScheduleID) FROM BvSchedule)", defaultSchedule);
            }
            finally
            {
                _logger.WriteLog("End SetDatabaseSettings");
            }
        }

        public string GetSettingValueFromDefaultCatiDatabase(string settingName)
        {
            var value = ExecuteScalar<object>(CatiSetupConstants.CatiDefaultDatabaseName, string.Format("select value from BvSystemSettings where SystemName='{0}'", settingName));

            return (value == null) ? null : value.ToString();
        }
    }
}
