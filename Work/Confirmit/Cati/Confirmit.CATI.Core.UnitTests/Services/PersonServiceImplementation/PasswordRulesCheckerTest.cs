using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Services.PersonServiceImplementation
{
    [TestClass]
    public class PasswordRulesCheckerTest
    {
        private ServiceLocator _serviceLocator;
        private IPasswordRulesChecker _passwordRulesChecker;
        private const string GoodPassword = "dflkjhGgGoL^&*(2";
        private const string SimplePassword1 = "password1";
        private const string SimplePassword2 = "password1A";
        private const string SimplePassword3 = "password1%";
        private const string EnoughComplexPassword = "Password1%";
        private const string ShortEnoughComplexPassword = "P%";

        [TestInitialize]
        public void TestInitialize()
        {
            _serviceLocator = new ServiceLocator();
            _serviceLocator.Cleanup();
            _serviceLocator.Initialize();
            new SystemSettingUnitTestRegistrator().RegisterTypes(_serviceLocator);

            _passwordRulesChecker = new PasswordRulesChecker();
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void ResetToSamePasswordDisabled_TheSamePasswordEntered_Success()
        {
            IInterviewerPasswordSettings interviewerPasswordSettings = new StubIInterviewerPasswordSettings()
            {
                IsResetToSamePasswordEnabledGet = () => true
            };

            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, GoodPassword, interviewerPasswordSettings);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        [ExpectedException(typeof(TheSamePasswordException))]
        public void ResetToSamePasswordEnabled_TheSamePasswordEntered_Fail()
        {
            IInterviewerPasswordSettings interviewerPasswordSettings = new StubIInterviewerPasswordSettings()
            {
                IsResetToSamePasswordEnabledGet = () => false
            };

            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, GoodPassword, interviewerPasswordSettings);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void MinimumPasswordLengthNotEnforced_ShortPasswordsEntered_Success()
        {
            IInterviewerPasswordSettings interviewerPasswordSettings = new StubIInterviewerPasswordSettings()
            {
                IsResetToSamePasswordEnabledGet = () => true,
                IsMinimumPasswordLengthEnforcedGet = () => false,
                MinimumPasswordLengthGet = () => 8
            };

            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, null, interviewerPasswordSettings);
            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, string.Empty, interviewerPasswordSettings);
            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, "1", interviewerPasswordSettings);
            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, "1234567", interviewerPasswordSettings);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void MinimumPasswordLengthEnforced_DifferentPasswords_HandledCorrectly()
        {
            IInterviewerPasswordSettings interviewerPasswordSettings = new StubIInterviewerPasswordSettings()
            {
                IsResetToSamePasswordEnabledGet = () => true,
                IsMinimumPasswordLengthEnforcedGet = () => true,
                MinimumPasswordLengthGet = () => 8
            };

            CheckPasswordLengthIsHandledCorrectly(null, interviewerPasswordSettings, false);
            CheckPasswordLengthIsHandledCorrectly(null, interviewerPasswordSettings, false);
            CheckPasswordLengthIsHandledCorrectly("1234567", interviewerPasswordSettings, false);
            CheckPasswordLengthIsHandledCorrectly("12345678", interviewerPasswordSettings, true);
            CheckPasswordLengthIsHandledCorrectly("1234567890abcdefghijklmnopqrstuvwxyz", interviewerPasswordSettings, true);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void IsComplexPasswordNotEnforced_AnyPasswordEntered_Success()
        {
            IInterviewerPasswordSettings interviewerPasswordSettings = new StubIInterviewerPasswordSettings()
            {
                IsResetToSamePasswordEnabledGet = () => true,
                IsMinimumPasswordLengthEnforcedGet = () => false,
                IsComplexPasswordEnforcedGet = () => false
            };

            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, null, interviewerPasswordSettings);
            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, string.Empty, interviewerPasswordSettings);
            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, "          ", interviewerPasswordSettings);
            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, SimplePassword1, interviewerPasswordSettings);
            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, SimplePassword2, interviewerPasswordSettings);
            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, SimplePassword3, interviewerPasswordSettings);
            _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, GoodPassword, interviewerPasswordSettings);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void IsComplexPasswordEnforced_DifferentPasswordsEntered_HandledCorrectly()
        {
            IInterviewerPasswordSettings interviewerPasswordSettings = new StubIInterviewerPasswordSettings()
            {
                IsResetToSamePasswordEnabledGet = () => true,
                IsMinimumPasswordLengthEnforcedGet = () => false,
                IsComplexPasswordEnforcedGet = () => true
            };

            CheckPasswordComplexity(null, interviewerPasswordSettings, false);
            CheckPasswordComplexity(string.Empty, interviewerPasswordSettings, false);
            CheckPasswordComplexity("             ", interviewerPasswordSettings, false);
            CheckPasswordComplexity(SimplePassword1, interviewerPasswordSettings, false);
            CheckPasswordComplexity(SimplePassword2, interviewerPasswordSettings, false);
            CheckPasswordComplexity(SimplePassword3, interviewerPasswordSettings, false);

            CheckPasswordComplexity(GoodPassword, interviewerPasswordSettings, true);
            CheckPasswordComplexity(EnoughComplexPassword, interviewerPasswordSettings, true);
            CheckPasswordComplexity(ShortEnoughComplexPassword, interviewerPasswordSettings, true);
        }

        private void CheckPasswordLengthIsHandledCorrectly(string password, IInterviewerPasswordSettings interviewerPasswordSettings, bool expectedResult)
        {
            try
            {
                _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, password, interviewerPasswordSettings);
                Assert.IsTrue(
                    expectedResult,
                    string.Format(
                        "Password [{0}] length is considered Ok, while minimal length is set to [{1}]",
                        password,
                        interviewerPasswordSettings.MinimumPasswordLength));
            }
            catch (TooShortPasswordException)
            {
                Assert.IsFalse(
                    expectedResult,
                    string.Format(
                        "Password [{0}] length is considered not Ok, while minimal length is set to [{1}]",
                        password,
                        interviewerPasswordSettings.MinimumPasswordLength));
            }
        }

        private void CheckPasswordComplexity(string password, IInterviewerPasswordSettings interviewerPasswordSettings, bool expectedResult)
        {
            try
            {
                _passwordRulesChecker.CheckNewPasswordSatisfiesRules(GoodPassword, password, interviewerPasswordSettings);
                Assert.IsTrue(
                    expectedResult,
                    string.Format("Password [{0}] is considered complex, while it is simple.", password));
            }
            catch (PasswordDoesNotSatisfyRulesException)
            {
                Assert.IsFalse(
                    expectedResult,
                    string.Format("Password [{0}] is considered simple, while it is complex.", password));
            }
        }
    }
}
