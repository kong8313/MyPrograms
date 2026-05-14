using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.EmailReports;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IScheduledEmailReportsRepository
    {
        BvScheduledEmailReportsEntity GetByReportType(ReportType reportType);

        BvScheduledEmailReportsEntity GetCreateByReportType(ReportType reportType);

        List<BvScheduledEmailReportsEntity> GetAll();

        void Update(BvScheduledEmailReportsEntity entity);
    }
}
