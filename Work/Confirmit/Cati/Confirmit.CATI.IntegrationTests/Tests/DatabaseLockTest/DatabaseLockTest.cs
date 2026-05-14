using System;
using System.Threading;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Tests.MultiUserEnvironment.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation.Fakes;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseLockTest
{
    [TestClass]
    public class DatabaseLockTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        private const string ResourceName = "ResourceName";
        private const string OwnerName = "DatabaseLockTest";
        private const int DefaultLockTimeout = 4000;
        private const int LockPeriod = 4000;
        private const int ThreadWorkTime = 2000;

        private static Counter _counter;
        private static Counter _timeoutCounter;
        private ManualResetEvent _getLockEvent;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _counter = new Counter(0);
            _getLockEvent = new ManualResetEvent(true);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        static void GeneralLockThreadMethod()
        {
            try
            {
                using (var dbLock = ExclusiveDatabaseLock.CreateLock(
                    ResourceName,
                    OwnerName,
                    DefaultLockTimeout))
                {
                    if (dbLock.TryEnterLock())
                    {
                        _counter += 1;

                        Thread.Sleep(ThreadWorkTime);
                        // simulate that thread is doing something for some time and holds the lock
                    }
                }
            }
            catch (Exception e)
            {
                TraceHelper.TraceException(e);
            }
        }

        private void RunThreadsInOrder(Thread thread1, Thread thread2, params object[] threadsParameters)
        {
            var processAndEnvironmentInfo = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>();
            var original = ServiceLocator.Resolve<IDatabaseAppLockService>();
            var stub = new StubIDatabaseAppLockService
            {
                Inner = original,
                GetExclusiveLockStringStringInt32Int32StringInt32 =
                    (resourceName, lockMode, lockTimeout, waitPeriod, resourceOwner, commandExecutionTimeout)
                        =>
                    {
                        int result;

                        BvSpGetAppLockAdapter.ExecuteNonQuery(
                            resourceName,
                            lockMode,
                            lockTimeout,
                            processAndEnvironmentInfo.MachineName,
                            waitPeriod,
                            resourceOwner,
                            commandExecutionTimeout,
                            out result);

                        _getLockEvent.Set();

                        return result;
                    }
            };
            ServiceLocator.RegisterInstance<IDatabaseAppLockService>(stub);

            _getLockEvent.Reset();

            if (threadsParameters.Length > 0)
            {
                if (threadsParameters[0] != null)
                    thread1.Start(threadsParameters[0]);
            }
            else
            {
                thread1.Start();
            }

            _getLockEvent.WaitOne();

            if (threadsParameters.Length > 1)
            {
                if (threadsParameters[1] != null)
                    thread2.Start(threadsParameters[1]);
            }
            else
            {
                thread2.Start();
            }
        }

        static void PeriodicalLockThreadMethod()
        {
            try
            {
                using (var dbLock = ExclusiveDatabaseLock.CreatePeriodicalLock(
                    ResourceName,
                    OwnerName,
                    LockPeriod))
                {
                    if (dbLock.TryEnterLock())
                    {
                        _counter += 1;

                        Thread.Sleep(ThreadWorkTime);
                        // simulate that thread is doing something for some time and holds the lock
                    }
                }
            }
            catch (Exception e)
            {
                TraceHelper.TraceException(e);
            }
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DatabaseLock_TwoGeneralLocks_GrantedSuccessfully()
        {
            var thread1 = new Thread(GeneralLockThreadMethod);
            var thread2 = new Thread(GeneralLockThreadMethod);

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.AreEqual(2, _counter);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DatabaseLock_TwoPeriodicalLocks_SecondIsNotGranted()
        {
            var thread1 = new Thread(PeriodicalLockThreadMethod);
            var thread2 = new Thread(PeriodicalLockThreadMethod);

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.AreEqual(1, _counter);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DatabaseLock_ThreePeriodicalLocks_LockIsNotGrantedWhileLockPeriodAndGrantedAfterIt()
        {
            var thread1 = new Thread(PeriodicalLockThreadMethod);
            var thread2 = new Thread(PeriodicalLockThreadMethod);
            var thread3 = new Thread(PeriodicalLockThreadMethod);

            thread1.Start();
            Thread.Sleep(ThreadWorkTime + LockPeriod / 2); // time passed after lock free < than LockPeriod
            thread2.Start();

            thread1.Join();
            thread2.Join();
            Assert.AreEqual(1, _counter);

            Thread.Sleep(LockPeriod / 2 + 1000); // time passed after lock free > than SleepTimeout

            thread3.Start();
            thread3.Join();

            Assert.AreEqual(2, _counter);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DatabaseLock_GetLockWithFailedReleaseLock_SecondLockSucceed()
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("failed transaction"))
                {
                    using (var dbLock = ExclusiveDatabaseLock.CreatePeriodicalLock(
                        ResourceName, OwnerName, LockPeriod))
                    {
                        Assert.IsTrue(dbLock.TryEnterLock());

                        //generate raiserror
                        BvSpScheduleParam_SetAdapter.ExecuteNonQuery(0, 0, 0);
                    }
                    transaction.Commit();
                }
            }
            catch (Exception)
            {
            }

            using (var dbLock = ExclusiveDatabaseLock.CreatePeriodicalLock(
                ResourceName,
                OwnerName,
                LockPeriod))
            {
                Assert.IsTrue(dbLock.TryEnterLock());
            }
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DatabaseLock_PeriodicalLockAndGeneralLock_GrantedSuccessfully()
        {
            var thread1 = new Thread(PeriodicalLockThreadMethod);
            var thread2 = new Thread(GeneralLockThreadMethod);

            RunThreadsInOrder(thread1, thread2);

            thread1.Join();
            thread2.Join();

            Assert.AreEqual(2, _counter);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DatabaseLock_PeriodicalLockAndGeneralLockInRunPeriod_GrantedSuccessfully()
        {
            var thread1 = new Thread(PeriodicalLockThreadMethod);
            var thread2 = new Thread(GeneralLockThreadMethod);

            thread1.Start();
            Thread.Sleep(ThreadWorkTime + LockPeriod / 2); // < than SleepTimeout
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.AreEqual(2, _counter);
        }

        static void GeneralLockWithExceptionThreadMethod(object exceptionMessage)
        {
            using (var dbLock = ExclusiveDatabaseLock.CreateLock(
                ResourceName,
                OwnerName,
                DefaultLockTimeout))
            {
                bool result = false;

                try
                {
                    result = dbLock.TryEnterLock();
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex.Message.Contains(Convert.ToString(exceptionMessage)), "Unexpected exception occured while TryEnterLock: " + ex.Message);
                }

                if (result)
                {
                    _counter += 1;
                }
            }
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DatabaseLock_TwoGeneralLocksAndExceptionInOneGetLock_NextLockSuccessfullyGranted()
        {
            const string exceptionMessage = "Exception occured while TryEnterLock";

            int callNumber = 0;
            Task task1 = new Task(() => GeneralLockWithExceptionThreadMethod("IsLockHeld = True"));
            Task task2 = new Task(GeneralLockThreadMethod);

            var processAndEnvironmentInfo = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>();
            var original = ServiceLocator.Resolve<IDatabaseAppLockService>();
            var stub = new StubIDatabaseAppLockService
            {
                Inner = original,
                GetExclusiveLockStringStringInt32Int32StringInt32 =
                    (resourceName, lockMode, lockTimeout, waitPeriod, resourceOwner, commandExecutionTimeout)
                        =>
                    {
                        ++callNumber;

                        if (callNumber == 2)
                        {
                            return original.GetExclusiveLock(
                                resourceName,
                                lockMode,
                                lockTimeout,
                                waitPeriod,
                                resourceOwner,
                                commandExecutionTimeout);
                        }

                        int result;

                        BvSpGetAppLockAdapter.ExecuteNonQuery(
                            resourceName,
                            lockMode,
                            lockTimeout,
                            processAndEnvironmentInfo.MachineName,
                            waitPeriod,
                            resourceOwner,
                            commandExecutionTimeout,
                            out result);

                        task2.Start();
                        throw new Exception(exceptionMessage);
                    }
            };
            ServiceLocator.RegisterInstance<IDatabaseAppLockService>(stub);

            task1.Start();

            Task.WaitAll(task1, task2);

            Assert.AreEqual(1, _counter);
        }

        static void GeneralLockInTransactionThreadMethod()
        {
            using (new DatabaseTransactionScope("LockTransaction"))
            {
                using (var dbLock = ExclusiveDatabaseLock.CreateLock(
                    ResourceName,
                    OwnerName,
                    DefaultLockTimeout))
                {
                    if (dbLock.TryEnterLock())
                    {
                        _counter += 1;
                        Thread.Sleep(ThreadWorkTime); // simulate that thread is doing something for some time and holds the lock
                    }
                }
            }
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DatabaseLock_TwoGeneralLocksInTransactions_BothGrantedSuccessfully()
        {
            var thread1 = new Thread(GeneralLockInTransactionThreadMethod);
            var thread2 = new Thread(GeneralLockInTransactionThreadMethod);

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.AreEqual(2, _counter);
        }

        static void GeneralLockInConnection()
        {
            ExclusiveDatabaseLock dbLock;
            using (new ConnectionScope())
            {
                dbLock = ExclusiveDatabaseLock.CreateLock(
                    ResourceName,
                    OwnerName,
                    DefaultLockTimeout);

                if (dbLock.TryEnterLock())
                {
                    _counter += 1;
                }

                // We do not call dbLock.Dispose();
            }

            try
            {
                dbLock.Dispose();
            }
            catch (Exception)
            {
                Assert.Fail("No exceptions should be thrown from Dispose method");
            }
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DatabaseLock_TwoGeneralLocksInConnectionDoNotCallLockDispose_GrantedSuccessfullyAfterConnectionDispose()
        {
            var thread1 = new Thread(GeneralLockInConnection);
            var thread2 = new Thread(GeneralLockThreadMethod);

            RunThreadsInOrder(thread1, thread2);

            thread1.Join();
            thread2.Join();

            Assert.AreEqual(2, _counter);
        }

        static void GeneralLockWithTimeoutThreadMethod(object timeOut)
        {
            using (var dbLock = ExclusiveDatabaseLock.CreateLock(
                ResourceName,
                OwnerName,
                (int)timeOut))
            {
                bool result = false;
                try
                {
                    result = dbLock.TryEnterLock();
                }
                catch (Exception ex) // This is general exection after work item CATI-1291
                {
                    if (!ex.Message.Contains("IsLockHeld"))
                    {
                        Assert.Fail("Unexpected exception occured while TryEnterLock");
                    }
                    else
                    {
                        _timeoutCounter += 1;
                    }
                }

                if (result)
                {
                    _counter += 1;

                    Thread.Sleep(ThreadWorkTime); // simulate that thread is doing something for some time and holds the lock
                }
            }
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DatabaseLock_TwoGeneralLocksOneTimedOut_NextLockGrantedSuccessfully()
        {
            _timeoutCounter = new Counter(0);
            var thread1 = new Thread(GeneralLockWithTimeoutThreadMethod);
            var thread2 = new Thread(GeneralLockWithTimeoutThreadMethod);
            var thread3 = new Thread(GeneralLockWithTimeoutThreadMethod);

            RunThreadsInOrder(thread1, thread2, DefaultLockTimeout, 1000);

            thread1.Join();
            thread2.Join();

            Assert.AreEqual(1, _counter, "Failer for m_Counter");
            Assert.AreEqual(1, _timeoutCounter, "Failed for m_TimeoutCounter");

            thread3.Start(0); // 0 seconds lock timeout

            thread3.Join();

            Assert.AreEqual(2, _counter, "Failer for second check m_Counter");
            Assert.AreEqual(1, _timeoutCounter, "Failer for second check m_TimeoutCounter");
        }

        private void DatabaseLock_2ExclusiveLockInTransaction_TheyNotLocksEachOther(string resourceName1, string resourceName2)
        {
            bool isExceptionThrown = false;

            var evt = new ManualResetEvent(false);

            Action<string> action = (resourceName) =>
            {
                try
                {
                    using (new DatabaseTransactionScope("Tran"))
                    {
                        using (
                            var dbLock = ExclusiveDatabaseLock.CreateLock(
                                resourceName, OwnerName, DefaultLockTimeout))
                        {
                            if (dbLock.TryEnterLock())
                            {
                                _counter += 1;

                                evt.WaitOne();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    isExceptionThrown = true;
                }
            };

            var thread1 = new Thread(() => action(resourceName1));
            var thread2 = new Thread(() => action(resourceName2));
            thread1.Start();
            thread2.Start();

            Thread.Sleep(1000);
            int counter = _counter;

            evt.Set();

            thread1.Join();
            thread2.Join();

            Assert.IsFalse(isExceptionThrown, "Thread should not throw exceptions");
            Assert.AreEqual(2, counter);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void DatabaseLock_2FirstExclusiveLockInTransaction_TheyNotLocksEachOther()
        {
            DatabaseLock_2ExclusiveLockInTransaction_TheyNotLocksEachOther("1", "2");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void DatabaseLock_FirstAndSecondExclusiveLockInTransaction_TheyNotLocksEachOther()
        {
            using (
                var dbLock = ExclusiveDatabaseLock.CreateLock(
                    "1", OwnerName, DefaultLockTimeout))
            {
                dbLock.TryEnterLock();
            }

            DatabaseLock_2ExclusiveLockInTransaction_TheyNotLocksEachOther("1", "2");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void DatabaseLock_2SecondExclusiveLockInTransaction_TheyNotLocksEachOther()
        {
            using (
                var dbLock = ExclusiveDatabaseLock.CreateLock(
                    "1", OwnerName, DefaultLockTimeout))
            {
                dbLock.TryEnterLock();
            }

            using (
                var dbLock = ExclusiveDatabaseLock.CreateLock(
                    "2", OwnerName, DefaultLockTimeout))
            {
                dbLock.TryEnterLock();
            }

            DatabaseLock_2ExclusiveLockInTransaction_TheyNotLocksEachOther("1", "2");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void DatabaseLock_NestedExclusiveLockInTransaction_TheyNotLocksEachOther()
        {
            bool testSuccessed = false;

            using (
                var dbLock1 = ExclusiveDatabaseLock.CreateLock(
                    "1", OwnerName, 10))
            {
                if (dbLock1.TryEnterLock())
                {
                    using (
                        var dbLock2 = ExclusiveDatabaseLock.CreateLock(
                            "1", OwnerName, 10))
                    {
                        if (dbLock2.TryEnterLock())
                        {
                            testSuccessed = true;
                        }
                    }
                }
            }

            Assert.IsTrue(testSuccessed);
        }
    }
}
