using System;
using System.Threading;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseTests
{
    [TestClass]
    public class TaskLockerTest 
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TryLock_TwoParalelLocksThreadAreSynchronized()
        {
            var test = new TestCati2(false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Manual, "user", "pwd", AgentTaskChoiceMode.Manual);
            test.Login("user", "pwd", AgentTaskChoiceMode.Manual, false);
            
            
            bool IsLockHolded = false;
            var thread = new Thread(() =>
            {
                using (TaskLocker taskLock2 = TaskLocker.TryLock(test.PersonSID))
                {
                    IsLockHolded = taskLock2 != null;
                }
            }); 

            using (TaskLocker taskLock = TaskLocker.TryLock(test.PersonSID))
            {
                Assert.IsNotNull(taskLock);

                
                thread.Start();
                var isFinished = thread.Join(TimeSpan.FromSeconds(5));
                Assert.IsFalse(isFinished);
            }

            Assert.IsTrue(thread.Join(TimeSpan.FromSeconds(5)));
            Assert.IsTrue(IsLockHolded);
        }
    }
}
