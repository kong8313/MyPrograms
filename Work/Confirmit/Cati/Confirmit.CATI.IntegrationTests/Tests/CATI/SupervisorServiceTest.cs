using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.SupervisorService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using DialerCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class SupervisorServiceTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"Firm\OlegM")]
        public void ConfigureInboundDdiNumbers_should_configure_success_not_soft_deleted_surveys()
        {
            // arrange
            var inboundCallNumberOpenSurvey = Guid.NewGuid().ToString();
            var inboundCallNumberDeletedSurvey = Guid.NewGuid().ToString();

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1",
                        IsOpen = true,
                        DialMode = DialingMode.Predictive,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumberOpenSurvey}
                        },

                    },
                    new SurveyData()
                    {
                        Tag = "S2",
                        IsOpen = true,
                        DialMode = DialingMode.Predictive,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumberDeletedSurvey}
                        }
                    }
                },
                Dialers = new[]
                {
                    new DialerData() {Tag = "D1"}
                }
            }.Create();

            var resultDialerId = 0;
            InboundDdiNumber[] resultInboundDDiNumbers = null;

            var survey = context.GetSurvey("S2");
            survey.Model.State = (int)SurveyState.SoftDeleted;

            var dialer = context.GetDialer("D1");
            dialer.DialerHelper.SetBehaviorForConfigureInboundDdiNumbers((args) =>
            {
                resultDialerId = args.TenantId;
                resultInboundDDiNumbers = args.InboundDDINumbers;
                return new[] { DialerErrorCode.Success };
            });

            var service = new SupervisorService();
            // act
            service.ConfigureInboundDdiNumbers(dialer.Id);
            // assert
            Assert.AreEqual(dialer.Model.TenantId, resultDialerId);
            Assert.AreEqual(1, resultInboundDDiNumbers.Length);
            Assert.AreEqual(inboundCallNumberOpenSurvey, resultInboundDDiNumbers[0].Number);
        }

        [TestMethod, Owner(@"Firm\OlegM")]
        [ExpectedException(typeof(UserMessageException))]
        public void ConfigureInboundDdiNumbers_disabled_inbound_should_fail()
        {
            // arrange
            // arrange
            var inboundCallNumberOpenSurvey = Guid.NewGuid().ToString();

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, false}
                },
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1",
                        IsOpen = true,
                        DialMode = DialingMode.Predictive,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumberOpenSurvey}
                        },

                    }
                },
                Dialers = new[]
                {
                    new DialerData() {Tag = "D1"}
                }
            }.Create();

            var dialer = context.GetDialer("D1");
            dialer.DialerHelper.SetBehaviorForConfigureInboundDdiNumbers(args => new[] { DialerErrorCode.Success });

            var service = new SupervisorService();
            // act
            service.ConfigureInboundDdiNumbers(dialer.Id);

            // assert
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void ConfigureInboundDdiNumbers_should_configure_success_not_deleted_surveys()
        {
            // arrange
            var inboundCallNumberOpenSurvey = Guid.NewGuid().ToString();
            var inboundCallNumberDeletedSurvey = Guid.NewGuid().ToString();

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsOpen = true,
                        DialMode = DialingMode.Predictive,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumberOpenSurvey}
                        }

                    },
                    new SurveyData
                    {
                        Tag = "S2",
                        IsOpen = true,
                        DialMode = DialingMode.Predictive,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumberDeletedSurvey}
                        }
                    }
                },
                Dialers = new[]
                {
                    new DialerData {Tag = "D1"}
                }
            }.Create();

            var resultDialerId = 0;
            InboundDdiNumber[] resultInboundDDiNumbers = null;

            var survey = context.GetSurvey("S2");
            BackendTools.DeleteSurvey(survey.Model.ProjectId);

            var dialer = context.GetDialer("D1");
            dialer.DialerHelper.SetBehaviorForConfigureInboundDdiNumbers(args =>
            {
                resultDialerId = args.TenantId;
                resultInboundDDiNumbers = args.InboundDDINumbers;
                return new[] { DialerErrorCode.Success };
            });

            var service = new SupervisorService();
            // act
            service.ConfigureInboundDdiNumbers(dialer.Id);
            // assert
            Assert.AreEqual(dialer.Model.TenantId, resultDialerId);
            Assert.AreEqual(1, resultInboundDDiNumbers.Length);
            Assert.AreEqual(inboundCallNumberOpenSurvey, resultInboundDDiNumbers[0].Number);
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void GetLogFiles_ShouldGiveFileInfoList_Success()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1"}
                }
            }.Create();
            var dialer = context.GetDialer("D1");
            dialer.DialerHelper.SetBehaviorForGetLogFiles(() =>
                new[] { new LogFileInfo("test.log", 0, DateTime.UtcNow, DateTime.UtcNow) });

            var service = new SupervisorService();
            // act
            var target = service.GetLogFiles(dialer.Id);
            // assert
            Assert.IsNotNull(target);
            Assert.IsTrue(target.Any());
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void GetLogFileBodyZipped_ShouldGiveFileBody_Success()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1"}
                }
            }.Create();
            var dialer = context.GetDialer("D1");
            dialer.DialerHelper.SetBehaviorForGetLogFileBodyZipped((fileName) =>
            {
                var fileInfos = new[] { new LogFileInfo("test.log", 0, DateTime.UtcNow, DateTime.UtcNow) };
                return fileInfos.Any(e => string.Equals(e.Name, fileName, StringComparison.InvariantCultureIgnoreCase))
                    ? new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
                    : null;
            });

            var service = new SupervisorService();
            // act
            var target = service.GetLogFileBodyZipped(dialer.Id, "test.log");
            // assert
            Assert.IsNotNull(target);
            Assert.IsTrue(target.Any());
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void GetAvailableExtendedFunctionality_ShouldGiveIsLogGetterSupported_Success()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1", DialerVersion = new Version(3,6,9).ToString()}
                }
            }.Create();
            var dialer = context.GetDialer("D1");

            var service = new SupervisorService();
            // act
            var target = service.GetAvailableExtendedFunctionality(dialer.Id);
            // assert
            Assert.IsNotNull(target);
            Assert.IsTrue(target.IsLogGetterSupported);
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void GetAvailableExtendedFunctionality_ShouldGiveIsNotLogGetterSupported_Success()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1", DialerVersion = new Version(3,6,8).ToString()}
                }
            }.Create();
            var dialer = context.GetDialer("D1");

            var service = new SupervisorService();
            // act
            var target = service.GetAvailableExtendedFunctionality(dialer.Id);
            // assert
            Assert.IsNotNull(target);
            Assert.IsFalse(target.IsLogGetterSupported);
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void GetDialerSupportedFeatures_ShouldGiveIsIVRSupported_Success()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1", Features = new DialerFeatures{IsIVRSupported = true}}
                }
            }.Create();
            var dialer = context.GetDialer("D1");

            var service = new SupervisorService();
            // act
            var target = service.GetDialerSupportedFeatures(dialer.Id);
            // assert
            Assert.IsNotNull(target);
            Assert.IsTrue(target.IsIVRSupported == true, "Dialer feature is unsupported or has wrong value");
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void GetDialerUnsupportedFeatures_ShouldGiveIsIVRSupported_Null()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1", Features = null}
                }
            }.Create();
            var dialer = context.GetDialer("D1");

            var service = new SupervisorService();
            // act
            var target = service.GetDialerSupportedFeatures(dialer.Id);
            // assert
            Assert.IsNotNull(target);
            Assert.IsNull(target.IsIVRSupported, "Dialer feature must be unsupported but has wrong value");
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void GetOverridenDialerSupportedFeatures_ShouldGiveIsIVRSupported_Success()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1", Features = new DialerFeatures{IsIVRSupported = true}}
                }
            }.Create();
            var dialer = context.GetDialer("D1");

            var service = new SupervisorService();
            // act
            var target = service.GetOverridenDialerSupportedFeatures(dialer.Id);
            var targetConfirm = service.GetDialerSupportedFeatures(dialer.Id);
            // assert
            Assert.IsNotNull(target);
            var targetFeature = target.FirstOrDefault(x => x.Name == "IsIVRSupported");
            Assert.IsNotNull(targetFeature);
            Assert.IsNull(targetFeature.OverridenValue, "Dialer feature mustn't have overriden value");
            Assert.IsTrue(targetFeature.DefaultValue == true, "Dialer feature is unsupported or has wrong value");
            Assert.IsTrue(targetConfirm.IsIVRSupported == true, "Dialer feature is unsupported or has wrong value");
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void GetOverridenDialerUnsupportedFeatures_ShouldGiveIsIVRSupported_Null()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1", Features = null}
                }
            }.Create();
            var dialer = context.GetDialer("D1");

            var service = new SupervisorService();
            // act
            var target = service.GetOverridenDialerSupportedFeatures(dialer.Id);
            var targetConfirm = service.GetDialerSupportedFeatures(dialer.Id);
            // assert
            Assert.IsNotNull(target);
            var targetFeature = target.FirstOrDefault(x => x.Name == "IsIVRSupported");
            Assert.IsNotNull(targetFeature);
            Assert.IsNull(targetFeature.OverridenValue, "Dialer feature mustn't have overriden value");
            Assert.IsNull(targetFeature.DefaultValue, "Dialer feature must be unsupported but has wrong value");
            Assert.IsNull(targetConfirm.IsIVRSupported, "Dialer feature must be unsupported but has wrong value");
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void UpdateOverridenDialerSupportedFeature_SetFalse_ShouldGiveOverridenIsIVRSupported_Success()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1", Features = new DialerFeatures{IsIVRSupported = true}}
                }
            }.Create();
            var dialer = context.GetDialer("D1");

            var service = new SupervisorService();
            // act
            service.UpdateOverridenDialerSupportedFeature(dialer.Id, "IsIVRSupported", false);
            var target = service.GetOverridenDialerSupportedFeatures(dialer.Id);
            var targetConfirm = service.GetDialerSupportedFeatures(dialer.Id);
            // assert
            Assert.IsNotNull(target);
            var targetFeature = target.FirstOrDefault(x => x.Name == "IsIVRSupported");
            Assert.IsNotNull(targetFeature);
            Assert.IsNotNull(targetFeature.OverridenValue, "Dialer feature must have overriden value");
            Assert.IsFalse(targetFeature.OverridenValue.Value, "Dialer feature overriden value isn't present or has wrong value");
            Assert.IsTrue(targetFeature.DefaultValue == true, "Dialer feature is unsupported or has wrong value");
            Assert.IsTrue(targetConfirm.IsIVRSupported == false, "Dialer feature is unsupported or has wrong value");
        }


        [TestMethod, Owner(@"firm\olegz")]
        public void UpdateOverridenDialerUnsupportedFeature_SetFalse_ShouldGiveOverridenIsIVRSupported_Success()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1", Features = new DialerFeatures()}
                }
            }.Create();
            var dialer = context.GetDialer("D1");

            var service = new SupervisorService();
            // act
            service.UpdateOverridenDialerSupportedFeature(dialer.Id, "IsIVRSupported", false);
            var target = service.GetOverridenDialerSupportedFeatures(dialer.Id);
            var targetConfirm = service.GetDialerSupportedFeatures(dialer.Id);
            // assert
            Assert.IsNotNull(target);
            var targetFeature = target.FirstOrDefault(x => x.Name == "IsIVRSupported");
            Assert.IsNotNull(targetFeature);
            Assert.IsNotNull(targetFeature.OverridenValue, "Dialer feature must have overriden value");
            Assert.IsFalse(targetFeature.OverridenValue.Value, "Dialer feature overriden value isn't present or has wrong value");
            Assert.IsNull(targetFeature.DefaultValue, "Dialer feature must be unsupported but has wrong value");
            Assert.IsTrue(targetConfirm.IsIVRSupported == false, "Dialer feature is unsupported or has wrong value");
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void UpdateOverridenDialerSupportedFeature_SetNull_ShouldGiveDefaultIsIVRSupported_Success()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1", Features = new DialerFeatures{IsIVRSupported = true}}
                }
            }.Create();
            var dialer = context.GetDialer("D1");

            var service = new SupervisorService();
            // act
            service.UpdateOverridenDialerSupportedFeature(dialer.Id, "IsIVRSupported", null);
            var target = service.GetOverridenDialerSupportedFeatures(dialer.Id);
            var targetConfirm = service.GetDialerSupportedFeatures(dialer.Id);
            // assert
            Assert.IsNotNull(target);
            var targetFeature = target.FirstOrDefault(x => x.Name == "IsIVRSupported");
            Assert.IsNotNull(targetFeature);
            Assert.IsNull(targetFeature.OverridenValue, "Dialer feature mustn't have overriden value");
            Assert.IsTrue(targetFeature.DefaultValue == true, "Dialer feature is unsupported or has wrong value");
            Assert.IsTrue(targetConfirm.IsIVRSupported == true, "Dialer feature is unsupported or has wrong value");
        }

    }
}