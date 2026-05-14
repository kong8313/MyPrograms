using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class ScheduledEmailReportsRepository : IScheduledEmailReportsRepository
    {
        /// <summary>
        /// Gets email report entity by type.
        /// </summary>
        public BvScheduledEmailReportsEntity GetByReportType(ReportType reportType)
        {
            var entities = BvScheduledEmailReportsAdapter.GetByCondition(
                "[ReportType] = @ReportType",
                new SqlParameter("@ReportType", (int)reportType));

            return entities.FirstOrDefault();
        }

        /// <summary>
        /// Gets email report entity by type, create it if it does not exist.
        /// </summary>
        public BvScheduledEmailReportsEntity GetCreateByReportType(ReportType reportType)
        {
            var bvScheduledEmailReportsEntity = GetByReportType(reportType);

            if (bvScheduledEmailReportsEntity == null)
            {
                bvScheduledEmailReportsEntity = new BvScheduledEmailReportsEntity {ReportType = (int)reportType};

                BvScheduledEmailReportsAdapter.Insert(bvScheduledEmailReportsEntity);
            }

            return bvScheduledEmailReportsEntity;
        }

        /// <summary>
        /// Gets list of all existing email report entities.
        /// </summary>
        /// <returns>List of all existing email report entities.</returns>
        public List<BvScheduledEmailReportsEntity> GetAll()
        {
            return BvScheduledEmailReportsAdapter.GetAll();
        }

        /// <summary>
        /// Updates the specified email report entity.
        /// </summary>
        /// <param name="entity">The email report entity to update.</param>
        public void Update(BvScheduledEmailReportsEntity entity)
        {
            BvScheduledEmailReportsAdapter.Update(entity);
        }
    }
}
