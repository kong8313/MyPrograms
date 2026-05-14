using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Query;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Repositories
{
    public class PersonSessionHistoryRepository : IPersonSessionHistoryRepository
    {
        private readonly ICompanyInfo _companyInfo;
        private readonly IDatabaseEngineFactory _databaseEngineFactory;
        private readonly IRemoteDataCopier _remoteDataCopier;
        private readonly IConnectionStrings _connectionStrings;

        public PersonSessionHistoryRepository(
            ICompanyInfo companyInfo,
            IDatabaseEngineFactory databaseEngineFactory,
            IRemoteDataCopier remoteDataCopier,
            IConnectionStrings connectionStrings)
        {
            _companyInfo = companyInfo;
            _databaseEngineFactory = databaseEngineFactory;
            _remoteDataCopier = remoteDataCopier;
            _connectionStrings = connectionStrings;
        }

        public int InsertStartSessionEvent(IConnectionProvider connectionProvider, int callCenterId, int personId)
        {
            var query =
                @"INSERT INTO CatiInterviewerSessionHistory( CompanyId, CallCenterId, InterviewerId, LoginTime ) 
                VALUES( @CompanyId, @CallCenterId, @InterviewerId, GETUTCDATE() )
              SELECT CAST( SCOPE_IDENTITY() AS INT )";

            var databaseEngine = _databaseEngineFactory.CreateForCustomConnectionProvider(connectionProvider);

            var result = databaseEngine.ExecuteScalar<int>(
                query,
                CommandType.Text,
                new SqlParameter("@CompanyId", _companyInfo.CompanyId),
                new SqlParameter("@CallCenterId", callCenterId),
                new SqlParameter("@InterviewerId", personId));
            return result;
         }

        public void InsertStopSessionEvent(IConnectionProvider connectionProvider, int sessionId)
        {
            var query =
                @"UPDATE CatiInterviewerSessionHistory SET LogoutTime = GETUTCDATE() WHERE SessionId = @SessionId";

            var databaseEngine = _databaseEngineFactory.CreateForCustomConnectionProvider(connectionProvider);
            databaseEngine.ExecuteNonQuery(
                query,
                CommandType.Text,
                new SqlParameter("@SessionId", sessionId));
        }

        public IEnumerable<PersonSessionHistoryEntity> GetSessionEvents(int? callCenterId, int companyId, DateTime? startDate, DateTime? endDate)
        {
            var maxRows = ServiceLocator.Resolve<ISystemSettings>().Reports.CallHistoryReportLoginLogoutEventsRowsLimit;
            var tempTableName = "#temp";

            var copyDataQuery = $@"SELECT p.SID, p.CallCenterID, p.Name as InterviewerName, c.Name as CallCenterName
                        FROM BvPerson p
                        INNER JOIN BvCallCenter c on c.ID = p.CallCenterID";

            var query = $@"SELECT TOP ({maxRows}) SessionId, InterviewerId, CompanyId, sh.CallCenterId, LoginTime, LogoutTime,  t.InterviewerName, CallCenterName
                        FROM CatiInterviewerSessionHistory sh
                        INNER JOIN {tempTableName} t on t.SID = sh.InterviewerId
                        WHERE CompanyId = @CompanyId and                          
                        LoginTime >= @StartDate and LoginTime <= @EndDate";

            var parameters = new List<SqlParameter>() {
                                                new SqlParameter("@CompanyId", companyId),
                                                new SqlParameter("@StartDate", startDate ?? new DateTime(1953, 01, 01, 0, 0, 0)),
                                                new SqlParameter("@EndDate", endDate ?? new DateTime(9999, 12, 31, 23, 59, 59, 997))};

            if (callCenterId.HasValue)
            {
                query += " and sh.CallCenterId = @CallCenterId";
                parameters.Add(new SqlParameter("@CallCenterId", callCenterId));
            }

            var dataTable = new DataTable();

            using (var connectionProvider = new RemoteConnectionProvider(_connectionStrings.ConfirmlogConnectionString))
            using (var command = new SqlCommand(query, connectionProvider.Connection))
            {
                _remoteDataCopier.CopyDataToNewTable(_connectionStrings.GetConnectionStringForSpecificCompany(companyId), connectionProvider, tempTableName, copyDataQuery);

                command.Parameters.AddRange(parameters.ToArray());
                command.CommandType = CommandType.Text;
                using (var reader = command.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
            }

            var personSessionHistoryEntities = dataTable.Select().Select(x => new PersonSessionHistoryEntity
            {
                CallCenterId = (int)x["CallCenterId"],
                CallCenterName = x["CallCenterName"].ToString(),
                CompanyId = (int)x["CompanyId"],
                SessionId = (int)x["SessionId"],
                InterviewerId = (int)x["InterviewerId"],
                InterviewerName = x["InterviewerName"].ToString(),
                LoginTime = (DateTime)x["LoginTime"],
                LogoutTime = (x["LogoutTime"] == DBNull.Value) ? null : (DateTime?)x["LogoutTime"]
            });

            return personSessionHistoryEntities;
        }
    }
}