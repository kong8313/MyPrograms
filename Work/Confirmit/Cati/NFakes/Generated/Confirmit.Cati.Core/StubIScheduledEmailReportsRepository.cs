using System;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIScheduledEmailReportsRepository : IScheduledEmailReportsRepository 
    {
        private IScheduledEmailReportsRepository _inner;

        public StubIScheduledEmailReportsRepository()
        {
            _inner = null;
        }

        public IScheduledEmailReportsRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvScheduledEmailReportsEntity GetByReportTypeReportTypeDelegate(ReportType reportType);
        public GetByReportTypeReportTypeDelegate GetByReportTypeReportType;

        BvScheduledEmailReportsEntity IScheduledEmailReportsRepository.GetByReportType(ReportType reportType)
        {


            if (GetByReportTypeReportType != null)
            {
                return GetByReportTypeReportType(reportType);
            } else if (_inner != null)
            {
                return ((IScheduledEmailReportsRepository)_inner).GetByReportType(reportType);
            }

            return default(BvScheduledEmailReportsEntity);
        }

        public delegate BvScheduledEmailReportsEntity GetCreateByReportTypeReportTypeDelegate(ReportType reportType);
        public GetCreateByReportTypeReportTypeDelegate GetCreateByReportTypeReportType;

        BvScheduledEmailReportsEntity IScheduledEmailReportsRepository.GetCreateByReportType(ReportType reportType)
        {


            if (GetCreateByReportTypeReportType != null)
            {
                return GetCreateByReportTypeReportType(reportType);
            } else if (_inner != null)
            {
                return ((IScheduledEmailReportsRepository)_inner).GetCreateByReportType(reportType);
            }

            return default(BvScheduledEmailReportsEntity);
        }

        public delegate List<BvScheduledEmailReportsEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<BvScheduledEmailReportsEntity> IScheduledEmailReportsRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IScheduledEmailReportsRepository)_inner).GetAll();
            }

            return default(List<BvScheduledEmailReportsEntity>);
        }

        public delegate void UpdateBvScheduledEmailReportsEntityDelegate(BvScheduledEmailReportsEntity entity);
        public UpdateBvScheduledEmailReportsEntityDelegate UpdateBvScheduledEmailReportsEntity;

        void IScheduledEmailReportsRepository.Update(BvScheduledEmailReportsEntity entity)
        {

            if (UpdateBvScheduledEmailReportsEntity != null)
            {
                UpdateBvScheduledEmailReportsEntity(entity);
            } else if (_inner != null)
            {
                ((IScheduledEmailReportsRepository)_inner).Update(entity);
            }
        }

    }
}