using System;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Confirmit.CATI.IntegrationTests.Tests.EmailReports;

namespace Confirmit.CATI.Core.UnitTests.EmailReports
{
    public class BaseReportBuilderTest
    {
        protected ISurveyRepository SurveyRepositoryStub;
        protected ISystemSettings SystemSettingsStub;
        protected ILocalTimeProvider LocalTimeProviderStub;
        protected ISupervisorApiClient SupervisorApiClientStub;
        protected DateTime LocalTime;
        protected DateTime StartTime;
        protected DateTime EndTime;
        protected int TimezoneHoursOffset;

        public void BaseInitialize()
        {
            LocalTime = DateTime.Now;
            StartTime = new DateTime(2014, 5, 10, 12, 30, 5);
            EndTime = new DateTime(2014, 10, 10, 20, 0, 0);
            TimezoneHoursOffset = 3;

            var _backendInstance = new BackendInstance();
            BackendInstance.Current = _backendInstance;

            SurveyRepositoryStub = new StubISurveyRepository
            {
                GetAll = () =>
                    new[]
                                               {
                                                   new BvSurveyEntity {SID = 100},
                                                   new BvSurveyEntity {SID = 200}
                                               }
            };

            SystemSettingsStub = new StubISystemSettings
            {
                ReportsGet = () =>
                             {
                                 var s = new StubIReportsSettings
                                 {
                                     ReportGenerationTimeoutGet = () => 600
                                 };
                                 return s;
                             },
                ConsoleGet = () =>
                {
                    return new StubIConsoleSettings() { IncludeOpenEndReviewTimeInInterviewDurationGet = () => (false) };
                }
            };

            LocalTimeProviderStub = new FakeLocalTimeProvider(LocalTime, TimezoneHoursOffset);

            SupervisorApiClientStub = new StubISupervisorApiClient
            {
                GetSystemTemplate = () => Task.FromResult(new InterviewerProductivityReportTemplate()
                {
                    IncludeBreakTimeInCalculations = false,
                    ShowDialerAttempts = true,
                }),
                GetTemplateInt32 = (int id) => Task.FromResult(new InterviewerProductivityReportTemplate()
                {
                    IncludeBreakTimeInCalculations = false,
                    ShowDialerAttempts = true,
                    Id = id
                })
            };
        }
    }
}
