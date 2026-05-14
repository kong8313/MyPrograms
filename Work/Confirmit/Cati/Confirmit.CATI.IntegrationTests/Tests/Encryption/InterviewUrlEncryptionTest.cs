using System;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Encryption;
using Confirmit.CATI.Common.Encryption.Fakes;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Encryption
{
    [TestClass]
    public class InterviewUrlEncryptionTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private const string UserName = "user";
        private const string Password = "password";

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);

            ServiceLocator.Register<ICatiSymmetricEncryptor, CatiSymmetricEncryptor>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void LoginToConsole_EncryptionKeyAndIVAreReturned_Success()
        {
            var test = new TestCati2(false, false, _backendTools);

            PersonInfo personInfo;
            test.CreatePerson(UserName, Password, AgentTaskChoiceMode.Manual);
            test.Login(UserName, Password, AgentTaskChoiceMode.Manual, false, out personInfo);

            var task = TaskRepository.GetByPerson(test.PersonSID);

            CollectionAssert.AreEqual(task.EncryptionKey, personInfo.EncryptionKey, "Encryption keys differ");
            CollectionAssert.AreEqual(task.EncryptionIV, personInfo.EncryptionIV, "Encryption initial vectors differ");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void StartingInterview_InterviewUrlIsReturned_InterviewUrlIsEncrypted()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);

            Assert.IsTrue(test.WS.StartInterview(string.Empty, 0));

            var state = test.WaitInterviewState(InterviewState.INTERVIEWING);

            Assert.IsFalse(Uri.IsWellFormedUriString(state.interviewURL, UriKind.Absolute));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void StartingInterview_InterviewUrlIsReturned_InterviewUrlIsEncryptedSuccessfully()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            PersonInfo personInfo;
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true, out personInfo);

            Assert.IsTrue(test.WS.StartInterview(string.Empty, 0));

            string interviewUrl = string.Empty;

            var originalEncryptor = ServiceLocator.Resolve<ICatiSymmetricEncryptor>();
            var stubICatiSymmetricEncryptor = new StubICatiSymmetricEncryptor
            {
                Inner = originalEncryptor,
                EncryptStringString = text =>
                {
                    interviewUrl = text;

                    return originalEncryptor.EncryptString(text);
                }
            };
            ServiceLocator.RegisterInstance<ICatiSymmetricEncryptor>(stubICatiSymmetricEncryptor);

            var state = test.WaitInterviewState(InterviewState.INTERVIEWING);

            var decryptor = ServiceLocator.Resolve<ICatiSymmetricEncryptor>();
            decryptor.Key = personInfo.EncryptionKey;
            decryptor.IV = personInfo.EncryptionIV;
            var decryptedUrl = decryptor.DecryptString(state.interviewURL);

            Assert.AreEqual(interviewUrl, decryptedUrl, "Interview urls differ");
        }
    }
}
