using System.Linq;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated
{
    public class SqlTableUpdatedPublisher : ISqlTableUpdatedPublisher
    {
        private const int DefaultCompanyId = 0;
        private const string ExchangeName = "Confirmit.Cati.Backend.SqlTableUpdated";

        private readonly CatiMessageBrokerPublisher _publisher;
        private readonly ICompanyInfo _companyInfo;
        private readonly IMultimodeInstanceName _multimodeInstanceName;
        private readonly ISideBySideManager _sideBySideManager;
        private readonly IConnectionStrings _connectionStrings;

        public SqlTableUpdatedPublisher(
            IMultimodeInstanceName multimodeInstanceName,
            ISideBySideManager sideBySideManager,
            IConnectionStrings connectionStrings,
            ICompanyInfo companyInfo,
            CatiMessageBrokerPublisher publisher)
        {
            _multimodeInstanceName = multimodeInstanceName;
            _sideBySideManager = sideBySideManager;
            _connectionStrings = connectionStrings;
            _publisher = publisher;
            _companyInfo = companyInfo;
        }

        public void PublishSystemSettingsUpdated()
        {
            PublishInCurrentCompany("BvSystemSettings");
        }

        public void PublishSystemSettingsUpdatedInAllCompanies()
        {
            using (new ConnectionScope(_connectionStrings.GetConnectionStringForSpecificCompany(DefaultCompanyId)))
            {
                var companyIds =
                    BvBackendInstanceAdapter.GetAll()
                        .Select(s => _sideBySideManager.AddSideBySideNameToServiceName(s.ServiceName))
                        .Select(serviceName => _multimodeInstanceName.ServiceNameToCompanyId(serviceName));

                Publish("BvSystemSettings", DefaultCompanyId.ToString());
                foreach (var companyId in companyIds)
                {
                    Publish("BvSystemSettings", companyId.ToString());
                }
            }
        }

        public void PublishShiftsUpdated()
        {
            PublishInCurrentCompany("BvShift");
        }

        public void PublishScheduleUpdated()
        {
            PublishInCurrentCompany("BvSchedule");
        }

        public void PublishScheduleParamsUpdated()
        {
            PublishInCurrentCompany("BvScheduleParam");
        }

        public void PublishBackendInstanceUpdated()
        {
            PublishInCurrentCompany("BvBackendInstance");
        }

        public void PublishTimeZoneUpdated()
        {
            PublishInCurrentCompany("BvTimezone");
        }

        public void PublishDialersUpdated()
        {
            PublishInCurrentCompany("BvDialers");
        }

        public void PublishInboundTelephoneNumberUpdated()
        {
            PublishInCurrentCompany("BvInboundTelephoneNumber");
        }

        public void PublishPersonUpdated()
        {
            PublishInCurrentCompany("BvPerson");
        }

        public void PublishPersonGroupUpdated()
        {
            PublishInCurrentCompany("BvPersonGroup");
        }

        public void PublishBreakTypeUpdated()
        {
            PublishInCurrentCompany("BvBreakType");
        }

        public void PublishCallCenterUpdated()
        {
            PublishInCurrentCompany("BvCallCenter");
        }

        public void PublishSurveyUpdated()
        {
            PublishInCurrentCompany("BvSurvey");
        }

        public void PublishStateUpdated()
        {
            PublishInCurrentCompany("BvState");
        }

        private void PublishInCurrentCompany(string tableName)
        {
            var routingKey = _companyInfo.CompanyId.ToString();
            Publish(tableName, routingKey);
        }

        private void Publish(string tableName, string routingKey)
        {
            var message = new SqlTableUpdatedMessage()
            {
                TableName = tableName
            };

            _publisher.Publish(ExchangeName, message, topic: routingKey);
        }
    }
}