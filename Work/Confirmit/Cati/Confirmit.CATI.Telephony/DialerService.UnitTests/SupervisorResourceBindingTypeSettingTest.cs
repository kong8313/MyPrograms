using System;
using Confirmit.CATI.Telephony.DialerService;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerService.UnitTests
{
    /// <summary>
    /// This is a test class for SupervisorResourceBindingTypeSetting and is intended
    /// to contain all SupervisorResourceBindingTypeSetting Unit Tests
    /// </summary>
    [TestClass]
    public class SupervisorResourceBindingTypeSettingTest
    {
        [TestMethod, Owner(@"FIRM\alm")]
        public void SupervisorResourceBindingTypeIsSetInConfig_IsSetIsTrueAndGetReturnsCorrectValue()
        {
            const ResourceBindingType expectedBindingType = ResourceBindingType.PhoneNumber;
            Settings.Default["SupervisorResourceBindingType"] = expectedBindingType.ToString();

            var target = new SupervisorResourceBindingTypeSetting(null);

            Assert.IsTrue(target.IsSet, "IsSet must be 'true' as the SupervisorResourceBindingType is specified.");
            Assert.AreEqual(expectedBindingType, target.Get(), "Supervisor resource binding type is not as expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SupervisorResourceBindingTypeIsNotSetInConfigOrNotValid_IsSetIsFalse()
        {
            var logger = new ConfirmitDialerInterface.Fakes.StubILogger();

            var testCases = new[]
            {
                "NotDefined", // The default value that means the setting is not specified
                "SomeIncorrectTextValue",
                "999"
            };

            foreach (var testCase in testCases)
            {
                Settings.Default["SupervisorResourceBindingType"] = testCase;

                var target = new SupervisorResourceBindingTypeSetting(logger);

                Assert.IsFalse(target.IsSet, "IsSet must be 'false' as the SupervisorResourceBindingType is not specified.");
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void IsSetIsFalse_GetThrewsException()
        {
            Settings.Default["SupervisorResourceBindingType"] = "NotDefined"; // The default value that means the setting is not specified

            var target = new SupervisorResourceBindingTypeSetting(null);

            Assert.IsFalse(target.IsSet, "IsSet must be 'false'.");

            try
            {
                var result = target.Get();

                Assert.Fail("DialerException was expected but not thrown. Result binding type is [{0}]", result);
            }
            catch (DialerException ex)
            {
                // The DialerException is expected
                Assert.AreEqual(DialerErrorCode.Exception, ex.ErrorCode, "Wrong error code.");
            }
        }
    }
}
