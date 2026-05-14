using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using DialerCommon.DialerParameters;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DialerSettingsTest
{
    [TestClass]
    public class DialerSettingsTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;
        private IDialerSurveyParametersManager _dialerSurveyParametersManager;
        private ITelephony _telephony;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();

            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _dialerSurveyParametersManager = ServiceLocator.Resolve<IDialerSurveyParametersManager>();
            _telephony = ServiceLocator.Resolve<ITelephony>();
        }

        private static IEnumerable<DialerParameter> CreateParametersList()
        {
            var parameters = new List<DialerParameter>
            {
                new DialerParameter
                {
                    Id = "1",
                    Name = "parameter1",
                    Type = typeof (Int32).FullName,
                    Value = "1"
                },
                new DialerParameter
                {
                    Id = "2",
                    Name = "parameter2",
                    Type = typeof (String).FullName,
                    Value = "string2"
                },
                new DialerParameter
                {
                    Id = "3",
                    Name = "parameter3",
                    Type = typeof (Boolean).FullName,
                    Value = "True"
                },
                new DialerParameter
                {
                    Id = "4",
                    Name = "parameter4",
                    Type = typeof (Decimal).FullName,
                    Value = "1.1"
                }
            };

            return parameters;
        }

        private List<DialerParameter> CloneList(IEnumerable<DialerParameter> list)
        {
            return list.Select(x => new DialerParameter
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Value = x.Value,
                Description = x.Description
            }).ToList();
        }
        
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DialerIsNotConfigured_GetDialerDefaultSurveyParameters_EmptyList()
        {
            var actualParameters = _dialerSurveyParametersManager.GetDialerDefaultSurveyParameters();
            Assert.AreEqual(false, actualParameters.Any());
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DialerHasNoParameters_GetDialerDefaultSurveyParameters_EmptyList()
        {
            InsertDialerEntity();

            Stubs.ExtendExistingIMnTciToolsStub(true);

            var actualParameters = _dialerSurveyParametersManager.GetDialerDefaultSurveyParameters();
            Assert.AreEqual(false, actualParameters.Any());
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void DialerIsConfigured_SetDialerDefaultSurveyParameters_RightParametersAreSet()
        {
            var newParameters = CreateParametersList().ToArray();
            _dialerSurveyParametersManager.SetDialerDefaultSurveyParameters(newParameters);

            //Now read settings from DB and ensure that we set what we want
            var actualParameters = _dialerSurveyParametersManager.GetDialerDefaultSurveyParameters();
            TestAssert.AreEqual(newParameters, actualParameters);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(45244)]
        public void DialerIsNotOperational_SetDialerDefaultSurveyParameters_Succeeded()
        {
            new TestCati2(true, BackendToolsObject);
            _telephony.UninitializeDialers(false);

            var newParameters = CreateParametersList();
            _dialerSurveyParametersManager.SetDialerDefaultSurveyParameters(newParameters); //Must succeed,
            //must NOT throw Dialer interface is [null] exception
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DialerHasNoParameters_GetDialerSurveyParameters_EmptyList()
        {
            InsertDialerEntity();

            Stubs.ExtendExistingIMnTciToolsStub(true);

            const string projectdId = "p0001234";
            int surveySid = BackendToolsObject.CreateSurvey(projectdId);

            var actualParameters = _dialerSurveyParametersManager.GetDialerSurveyParameters(surveySid);

            Assert.AreEqual(false, actualParameters.Any());
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DialerHasParameters_GetDialerSurveyParameters_DefaultParametersList()
        {
            var parameters = CreateParametersList().ToArray();

            InsertDialerEntity();

            Stubs.ExtendExistingIMnTciToolsStub(true);

            RegisterTelephonyStub();

            _dialerSurveyParametersManager.SetDialerDefaultSurveyParameters(parameters);

            const string projectdId = "p0001234";
            int surveySid = BackendToolsObject.CreateSurvey(projectdId);

            var actualParameters = _dialerSurveyParametersManager.GetDialerSurveyParameters(surveySid);

            TestAssert.AreEqual(parameters, actualParameters);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DialerHasParametersAndOverrideForSurvey_GetDialerSurveyParameters_SurveyParametersList()
        {
            var defaultParameters = CreateParametersList().ToArray();

            InsertDialerEntity();

            Stubs.ExtendExistingIMnTciToolsStub(true);

            RegisterTelephonyStub();

            _dialerSurveyParametersManager.SetDialerDefaultSurveyParameters(defaultParameters);

            const string projectdId = "p0001234";
            int surveySid = BackendToolsObject.CreateSurvey(projectdId);

            var surveyParameters = defaultParameters.ToList();
            surveyParameters[0].Value = "2";
            surveyParameters[1].Value = "string2";

            _dialerSurveyParametersManager.SetDialerSurveyParameters(surveySid, surveyParameters);

            var actualParameters = _dialerSurveyParametersManager.GetDialerSurveyParameters(surveySid);

            TestAssert.AreEqual(surveyParameters, actualParameters);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DialerHasParametersAndOverrideForSurvey_ResetDialerSurveyParameters_DefaultParametersList()
        {
            var defaultParameters = CreateParametersList().ToArray();

            InsertDialerEntity();

            Stubs.ExtendExistingIMnTciToolsStub(true);

            RegisterTelephonyStub();

            _dialerSurveyParametersManager.SetDialerDefaultSurveyParameters(defaultParameters);

            const string projectdId = "p0001234";
            var surveySid = BackendToolsObject.CreateSurvey(projectdId);
            var surveyParameters = CloneList(defaultParameters);
            surveyParameters[0].Value = "2";
            surveyParameters[1].Value = "string2";

            _dialerSurveyParametersManager.SetDialerSurveyParameters(surveySid, surveyParameters);
            _dialerSurveyParametersManager.ResetSurveyDialerParametersToDefaultValues(surveySid);

            var actualParameters = _dialerSurveyParametersManager.GetDialerSurveyParameters(surveySid);

            TestAssert.AreEqual(defaultParameters, actualParameters);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(45244)]
        public void DialerIsNotOperational_SetDialerSurveyParametersForOpenedSurvey_DialerMustNotBeCalled()
        {
            var test = new TestCati2(true, BackendToolsObject);

            var parameters = CreateParametersList().ToArray();
            _dialerSurveyParametersManager.SetDialerDefaultSurveyParameters(parameters);

            var surveySid = test.CreateSurvey(null);
            _surveyStateService.Open(surveySid);
            _telephony.UninitializeDialers(false);

            _dialerSurveyParametersManager.SetDialerSurveyParameters(surveySid, parameters); //Must succeed,
            //must NOT throw Dialer interface is [null] exception
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(50054)]
        public void SurveyDialerParametersAreNull_GetSurveyParameters_DefaultSurveyDialerParametersAreAppliedAndReturned()
        {
            var test = new TestCati2(true, BackendToolsObject);

            var parameters = CreateParametersList().ToArray();
            _dialerSurveyParametersManager.SetDialerDefaultSurveyParameters(parameters);

            var surveySid = test.CreateSurvey(null);

            // Set DialerParameters to NULL for all surveys
            using (var connection = new SqlConnection(TestingFramework.DbEngine.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("UPDATE BvSurvey SET DialerParameters = NULL", connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Get dialer survey parameters, check that survey dialer parameters are now not null.
            var surveyDialerParameters = _dialerSurveyParametersManager.GetDialerSurveyParameters(surveySid);
            var xmlSurveyParametersString = DialerParametersSerializer.SerializeDialerParameters(surveyDialerParameters);
            var xmlDefaultParametersString = DialerParametersSerializer.SerializeDialerParameters(parameters);
            Assert.AreEqual(xmlDefaultParametersString, xmlSurveyParametersString);

            // Check that survey dialer parameters are now not null in DB
            using (var connection = new SqlConnection(TestingFramework.DbEngine.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT DialerParameters FROM BvSurvey WHERE SID = " + surveySid, connection))
                {
                    SqlDataReader sdr = command.ExecuteReader();
                    Assert.IsTrue(sdr.Read());
                    var expected = sdr["DialerParameters"];
                    Assert.AreEqual(expected, xmlDefaultParametersString);
                }
            }
        }

        private void RegisterTelephonyStub()
        {
            var stubITelephony = new StubITelephony
            {
                Inner = _telephony,
                ValidateCampaignParametersString = xml => DialerErrorCode.Success
            };

            ServiceLocator.RegisterInstance<ITelephony>(stubITelephony);
        }

        private static void InsertDialerEntity()
        {
            var dialerEntity = new BvDialersEntity
            {
                Id = 1,
                Name = "SomeDialer"
            };

            BvDialersAdapter.Insert(dialerEntity);
        }
    }
}
