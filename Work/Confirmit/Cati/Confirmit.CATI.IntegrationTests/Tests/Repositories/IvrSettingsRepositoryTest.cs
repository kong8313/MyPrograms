using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Repositories
{
    [TestClass]
    public class IvrSettingsRepositoryTest : BaseMockedIntegrationTest
    {
        private IIvrSettingsRepository _ivrSettingsRepository;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();

            _ivrSettingsRepository = ServiceLocator.Resolve<IIvrSettingsRepository>();
        }

        private void CompareIvrSettings(BvIvrSettingsEntity expectedIvrSettings, BvIvrSettingsEntity actualIvrSettings)
        {
            Assert.AreEqual(expectedIvrSettings.LanguageId, actualIvrSettings.LanguageId);
            Assert.AreEqual(expectedIvrSettings.LanguageDescription, actualIvrSettings.LanguageDescription);
            Assert.AreEqual(expectedIvrSettings.WrongInputText, actualIvrSettings.WrongInputText);
            Assert.AreEqual(expectedIvrSettings.WrongInputAudioUrl, actualIvrSettings.WrongInputAudioUrl);
            Assert.AreEqual(expectedIvrSettings.WrongInputExitText, actualIvrSettings.WrongInputExitText);
            Assert.AreEqual(expectedIvrSettings.WrongInputExitAudioUrl, actualIvrSettings.WrongInputExitAudioUrl);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void Insert_InsertNewIvrSetting_SettingsAreAddedCorrectly()
        {
            var expectedIvrSettings = new BvIvrSettingsEntity
            {
                LanguageId = 9,
                LanguageDescription = "English, en",
                WrongInputText = "Wrong input text",
                WrongInputAudioUrl = "http://WrongInputAudioUrl.ru",
                WrongInputExitText = "Wrong input exit text",
                WrongInputExitAudioUrl = "http://WrongInputExitAudioUrl"
            };

            _ivrSettingsRepository.Insert(expectedIvrSettings);

            var actualIvrSettings = _ivrSettingsRepository.TryGetByLanguageId(expectedIvrSettings.LanguageId);

            CompareIvrSettings(expectedIvrSettings, actualIvrSettings);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void Update_InsertNewIvrSettingAndChangeAllFields_SettingsAreUpdatesCorrectly()
        {
            var initilIvrSettings = new BvIvrSettingsEntity
            {
                LanguageId = 9,
                LanguageDescription = "English, en",
                WrongInputText = "Wrong input text",
                WrongInputAudioUrl = "http://WrongInputAudioUrl.ru",
                WrongInputExitText = "Wrong input wxit text",
                WrongInputExitAudioUrl = "http://WrongInputExitAudioUrl"
            };

            _ivrSettingsRepository.Insert(initilIvrSettings);

            var expectedIvrSettings = new BvIvrSettingsEntity
            {
                LanguageId = 8,
                LanguageDescription = "Greek, el",
                WrongInputText = "Changes wrong input text",
                WrongInputAudioUrl = "http://ChangedWrongInputAudioUrl.ru",
                WrongInputExitText = "Changes wrong input exit text",
                WrongInputExitAudioUrl = "http://ChangedWrongInputExitAudioUrl"
            };

            _ivrSettingsRepository.Update(initilIvrSettings.LanguageId, expectedIvrSettings);

            var actualIvrSettings = _ivrSettingsRepository.TryGetByLanguageId(expectedIvrSettings.LanguageId);

            CompareIvrSettings(expectedIvrSettings, actualIvrSettings);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void Delete_InsertTreeIvrSettingAndDeleteTwoOfFirst_SettingsAreRemovedCorrectly()
        {
            for (int i = 0; i < 3; i++)
            {
                var ivrSettings = new BvIvrSettingsEntity
                {
                    LanguageId = 1 + i,
                    LanguageDescription = "LanguageDescription " + i,
                    WrongInputText = "Wrong input text" + i,
                    WrongInputAudioUrl = "http://WrongInputAudioUrl.ru" + i,
                    WrongInputExitText = "Wrong input wxit text" + i,
                    WrongInputExitAudioUrl = "http://WrongInputExitAudioUrl" + i
                };

                _ivrSettingsRepository.Insert(ivrSettings);
            }

            _ivrSettingsRepository.Delete(new List<int> { 1, 3 });

            var allIvrSettings = _ivrSettingsRepository.GetAll();

            Assert.AreEqual(1, allIvrSettings.Count);
            Assert.AreEqual(2, allIvrSettings[0].LanguageId);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void GetAllAndTryGetByLanguageId_GetIvrSettingsFromAmptyTable_CorrectResultsAreReturned()
        {
            var allIvrSettings = _ivrSettingsRepository.GetAll();

            Assert.AreEqual(0, allIvrSettings.Count);

            var ivrSettings = _ivrSettingsRepository.TryGetByLanguageId(1);

            Assert.IsNull(ivrSettings);
        }


    }
}