using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Confirmit.CATI.Telephony.DialerService;
using Confirmit.CATI.Telephony.DialerService.Contract;
using Confirmit.Test.Common.Attributes;
using DialerCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfirmitDialerInterface;
using ConfirmitDialerInterface.Fakes;

using DialerServiceAlias = Confirmit.CATI.Telephony.DialerService.DialerService;
using Confirmit.CATI.Telephony;
using DialerCommon.Logging;

namespace DialerService.UnitTests
{
    /// <summary>
    /// This is a test class for DialerService and is intended
    /// to contain all DialerService Unit Tests
    /// </summary>
    [TestClass]
    public class DialerServiceTest
    {
        /// <summary>
        /// Note, it's MANUAL test for checking log messages. Remove the 'Ignore' attribute below in order to run the test.
        /// </summary>
        [TestMethod, Owner(@"FIRM\alm"), Cr(72994), Ignore]
        public void EventsLoggingTest()
        {
            var target = new DialerServiceAlias();

            target.NotifyDialerState(7, 1, DialerState.Available);
            target.NotifyAgentState(8, 2, 538, 53, AgentState.LoggedIn);
            target.NotifyOutcome(9, 3, 539, 43, 15, 23, CallOutcome.Busy, null, TimeSpan.Zero, null, null);
            target.ScreenPop(10, 4, 540, 33, 23, 23, DialingMode.Predictive);
            target.RequestCalls("some request id", 11, 5, 541, 23, CallsSelectionAlgorithm.ByPersonGroup, 30);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(73809)]
        public void ServiceStateIsExpired_ServiceStateIsNotTakenIntoAccount()
        {
            //Create the state file
            Directory.CreateDirectory(DialerServiceAppDataPath.GetServiceAppDataPath());
            var dialerState = new DialerServiceState { companyId = 1, dialerId = 2 };
            dialerState.Save(new Logger("DialerServiceTest", new RequestId()));

            Settings.Default["DialerId"] = 0;
            Settings.Default["StatefulMode"] = true;
            Settings.Default["ServiceStateExpirationTimeout"] = 0;

            var dialerService = new DialerServiceAlias();

            Assert.AreEqual(0, dialerService.ServiceState.companyId);
            Assert.AreEqual(0, dialerService.ServiceState.dialerId);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(73809)]
        public void ServiceStateIsNotExpired_ServiceStateIsReadCorrectly()
        {
            // Create the state file
            Directory.CreateDirectory(DialerServiceAppDataPath.GetServiceAppDataPath());
            var dialerState = new DialerServiceState { companyId = 1, dialerId = 2 };
            dialerState.Save(new Logger("DialerServiceTest", new RequestId()));

            Settings.Default["DialerId"] = 0; // This means that the dialer id should be taken from the dialer state file.
            Settings.Default["UseAuthorization"] = false;
            Settings.Default["StatefulMode"] = true;
            Settings.Default["ServiceStateExpirationTimeout"] = 200;

            var dialerService = new DialerServiceAlias();

            Assert.AreEqual(1, dialerService.ServiceState.companyId);
            Assert.AreEqual(2, dialerService.ServiceState.dialerId);
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ExceptionThrownInsideExecuteIsRethrownAsProperFaultException()
        {
            Settings.Default["UseAuthorization"] = false;

            const DialerErrorCode expectedErrorCode = DialerErrorCode.Exception;
            const string expectedExceptionMessage = "Test exception in ExceptionThrownInsideExecuteIsRethrownAsProperFaultException";

            var target = new DialerServiceAlias();

            try
            {
                target.Execute<DialerErrorCode>(
                    1,
                    "DialerServiceTest.ExceptionTrownInsideExecuteIsRethrownAsProperFaultException",
                    "There are no arguments",
                    () => { throw new Exception(expectedExceptionMessage); });

                Assert.Fail("FaultException<DialerExceptionDetail> was expected but not thrown.");
            }
            catch (FaultException<DialerExceptionDetail> ex)
            {
                // The exception is expected

                var faultExceptionDetail = ex.Detail;

                Assert.AreEqual(expectedErrorCode, faultExceptionDetail.ErrorCode, "Error code is wrong.");
                Assert.IsTrue(faultExceptionDetail.ErrorString.Contains(expectedExceptionMessage),
                    string.Format("Error message is wrong. Expected to be contained: [{0}]. Actual: [{1}].",
                    expectedExceptionMessage, faultExceptionDetail.ErrorString));
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void DialerExceptionThrownInsideExecuteIsRethrownAsProperFaultException()
        {
            Settings.Default["UseAuthorization"] = false;

            const DialerErrorCode expectedErrorCode = DialerErrorCode.UnknownSupervisor; // Any other code could be used
            const string expectedExceptionMessage = "Test exception in DialerExceptionThrownInsideExecuteIsRethrownAsProperFaultException";

            var target = new DialerServiceAlias();

            try
            {
                target.Execute<DialerErrorCode>(
                    1,
                    "DialerServiceTest.ExceptionTrownInsideExecuteIsRethrownAsProperFaultException",
                    "There are no arguments",
                    () => { throw new DialerException(expectedErrorCode, expectedExceptionMessage); });

                Assert.Fail("FaultException<DialerExceptionDetail> was expected but not thrown.");
            }
            catch (FaultException<DialerExceptionDetail> ex)
            {
                // The exception is expected

                var faultExceptionDetail = ex.Detail;

                Assert.AreEqual(expectedErrorCode, faultExceptionDetail.ErrorCode, "Error code is wrong.");
                Assert.IsTrue(faultExceptionDetail.ErrorString.Contains(expectedExceptionMessage),
                    string.Format("Error message is wrong. Expected to be contained: [{0}]. Actual: [{1}].",
                    expectedExceptionMessage, faultExceptionDetail.ErrorString));
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ExceptionThrownInsideDoDialerCallIsRethrown()
        {
            Settings.Default["UseAuthorization"] = false;

            var target = new DialerServiceAlias();

            try
            {
                target.DoDialerCall<DialerErrorCode>(() => { throw new Exception("Test exception"); });

                Assert.Fail("Exception was expected but not thrown.");
            }
            catch (Exception ex)
            {
                // The exception is expected
                Assert.AreEqual(typeof(Exception), ex.GetType());
            }
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void Hangup_DialerDriverReturnsWrongStateAgentNotInCall_SuccessIsReturnedToHigherLevel()
        {
            var dialerDriver = new StubIDialerCoreApi
            {
                HangupInt32Int32Int64Int32Int32Int64 = (companyId, dialerId, campaignId, agentId, interviewId, callId) => DialerErrorCode.WrongStateAgentNotInCall
            };

            var warningMessages = new List<string>();

            var logger = new DialerCommon.Logging.Fakes.StubICommonLogger
            {
                WarningStringStringArrayOfObject = (sourceCodeLocation, message, args) => warningMessages.Add(string.Format(message, args))
            };

            var target = new DialerServiceAlias(dialerDriver, logger);

            var result = target.Hangup(1, 2, 3, 4, 5, 6);

            Assert.AreEqual(
                DialerErrorCode.Success,
                result,
                "WrongStateAgentNotInCall is not treated as Success in Hangup at CODI dialer service.");

            Assert.IsTrue(
                warningMessages.Any(warningMessage => warningMessage.Contains(
                    "DialerErrorCode.WrongStateAgentNotInCall is replaced with DialerErrorCode.Success /// companyId=1, dialerId=2, campaignId=3, agentId=4")),
                    "Log message is either absent or incorrect.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SupervisorResourceBindingTypeIsSetInConfig_TheConfiguredValueOverridesParameterPassed()
        {
            const ResourceBindingType expectedBindingType = ResourceBindingType.Local;
            Settings.Default["SupervisorResourceBindingType"] = expectedBindingType.ToString(); // "Local"

            var actualBindingType = (ResourceBindingType)999; // Some incorrect value bu default.

            var dialerDriver = new StubIDialerCoreApi
            {
                StartMonitorInt32Int32Int32StringStringResourceBindingTypeStringRef = (
                    int companyId,
                    int dialerId,
                    int agentId,
                    string supervisorName,
                    string supervisorConnectionString,
                    ResourceBindingType resourceBindingType,
                    ref string sessionId) =>
                {
                    actualBindingType = resourceBindingType;
                    return DialerErrorCode.Success;
                }
            };

            var logger = new DialerCommon.Logging.Fakes.StubICommonLogger();

            var target = new DialerServiceAlias(dialerDriver, logger);

            string dummySessionId = "sessionId";

            target.StartMonitor(
                0 /* companyId */,
                1,
                0 /* agentId */,
                "supervisorName",
                "supervisorConnectionString",
                ResourceBindingType.PhoneNumber, /* Note, the value here should be different than expectedBindingType */
                ref dummySessionId);

            Assert.AreEqual(expectedBindingType, actualBindingType, "Supervisor resource binding type is not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SupervisorResourceBindingTypeIsNotSetInConfig_TheParameterPassedIsUsed()
        {
            const ResourceBindingType expectedBindingType = ResourceBindingType.PhoneNumber;
            Settings.Default["SupervisorResourceBindingType"] = "NotDefined"; // The default value that means the setting is not specified

            var actualBindingType = (ResourceBindingType)999; // Some incorrect value bu default.

            var dialerDriver = new StubIDialerCoreApi
            {
                StartMonitorInt32Int32Int32StringStringResourceBindingTypeStringRef = (
                    int companyId,
                    int dialerId,
                    int agentId,
                    string supervisorName,
                    string supervisorConnectionString,
                    ResourceBindingType resourceBindingType,
                    ref string sessionId) =>
                {
                    actualBindingType = resourceBindingType;
                    return DialerErrorCode.Success;
                }
            };

            var logger = new DialerCommon.Logging.Fakes.StubICommonLogger();

            var target = new DialerServiceAlias(dialerDriver, logger);

            string dummySessionId = "sessionId";

            target.StartMonitor(
                0 /* companyId */,
                1,
                0 /* agentId */,
                "supervisorName",
                "supervisorConnectionString",
                expectedBindingType,
                ref dummySessionId);

            Assert.AreEqual(expectedBindingType, actualBindingType, "Supervisor resource binding type is not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SecurityProtocolsSettingMustBeEmptyByDefault()
        {
            var settings = new Settings();

            var securityProtocolsCollection = (StringCollection)settings["SecurityProtocols"];

            Assert.IsNotNull(securityProtocolsCollection, "securityProtocolsCollection is null");

            Assert.AreEqual(0, securityProtocolsCollection.Count,
                "Security protocol collection is not empty in the default settings: [{0}]",
                string.Join(", ", securityProtocolsCollection.Cast<string>()));
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SecurityProtocolsSettingIsNull_SecurityProtocolsAreTakenFromHost()
        {
            var expectedSecurityProtocols = ServicePointManager.SecurityProtocol;

            Settings.Default["SecurityProtocols"] = null;

            // ReSharper disable once UnusedVariable
            var target = new DialerServiceAlias();

            Assert.AreEqual(expectedSecurityProtocols, ServicePointManager.SecurityProtocol,
                "Security protocols are not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SecurityProtocolsSettingIsEmpty_SecurityProtocolsAreTakenFromHost()
        {
            var expectedSecurityProtocols = ServicePointManager.SecurityProtocol;

            Settings.Default["SecurityProtocols"] = new StringCollection();

            // ReSharper disable once UnusedVariable
            var target = new DialerServiceAlias();

            Assert.AreEqual(expectedSecurityProtocols, ServicePointManager.SecurityProtocol,
                "Security protocols are not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SecurityProtocolsSettingContainsOneProtocol_SecurityProtocolsAreTakenFromTheSettings()
        {
            const SecurityProtocolType expectedSecurityProtocols = SecurityProtocolType.Tls;

            Settings.Default["SecurityProtocols"] = new StringCollection
            {
                expectedSecurityProtocols.ToString()
            };

            // ReSharper disable once UnusedVariable
            var target = new DialerServiceAlias();

            Assert.AreEqual(expectedSecurityProtocols, ServicePointManager.SecurityProtocol,
                "Security protocols are not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SecurityProtocolsSettingContainsSeveralProtocols_SecurityProtocolsAreTakenFromTheSettings()
        {
            var expectedSecurityProtocols = new List<SecurityProtocolType>
            {
                SecurityProtocolType.Ssl3,
                SecurityProtocolType.Tls,
                SecurityProtocolType.Tls11,
                SecurityProtocolType.Tls12
            };

            var settinsSecurityProtocols = new StringCollection();
            settinsSecurityProtocols.AddRange(expectedSecurityProtocols.Select(x => x.ToString()).ToArray());

            Settings.Default["SecurityProtocols"] = settinsSecurityProtocols;

            // ReSharper disable once UnusedVariable
            var target = new DialerServiceAlias();

            Assert.AreEqual(expectedSecurityProtocols.Aggregate((result, x) => result | x), ServicePointManager.SecurityProtocol,
                "Security protocols are not as expected.");
        }


        [TestMethod, Owner(@"FIRM\alm")]
        public void SecurityProtocolsSettingContainsSeveralProtocolsAndSomeUnknownValues_SecurityProtocolsAreTakenFromTheSettingsUnknownValuesAreIgnored()
        {
            var expectedSecurityProtocols = new List<SecurityProtocolType>
            {
                SecurityProtocolType.Ssl3,
                SecurityProtocolType.Tls,
                SecurityProtocolType.Tls11,
                SecurityProtocolType.Tls12
            };

            var settinsSecurityProtocols = new StringCollection()
            {
                "SomeUnknownValue1",
                "SomeUnknownValue2",
                "SomeUnknownValue3",
                "SomeUnknownValue4",
                "SomeUnknownValue5"
            };

            settinsSecurityProtocols.AddRange(expectedSecurityProtocols.Select(x => x.ToString()).ToArray());

            Settings.Default["SecurityProtocols"] = settinsSecurityProtocols;

            // ReSharper disable once UnusedVariable
            var target = new DialerServiceAlias();

            Assert.AreEqual(expectedSecurityProtocols.Aggregate((result, x) => result | x), ServicePointManager.SecurityProtocol,
                "Security protocols are not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SecurityProtocolsSettingContainsUnexpectedValuesOnly_SecurityProtocolsAreTakenFromHost()
        {
            var expectedSecurityProtocols = ServicePointManager.SecurityProtocol;

            Settings.Default["SecurityProtocols"] = new StringCollection
            {
                "SomeUnknownValue1",
                "SomeUnknownValue2",
                "SomeUnknownValue3",
                "SomeUnknownValue4",
                "SomeUnknownValue5"
            };

            // ReSharper disable once UnusedVariable
            var target = new DialerServiceAlias();

            Assert.AreEqual(expectedSecurityProtocols, ServicePointManager.SecurityProtocol,
                "Security protocols are not as expected.");
        }

        [TestMethod, Owner(@"FIRM\olegz")]
        public void GetLogFiles_GetFiles_Success()
        {
            Settings.Default["UseAuthorization"] = false;
            var target = new DialerServiceAlias();
            var result = target.GetLogFiles();

            Assert.IsTrue(result.Any());
        }

        [TestMethod, Owner(@"FIRM\olegz")]
        public void GetLogFiles_GetFileWithNumber_Success()
        {
            Settings.Default["UseAuthorization"] = false;
            var target = new DialerServiceAlias();
            var logger = (Logger)target.Logger;
            var generator = new LogFileNameGenerator(logger.LoggingFileName);
            var targetFileName = generator.GenerateLogFileName(DateTime.Now);

            var extensionPos = targetFileName.LastIndexOf('.');
            var insertPos = extensionPos >= 0 ? extensionPos : targetFileName.Length;
            targetFileName = targetFileName.Insert(insertPos, ".023");
            File.AppendAllText(targetFileName, "Test log file");

            var result = target.GetLogFiles();
            Assert.IsTrue(result.Any(x => x.Name == targetFileName));
        }

        [TestMethod, Owner(@"FIRM\olegz")]
        public void GetLogFileBodyZipped_GetBody_Success()
        {
            Settings.Default["UseAuthorization"] = false;
            var target = new DialerServiceAlias();
            var files = target.GetLogFiles().ToList();
            var file = files.Last();
            var fileBodyZipped = target.GetLogFileBodyZipped(file.Name);
            Assert.IsNotNull(fileBodyZipped);
            var filePath = Path.Combine(Path.GetTempPath(), file.Name);
            try
            {
                using (var zipStream = new MemoryStream(fileBodyZipped))
                {
                    using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                        foreach (var entry in zipArchive.Entries)
                            using (var entryStream = entry.Open())
                            {
                                var path = Path.Combine(Path.GetTempPath(), entry.FullName);
                                using (var fileStream = File.Create(path))
                                    entryStream.CopyTo(fileStream);
                            }
                }

                Assert.IsTrue(File.Exists(filePath));
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }
}
