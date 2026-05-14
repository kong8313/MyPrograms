using System;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Misc
{
    [TestClass]
    public class LanguageVariableProviderTest
    {
        private LanguageVariableProvider _languageVariableProvider;

        private StubIInterviewRespondentDataSourceService.GetRespondentValueStringDelegate _getRespondentValueFunc = (arg3) => string.Empty;

        [TestMethod, Owner(@"firm\DenisM")]
        public void GetLanguageForInterview_LanguageFieldNotExistsInRespondentTable_ReturnsNull()
        {
            _getRespondentValueFunc = (arg3) => "9";
            PrepareStubs(false);

            var value = _languageVariableProvider.GetLanguageForInterview(1, 1);

            Assert.IsNull(value);
        }

        [TestMethod, Owner(@"firm\DenisM")]
        public void GetLanguageForInterview_LanguageVariableExistsContainsNonInt_Success()
        {
            _getRespondentValueFunc = (arg3) => "english";
            PrepareStubs(true);

            var value = _languageVariableProvider.GetLanguageForInterview(1, 1);

            Assert.IsNull(value);
        }

        [TestMethod, Owner(@"firm\DenisM")]
        public void GetLanguageForInterview_LanguageVariableExistsContainsIntValue_Success()
        {
            _getRespondentValueFunc = (arg3) => "9";
            PrepareStubs(true);

            var value = _languageVariableProvider.GetLanguageForInterview(1, 1);

            Assert.AreEqual(9, value);
        }

        [TestMethod, Owner(@"firm\DenisM")]
        public void GetLanguageForInterview_ExceptionWasThrownDuringGetRespondentValue_ReturnsNull()
        {
            _getRespondentValueFunc = (arg3) =>
            {
                throw new Exception("Invalid column name 'language'.");
            };
            PrepareStubs(true);

            var value = _languageVariableProvider.GetLanguageForInterview(1, 1);

            Assert.IsNull(value);
        }

        [TestMethod, Owner(@"firm\DenisM")]
        public void GetLanguageForInterview_LanguageVariableExistsContainsNull_Success()
        {
            _getRespondentValueFunc = (arg3) => null;
            PrepareStubs(true);

            var value = _languageVariableProvider.GetLanguageForInterview(1, 1);

            Assert.IsNull(value);
        }

        [TestMethod, Owner(@"firm\DenisM")]
        public void GetLanguageForInterview_LanguageVariableExistsContainsEmpty_Success()
        {
            _getRespondentValueFunc = (arg3) => "";
            PrepareStubs(true);

            var value = _languageVariableProvider.GetLanguageForInterview(1, 1);

            Assert.IsNull(value);
        }

        private void PrepareStubs(bool shouldContainsInCache)
        {
            var stubISurveyMetadataCacheService = new StubISurveyMetadataCacheService();
            var stubISurveyMetadataCache =  new StubISurveyMetadataCache();

            if (shouldContainsInCache)
            {
                stubISurveyMetadataCache.GetRespondentFieldDescString =
                    s => new SurveyDatabaseFieldInfo() {FieldName = "language", TableName = "respondent"};
            }
            else
            {
                stubISurveyMetadataCache.GetRespondentFieldDescString = s => null;
            }


            var dataDatabaseService = new StubIInterviewRespondentDataSourceService()
            {
                GetRespondentValueString = _getRespondentValueFunc
            };

            var dataServiceFactory = new StubIInterviewDataServiceFactory()
            {
                CreateRespondentServiceInt32Int32 = (s, i) => dataDatabaseService
            };

            stubISurveyMetadataCacheService.GetInt32 = i => stubISurveyMetadataCache;

            _languageVariableProvider = new LanguageVariableProvider(
                stubISurveyMetadataCacheService,
                dataServiceFactory);
        }
    }
}
