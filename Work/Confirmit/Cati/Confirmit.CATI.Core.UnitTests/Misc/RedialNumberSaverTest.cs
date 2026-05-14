using System;
using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Misc
{
    [TestClass]
    public class RedialNumberSaverTest
    {
        private const string TelephoneNumber = "123456789";
        private const string RedialVariableName = "alternativenumber";

        private RedialNumberSaver _redialNumberSaver;

        private StubIInterviewFormDataSourceService.SetFormValueFormDescBaseStringArrayOfStringStringDelegate _setFormValueAction = (desc, cat, loops, val) => { };

        [TestMethod, Owner(@"firm\DenisM")]
        public void SaveAlternativeNumber_SaveNewNumber_ValueSet()
        {
            var isValueSet = false;
            _setFormValueAction = (desc, cat, loops, val) => { isValueSet = true; };
            PrepareStubs(RedialVariableName, isVariableExists: true, isOpen: true);

            RunSaveAlternativeNumber("ALTERNATIVENUMBER");

            Assert.IsTrue(isValueSet);
        }

        [TestMethod, Owner(@"firm\DenisM")]
        public void SaveAlternativeNumber_SaveTheSameNumber_ValueNotSet()
        {
            var isValueSet = false;
            _setFormValueAction = (desc, cat, loops, val) => { isValueSet = true; };
            PrepareStubs(RedialVariableName, isVariableExists: true, isOpen: true);

            RunSaveAlternativeNumber();

            Assert.IsFalse(isValueSet);
        }

        [TestMethod, Owner(@"firm\DenisM")]
        public void SaveAlternativeNumber_SaveNumberValueNotExists_ValueNotSet()
        {
            var isValueSet = false;
            _setFormValueAction = (desc, cat, loops, val) => { isValueSet = true; };
            PrepareStubs(RedialVariableName, isVariableExists: false, isOpen: true);

            RunSaveAlternativeNumber("ALTERNATIVENUMBER");

            Assert.IsFalse(isValueSet);
        }

        [TestMethod, Owner(@"firm\DenisM")]
        public void SaveAlternativeNumber_SaveNewNumberValueIsLessThanLengthAllowed_ValueSet()
        {
            var numberToSave = string.Empty;
            _setFormValueAction = (desc, cat, loops, val) => { numberToSave = val; };
            PrepareStubs(RedialVariableName, isVariableExists: true, isOpen: true, fieldWidth: 4);

            RunSaveAlternativeNumber("ALTERNATIVENUMBER");

            Assert.IsTrue(numberToSave.Length == 4);
            Assert.AreEqual(TelephoneNumber.Substring(0, 4), numberToSave);
        }

        [TestMethod, Owner(@"firm\DenisM")]
        public void SaveAlternativeNumber_SaveNewNumberFieldWidthIsNegative_ValueSet()
        {
            var numberToSave = string.Empty;
            _setFormValueAction = (desc, cat, loops, val) => { numberToSave = val; };
            PrepareStubs(RedialVariableName, isVariableExists: true, isOpen: true, fieldWidth: -1);

            RunSaveAlternativeNumber("ALTERNATIVENUMBER");

            Assert.AreEqual(TelephoneNumber + "ALTERNATIVENUMBER", numberToSave);
        }

        [TestMethod, Owner(@"firm\DenisM")]
        public void SaveAlternativeNumber_SaveNewNumberValueIsNotOpen_ValueNotSet()
        {
            var isValueSet = false;
            _setFormValueAction = (desc, cat, loops, val) => { isValueSet = true; };
            PrepareStubs(RedialVariableName, isVariableExists: true, isOpen: false);

            RunSaveAlternativeNumber("ALTERNATIVENUMBER");

            Assert.IsFalse(isValueSet);
        }

        private void RunSaveAlternativeNumber(string telephoneNumber = "")
        {
            _redialNumberSaver.SaveAlternativeNumber(1, TelephoneNumber + telephoneNumber, 1);
        }

        private void PrepareStubs(string valueName, bool isVariableExists, bool isOpen,VariableDataType variableType= VariableDataType.Hidden ,int fieldWidth = 100)
        {
            var stubIInterviewRepository = new StubIInterviewRepository();
            stubIInterviewRepository.GetByIdInt32Int32 = (i, i1) => new BvInterviewWithOriginEntity(new BvInterviewEntity
            {
                TelephoneNumber = TelephoneNumber
            });

            var stubISurveyMetadataCacheService = new StubISurveyMetadataCacheService();
            var stubISurveyMetadataCache =  new StubISurveyMetadataCache();

            FormDescBase formDescription;
            var fields = new[]
            {
                new SurveyDatabaseFieldInfo
                {
                    FieldName = valueName,
                    TableName = "response0"
                }
            };
            var formtexts = new[] { new FormText { Title = "t1" } };

            if (isOpen)
            {
                formDescription = FormDescBase.CreateInstance(1, "p0000000123",
                            new OpenForm { FormTexts = formtexts },
                            new SurveyDatabaseFormInfo
                            {
                                Name = valueName,
                                Fields = fields,
                                LoopPath = new[] { "responseid" }

                            });

                formDescription.FieldWidth = fieldWidth;
            }
            else
            {
                formDescription = FormDescBase.CreateInstance(1, "p0000000123",
                    new SingleForm
                    {
                        FormTexts = formtexts,
                        SingleAnswers =
                            new SingleAnswers
                            {
                                Items = new AnswerBase[] { new Answer { Precode = "9" }, new Answer { Precode = "25" } }
                            }
                    },
                    new SurveyDatabaseFormInfo
                    {
                        Name = valueName,
                        Fields = fields,
                        LoopPath = new[] { "responseid" }
                    });
            }

            formDescription.VariableType = variableType;

            if (isVariableExists)
            {
                stubISurveyMetadataCache.GetFormDescString = (s) => formDescription;
            }
            else
            {
                stubISurveyMetadataCache.GetFormDescString = (s) => null;
            }

            var formDataService = new StubIInterviewFormDataSourceService();
            var factory = new StubIInterviewDataServiceFactory();
            var respondentDataService = new StubIInterviewRespondentDataSourceService();

            stubISurveyMetadataCacheService.GetInt32 = i => stubISurveyMetadataCache;

            factory.CreateFormServiceInt32Int32 = (s, i) => formDataService;
            factory.CreateRespondentServiceInt32Int32 = (s, i) => respondentDataService;

            _redialNumberSaver = new RedialNumberSaver(
                stubISurveyMetadataCacheService,
                factory,
                stubIInterviewRepository);

            formDataService.SetFormValueFormDescBaseStringArrayOfStringString = _setFormValueAction;
        }
    }
}
