using System;
using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.ActivityLogging.SiteSettings;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.Test.Common.Attributes;
using DialerCommon.DialerParameters;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.ActivityLogging
{
    [TestClass]
    public class ActivityLoggingTest : BaseTest
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
         
            var backendInstance = new BackendInstance();
            BackendInstance.Current = backendInstance;
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();

            BackendInstance.Current = null;
        }
        
        [TestMethod, Owner(@"FIRM\MikhailT")]
        [Bug(45671)]
        public void SetDialerDefaultSurveyParametersEvent_AcceptsEnumerable_EventSerializationSucceeded()
        {
            var evt = new SetDialerDefaultSurveyParametersEvent(new List<DialerParameter>());
            evt.Finish();
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        [Bug(45671)]
        public void SetDialerSurveyParametersEvent_AcceptsEnumerable_EventSerializationSucceeded()
        {
            var evt = new SetDialerSurveyParametersEvent(1, "0001111", new List<DialerParameter>());
            evt.Finish();
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void InterviewerActivityEventType_EnumContainsEachKeyOneTime_EnumIsCorrect()
        {
            CheckEnumDuplicates(typeof(InterviewerActivityEventType));
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void ManagementEvent_EnumContainsEachKeyOneTime_EnumIsCorrect()
        {
            CheckEnumDuplicates(typeof(ManagementEvent));
        }

        private static void CheckEnumDuplicates(Type type)
        {
            var duplicates = Enum.GetNames(type).GroupBy(x => (int)Enum.Parse(type, x)).Where(x => x.Count() > 1);
            var res = duplicates.Select(x => String.Join(",", x)).ToArray();

            Assert.IsFalse(res.Any(),
                          string.Format("Duplicate values in enum: {0}", String.Join("\n", res)));
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void ManagementEvent_SyncQueueEventsAreOnRightPlace_EnumIsCorrect()
        {
            Assert.AreEqual(196, (int)ManagementEvent.SyncQueueAdd);
            Assert.AreEqual(197, (int)ManagementEvent.SyncQueueDelete);
            Assert.AreEqual(198, (int)ManagementEvent.SyncQueueResync);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void UpdateGneralSiteSettingsEvent_NoChangedMadeInSettings_HasChangesIsFalse()
        {
            var evt = new UpdateGeneralSiteSettingsEvent();
            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            evt.RememberSettings(systemSettings);
            evt.CollectChangedSettings(systemSettings);
            Assert.IsFalse(evt.HasChanges);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void UpdateGeneralSiteSettingsEvent_ChangedMadeInSettings_HasChangesIsTrueAndOnlyChangedSettingsAreReflectedInTheEventDetails()
        {
            var evt = new UpdateGeneralSiteSettingsEvent();
            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            evt.RememberSettings(systemSettings);
            systemSettings.Reports.CallHistoryReportEnabled = true;
            evt.CollectChangedSettings(systemSettings);

            Assert.IsTrue(evt.HasChanges);
            Assert.IsTrue(evt.Details.ChangedSettings.Contains("Reports.CallHistoryReportEnabled"));
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void UpdateInterviewerConsoleSiteSettingsEvent_NoChangedMadeInSettings_HasChangesIsFalse()
        {
            var evt = new UpdateInterviewerConsoleSiteSettingsEvent();
            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            evt.RememberSettings(systemSettings);
            evt.CollectChangedSettings(systemSettings);
            Assert.IsFalse(evt.HasChanges);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void UpdateInterviewerConsoleSiteSettingsEvent_ChangedMadeInSettings_HasChangesIsTrueAndOnlyChangedSettingsAreReflectedInTheEventDetails()
        {
            var evt = new UpdateInterviewerConsoleSiteSettingsEvent();
            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            evt.RememberSettings(systemSettings);
            systemSettings.Console.EnableCheckSpellingToolbarButton = false;
            evt.CollectChangedSettings(systemSettings);

            Assert.IsTrue(evt.HasChanges);
            Assert.IsTrue(evt.Details.ChangedSettings.Contains("EnableCheckSpellingToolbarButton"));
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void UpdateSecuritySiteSettingsEvent_NoChangedMadeInSettings_HasChangesIsFalse()
        {
            var evt = new UpdateSecuritySiteSettingsEvent();
            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            evt.RememberSettings(systemSettings);
            evt.CollectChangedSettings(systemSettings);
            Assert.IsFalse(evt.HasChanges);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void UpdateSecuritySiteSettingsEvent_ChangedMadeInSettings_HasChangesIsTrueAndOnlyChangedSettingsAreReflectedInTheEventDetails()
        {
            var evt = new UpdateSecuritySiteSettingsEvent();
            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            evt.RememberSettings(systemSettings);
            systemSettings.InterviewerPassword.IsExpirationEnabled = true;
            evt.CollectChangedSettings(systemSettings);

            Assert.IsTrue(evt.HasChanges);
            Assert.IsTrue(evt.Details.ChangedSettings.Contains("InterviewerPassword.IsExpirationEnabled"));
        }
    }
}
