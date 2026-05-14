using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;

using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Cache;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Confirmit.CATI.IntegrationTests.Tests.SystemSettings
{
    [TestClass]
    public class SystemSettingsTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        private ISystemSettings _settings;

        private ISystemSettingCache _cahce;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize(true);
            _framework.BackendInitialize();

            _settings = ServiceLocator.Resolve<ISystemSettings>();
            _cahce = ServiceLocator.Resolve<ISystemSettingCache>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }
        
        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetSettings_SettingsIsnotOverwrited_SettingAreCorrect()
        {
            const string email = @"asd@firmsw.no";
            
            SetDefaultValue(@"Email.AdministratorEmailAddress", email);

            _cahce.Reset();
            
            Assert.AreEqual(email, _settings.Email.AdministratorEmailAddress); 
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetSettings_UpdateSettingInsideTransaction_CacheIsValidAfterTransactionIsCommited()
        {
            const string defaultEmail = @"asd@firmsw.no";
            const string specificEmail = @"qwe@firmsw.no";
            const string specificEmail2 = @"qwe@firmsw.no";

            SetDefaultValue(@"Email.AdministratorEmailAddress", defaultEmail);

            _cahce.Reset();

            _settings.Email.AdministratorEmailAddress = specificEmail;

            using (var transaction = new DatabaseTransactionScope("tran"))
            {
                _settings.Email.AdministratorEmailAddress = specificEmail2;

                _cahce.Reset();

                transaction.Commit();
            }

            Assert.AreEqual(specificEmail2, _settings.Email.AdministratorEmailAddress);
        }


        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetSettings_SettingsIsnotOverwritedAndDefaultValueIsChanged_SettingAreUpdated()
        {
            const string email = @"asd@firmsw.no";

            SetDefaultValue(@"Email.AdministratorEmailAddress", email);

            _cahce.Reset();

            Assert.AreEqual(email, _settings.Email.AdministratorEmailAddress);

            const string newEmail = @"new@firmsw.no";

            SetDefaultValue(@"Email.AdministratorEmailAddress", newEmail);

            _cahce.Reset();

            Assert.AreEqual(newEmail, _settings.Email.AdministratorEmailAddress);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetSettings_SettingsIsOverwritedAndDefaultValueIsChanged_SettingAreNotUpdated()
        {
            const string defaultEmail = @"asd@firmsw.no";
            const string email = @"qwe@firmsw.no";

            SetDefaultValue(@"Email.AdministratorEmailAddress", defaultEmail);
            OverwriteDefaultValue(@"Email.AdministratorEmailAddress", email);

            _cahce.Reset();

            Assert.AreEqual(email, _settings.Email.AdministratorEmailAddress);

            const string newDefaultEmail = @"new@new.new";

            SetDefaultValue(@"Email.AdministratorEmailAddress", newDefaultEmail);

            _cahce.Reset();

            Assert.AreEqual(email, _settings.Email.AdministratorEmailAddress);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetSettings_SettingsIsOverwrited_SettingAreCorrect()
        {
            const string defaultEmail = @"asd@firmsw.no";
            const string email = @"qwe@firmsw.no";

            SetDefaultValue(@"Email.AdministratorEmailAddress", defaultEmail);
            OverwriteDefaultValue(@"Email.AdministratorEmailAddress", email);

            _cahce.Reset();

            Assert.AreEqual(email, _settings.Email.AdministratorEmailAddress);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void SetSettings_SettingsIsChanged_OnChangeIsCalled()
        {
            var fake = new SiteSettingsFake();

            ServiceLocator.Resolve<IServiceRegistrator>()
                    .RegisterSingleton<ISiteSettings>(fake)
                    .RegisterSingleton<ISiteSettingsGroup>(fake);

            OverwriteDefaultValue(SystemSettingConstants.Site.StartSurveyURL, "http://blablabla");

            _cahce.Reset();

            Assert.AreEqual(true, fake.IsChanged);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void SetSettings_SettingsIsNotOverwrited_SettingAreCorrect()
        {
            OverwriteDefaultValue(SystemSettingConstants.Site.StartSurveyURL, "http://blablabla");
            _cahce.Reset(); 
            
            var fake = new SiteSettingsFake();

            ServiceLocator.Resolve<IServiceRegistrator>()
                    .RegisterSingleton<ISiteSettings>(fake)
                    .RegisterSingleton<ISiteSettingsGroup>(fake);

            OverwriteDefaultValue(SystemSettingConstants.Site.StartSurveyURL, "http://blablabla");

            _cahce.Reset();

            Assert.AreEqual(false, fake.IsChanged);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void SetSettings_SetSettingWhenInternalCacheIsNull_SettingWasSetCorrectlyWithoutException()
        {
            var cahce = new SystemSettingCache(new IpFilterCache(), new StubISqlTableUpdatedPublisher(), new StubICompanyInfo());
            const string settingName = SystemSettingConstants.Setup.TestCertificateName;
            const string testValue = "testCertificateName";
            cahce.Set(settingName, testValue);

            Assert.AreEqual(testValue, cahce.Get(settingName));
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void DialerSettings_SetInboundAudioMessagesDictionary_SetterWorksFine()
        {
            IDialerSettings dialerSettings = ServiceLocator.Resolve<IDialerSettings>();

            var actual = new ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor>();
            actual[AudioMessageType.IncomingCall] = new AudioMessageDescriptor
            {
                Type = AudioSourceType.AudioUrl,
                Source = "http://IncomingCall.mp3"
            };
            actual[AudioMessageType.DropCallOutOfShift] = new AudioMessageDescriptor
            {
                Type = AudioSourceType.AudioUrl,
                Source = "http://DropCallOutOfShift.mp3"
            };

            dialerSettings.InboundAudioMessagesDictionary = actual;

            string inboundAudioMessagesJson = BvSystemSettingsAdapter.GetByCondition("SystemName='Dialer.InboundAudioMessagesJson'")[0].Value;

            var result = JsonConvert.DeserializeObject<ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor>>(inboundAudioMessagesJson);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(actual[AudioMessageType.IncomingCall].Type, result[AudioMessageType.IncomingCall].Type);
            Assert.AreEqual(actual[AudioMessageType.IncomingCall].Source, result[AudioMessageType.IncomingCall].Source);
            Assert.AreEqual(actual[AudioMessageType.DropCallOutOfShift].Type, result[AudioMessageType.DropCallOutOfShift].Type);
            Assert.AreEqual(actual[AudioMessageType.DropCallOutOfShift].Source, result[AudioMessageType.DropCallOutOfShift].Source);
        }

        private void SetDefaultValue(string systemName, string value)
        {
            _framework.DefaultDbEngine.ExecuteNonQuery(
                    @"UPDATE BvSystemSettings SET Value = @value WHERE SystemName = @name", 
                    CommandType.Text,
                    new SqlParameter("@value", value),
                    new SqlParameter("@name", systemName));
        }

        private void OverwriteDefaultValue(string systemName, string value)
        {
            BvSpSystemSetting_UpdateAdapter.ExecuteNonQuery(systemName, value);
        }
    }
}
