using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Security;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.PersonTests
{
    [TestClass]
    public class PersonAuthorizerTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        private int defaultMaxFailedLoginAttempts = 3;
        private string _personPassword;
        private string _legacyPasswordHash;
        private string _legacySalt;
        private int _personWithLegacyHashId;

        private IPersonRepository _personRepository;
        private IAccountLockingSettings _accountLockingSettings;
        private IInterviewerPasswordSettings _interviewerPasswordSettings;
        private IPersonAuthorizer _personAuthorizer;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();

            _personRepository = ServiceLocator.Resolve<IPersonRepository>();
            _accountLockingSettings = ServiceLocator.Resolve<IAccountLockingSettings>();
            _interviewerPasswordSettings = ServiceLocator.Resolve<IInterviewerPasswordSettings>();
            _personAuthorizer = ServiceLocator.Resolve<IPersonAuthorizer>();

            CreatePersonWithLegacyPasswordHash();
        }

        private void CreatePersonWithLegacyPasswordHash()
        {
            _personPassword = "1";
            _legacySalt = "1b4y6A==";

            _personWithLegacyHashId = _personRepository.Insert(new BvPersonEntity
            {
                Name = "user",
                ManualSelection = (int)AgentTaskChoiceMode.Manual,
                Description = "",
                FullName = "",
                CallCenterID = CallCenterTools.DefaultId
            });

            var hash = new PasswordHash();
            _legacyPasswordHash = hash.ComputeLegacyHash(_personWithLegacyHashId, _personPassword, _legacySalt);
            var person = PersonRepository.GetById(_personWithLegacyHashId);
            person.PwdHashTxt = _legacyPasswordHash;
            person.PwdSaltTxt = _legacySalt;
            PersonRepository.Update(person);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_NoPersonFound_False()
        {
            var personId = 1;
            var password = "password";

            var isPersonAuthorized = TryToLoginOnce(personId, password, true, defaultMaxFailedLoginAttempts);

            Assert.AreEqual(false, isPersonAuthorized);
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_LoginWithIncorrectPassword_FailedLoginAttemptsWasIncreased()
        {
            var password = "password";
            var wrongPassword = password + "1";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            TryToLoginOnce(personId, wrongPassword, true, defaultMaxFailedLoginAttempts);

            Assert.AreEqual(1, PersonService.GetFailedLoginAttempts(personId));
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_LoginWithIncorrectPassword_False()
        {
            var password = "password";
            var wrongPassword = password + "1";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            var isPersonAuthorized = TryToLoginOnce(personId, wrongPassword, true, defaultMaxFailedLoginAttempts);

            Assert.AreEqual(false, isPersonAuthorized);
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_LoginWithIncorrectPasswordMaxTimes_PersonLocked()
        {
            var password = "password";
            var wrongPassword = password + "1";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            TryToLoginSeveralTimes(defaultMaxFailedLoginAttempts, personId, wrongPassword, true, defaultMaxFailedLoginAttempts);

            Assert.IsTrue(PersonRepository.GetById(personId).IsLocked);
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_LoginWithCorrectPassword_True()
        {
            var password = "password";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            var isPersonAuthorized = TryToLoginOnce(personId, password, true, defaultMaxFailedLoginAttempts);

            Assert.AreEqual(true, isPersonAuthorized);
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_LoginWithIncorrectAndThenCorrectPassword_ReturnTrueAndFailedLoginAttemptsWasReset()
        {
            var password = "password";
            var wrongPassword = password + "1";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            var personFailedLoginAttempts = defaultMaxFailedLoginAttempts - 1;
            TryToLoginSeveralTimes(personFailedLoginAttempts, personId, wrongPassword, true, defaultMaxFailedLoginAttempts);

            var isPersonAuthorized = TryToLoginOnce(personId, password, true, defaultMaxFailedLoginAttempts);
            Assert.AreEqual(true, isPersonAuthorized);

            Assert.AreEqual(0, PersonService.GetFailedLoginAttempts(personId));
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_LockedPersonWithIncorrectPassword_ReturnFalseAndFailedLoginAttemptsIsMaxAndPersonLocked()
        {
            var password = "password";
            var wrongPassword = password + "1";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            TryToLoginSeveralTimes(defaultMaxFailedLoginAttempts, personId, wrongPassword, true, defaultMaxFailedLoginAttempts);

            // now person should be locked

            var isPersonAuthorized = TryToLoginOnce(personId, wrongPassword, true, defaultMaxFailedLoginAttempts);
            Assert.AreEqual(false, isPersonAuthorized);

            var person = PersonRepository.GetById(personId);
            Assert.AreEqual(defaultMaxFailedLoginAttempts, PersonService.GetFailedLoginAttempts(personId));
            Assert.IsTrue(person.IsLocked);
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_LockedPersonWithCorrectPassword_ReturnFalseAndFailedLoginAttemptsIsMaxAndPersonLocked()
        {
            var password = "password";
            var wrongPassword = password + "1";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            TryToLoginSeveralTimes(defaultMaxFailedLoginAttempts, personId, wrongPassword, true, defaultMaxFailedLoginAttempts);

            // now person should be locked

            var isPersonAuthorized = TryToLoginOnce(personId, password, true, defaultMaxFailedLoginAttempts);
            Assert.AreEqual(false, isPersonAuthorized);

            var person = PersonRepository.GetById(personId);
            Assert.AreEqual(defaultMaxFailedLoginAttempts, PersonService.GetFailedLoginAttempts(personId));
            Assert.IsTrue(person.IsLocked);
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_PersonWithSomeFailedLoginAttemptsAndDecreaseMaxFailedLoginAttemptsCorrectPassword_FalsePersonLocked()
        {
            var password = "password";
            var wrongPassword = password + "1";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            int someFailedLoginAttempts = defaultMaxFailedLoginAttempts - 1;
            TryToLoginSeveralTimes(someFailedLoginAttempts, personId, wrongPassword, true, defaultMaxFailedLoginAttempts);

            // decrease maxFaildeLoginAttempts
            // now FailedLoginAttempts for person is bigger than maxFaildeLoginAttempts

            var isPersonAuthorized = TryToLoginOnce(personId, password, true, 1);
            Assert.IsFalse(isPersonAuthorized);

            var person = PersonRepository.GetById(personId);
            Assert.AreEqual(someFailedLoginAttempts, PersonService.GetFailedLoginAttempts(personId));
            Assert.IsTrue(person.IsLocked);
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_PersonWithSomeFailedLoginAttemptsAndDecreaseMaxFailedLoginAttemptsInCorrectPassword_FalsePersonLocked()
        {
            var password = "password";
            var wrongPassword = password + "1";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            int someFailedLoginAttempts = defaultMaxFailedLoginAttempts - 1;
            TryToLoginSeveralTimes(someFailedLoginAttempts, personId, wrongPassword, true, defaultMaxFailedLoginAttempts);

            // decrease maxFaildeLoginAttempts
            // now FailedLoginAttempts for person is bigger than maxFaildeLoginAttempts

            var isPersonAuthorized = TryToLoginOnce(personId, wrongPassword, true, 1);
            Assert.IsFalse(isPersonAuthorized);

            var person = PersonRepository.GetById(personId);
            Assert.AreEqual(someFailedLoginAttempts, PersonService.GetFailedLoginAttempts(personId));
            Assert.IsTrue(person.IsLocked);
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_LockedPersonAndIncreaseMaxFailedLoginAttemptsCorrectPassword_FalsePersonLocked()
        {
            var password = "password";
            var wrongPassword = password + "1";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            TryToLoginSeveralTimes(defaultMaxFailedLoginAttempts, personId, wrongPassword, true, defaultMaxFailedLoginAttempts);

            // now person is locked
            // increase maxFaildeLoginAttempts

            var isPersonAuthorized = TryToLoginOnce(personId, wrongPassword, true, defaultMaxFailedLoginAttempts + 1);
            Assert.IsFalse(isPersonAuthorized);

            var person = PersonRepository.GetById(personId);
            Assert.AreEqual(defaultMaxFailedLoginAttempts, PersonService.GetFailedLoginAttempts(personId));
            Assert.IsTrue(person.IsLocked);
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_LockedPersonLockingDisabled_False()
        {
            var password = "password";
            var wrongPassword = password + "1";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            TryToLoginSeveralTimes(defaultMaxFailedLoginAttempts, personId, wrongPassword, true, defaultMaxFailedLoginAttempts);

            //now person is locked

            var isPersonAuthorized = TryToLoginOnce(personId, password, false, defaultMaxFailedLoginAttempts);
            Assert.AreEqual(false, isPersonAuthorized);

            var person = PersonRepository.GetById(personId);
            Assert.AreEqual(defaultMaxFailedLoginAttempts, PersonService.GetFailedLoginAttempts(personId));
            Assert.IsTrue(person.IsLocked);
        }

        [TestMethod, Owner("SvetlanaT")]
        public void Authorize_LoginWithIncorrectPasswordLockingDisabled_FalseButFailedLoginAttemptsNotChanged()
        {
            var password = "password";
            var wrongPassword = password + "1";

            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);

            var isPersonAuthorized = TryToLoginOnce(personId, wrongPassword, false, defaultMaxFailedLoginAttempts);
            Assert.AreEqual(false, isPersonAuthorized);

            Assert.AreEqual(0, PersonService.GetFailedLoginAttempts(personId));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Authorise_LoginWithValidPasswordForUserWithLegacyPasswordHash_SuccessfulAuthentication()
        {
            var result = TryToLoginOnce(_personWithLegacyHashId, _personPassword, false, 0);

            Assert.IsTrue(result, "Person should be authorised");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Authorise_LoginWithIncorrectPasswordForUserWithLegacyPasswordHash_AuthenticationFails()
        {
            var result = TryToLoginOnce(_personWithLegacyHashId, _personPassword + "1", false, 0);

            Assert.IsFalse(result, "Person should not be authorised");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Authorise_LoginWithIncorrectPasswordForUserWithLegacyPasswordHash_PasswordHashIsNotChanged()
        {
            TryToLoginOnce(_personWithLegacyHashId, _personPassword + "1", false, 0);

            var person = PersonRepository.GetById(_personWithLegacyHashId);

            Assert.AreEqual(_legacyPasswordHash, person.PwdHashTxt, "Password should be {0} but it is {1}", _legacyPasswordHash, person.PwdHashTxt);
            Assert.AreEqual(_legacySalt, person.PwdSaltTxt, "Salt should be {0} but it is {1}", _legacySalt, person.PwdSaltTxt);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Authorise_LoginWithValidPasswordForUserWithLegacyPasswordHash_PasswordHashShouldBeUpdated()
        {
            TryToLoginOnce(_personWithLegacyHashId, _personPassword, false, 0);

            var person = PersonRepository.GetById(_personWithLegacyHashId);
            var hash = ServiceLocator.Resolve<IPasswordHash>();
            Assert.IsFalse(hash.IsLegacyHash(person.PwdHashTxt), "Password wasn't updated correctly. Old {0}, new {1}", _legacyPasswordHash, person.PwdHashTxt);
            Assert.IsFalse(hash.IsLegacyHash(person.PwdSaltTxt), "Salt value wasn't updated correctly. Old {0}, new {1}", _legacySalt, person.PwdSaltTxt);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void Authorise_PasswordExpiredButPasswordExpirationDisabled_Success()
        {
            var password = "password";
            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);
            var person = PersonRepository.GetById(personId);
            person.PwdSetDate = DateTime.UtcNow.AddDays(-5);
            PersonRepository.Update(person);

            Assert.IsTrue(
                TryToLoginWithCheckExpiration(person, "password", false, 0, false),
                "Authorization failed while it was expected to succeed.");
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void Authorise_PasswordExpiredAndPasswordExpirationEnabled_Fail()
        {
            var password = "password";
            var personId = PersonTools.CreatePerson("person", password, AgentTaskChoiceMode.Automatic);
            var person = PersonRepository.GetById(personId);
            person.PwdSetDate = DateTime.UtcNow.AddDays(-5);
            PersonRepository.Update(person);

            Assert.IsFalse(
                TryToLoginWithCheckExpiration(person, "password", false, 0, true),
                "Authorization succeeded while it was expected to fail.");
        }

        private bool TryToLoginWithCheckExpiration(
            BvPersonEntity person,
            string password,
            bool accountLockingEnabled,
            int maxFailedLoginAttempts,
            bool isPasswordExpirationEnabled)
        {
            _accountLockingSettings.Enabled = accountLockingEnabled;
            _accountLockingSettings.MaxFailedLoginAttempts = maxFailedLoginAttempts;
            _interviewerPasswordSettings.IsExpirationEnabled = isPasswordExpirationEnabled;
            _interviewerPasswordSettings.ExpirationPeriodInDays = 1;

            return (_personAuthorizer.Authorize(person, password) &&
                !_personAuthorizer.IsPasswordExpired(person, _interviewerPasswordSettings));
        }


        private bool TryToLoginOnce(
            int personId,
            string password,
            bool accountLockingEnabled,
            int maxFailedLoginAttempts)
        {
            _accountLockingSettings.Enabled = accountLockingEnabled;
            _accountLockingSettings.MaxFailedLoginAttempts = maxFailedLoginAttempts;

            var person = PersonRepository.TryGetById(personId);
            return _personAuthorizer.Authorize(person, password);
        }

        private void TryToLoginSeveralTimes(int loginAttemptsCount, int personId, string password, bool accountLockingEnabled, int maxFailedLoginAttempts)
        {
            _accountLockingSettings.Enabled = accountLockingEnabled;
            _accountLockingSettings.MaxFailedLoginAttempts = maxFailedLoginAttempts;

            var person = PersonRepository.GetById(personId);
            for (int i = 0; i < loginAttemptsCount; i++)
            {
                _personAuthorizer.Authorize(person, password);
            }
        }
    }
}
