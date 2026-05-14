using System;
using BvDotNetEngine.Events;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.Fakes;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;


namespace Confirmit.CATI.Core.UnitTests.Scheduling.BvDotNetScript
{
    /// <summary>
    /// Summary description for ExtendedSchedulingAPITest
    /// </summary>
    [TestClass]
    public class ExtendedSchedulingAPITest : BaseTest
    {
        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();

            RegistryStub<ITimezoneService, StubITimezoneService>().GetDefaultCallCenterTimezoneId = () => 1;
            RegistryStub<ISurveyMetadataCacheService, StubISurveyMetadataCacheService>().GetInt32 =
            id => new StubISurveyMetadataCache { GetReplFormDescString = form => null };

            RegistryStub<IInterviewerApiClient, StubIInterviewerApiClient>();
            RegistryStub<ISurveyConnectionStringProvider, StubISurveyConnectionStringProvider>();

            var backendInstance = new BackendInstance();
            BackendInstance.Current = backendInstance;
        }


        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(SchedulingScriptExecutionException))]
        public void f_InvalidVariableName_ThrowException()
        {
            ExtendedSchedulingAPI target = CreateSchedulingAPI();
            ExprObj result = target.f("123", null);
        }

        private ExtendedSchedulingAPI CreateSchedulingAPI()
        {
            var api = new ExtendedSchedulingAPI();
            RegistryStub<IShiftServiceFactory, StubIShiftServiceFactory>().GetInt32 = (id) => null;
            RegistryStub<ICompanyInfo, StubICompanyInfo>().CompanyIdGet = () => 0;

            api.Init(new EventSchedule(new BvSurveyEntity(), new BvInterviewWithOriginEntity(new BvInterviewEntity()), null, new SchedulingScriptExecutionOptions() { Timings = new BvInterviewTimings(), EventTime = DateTime.Now }, 0, new ExecuteSchedulingScriptEvent()));
            return api;
        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(SchedulingScriptExecutionException))]
        public void fr_InvalidVariableName_ThrowException()
        {
            ExtendedSchedulingAPI target = CreateSchedulingAPI();
            ExprObj result = target.fr("1234");
        }

    }
}
