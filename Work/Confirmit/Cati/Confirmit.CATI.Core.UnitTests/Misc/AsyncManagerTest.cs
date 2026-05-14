using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Misc
{
    [TestClass]
    public class AsyncManagerTest
    {
        private AsyncManager asyncManager = new AsyncManager();

        [TestMethod]
        public void ScheduleTask_UseShortDelay_TaskWasExecuted()
        {
            ManualResetEvent taskCompleteEvent = new ManualResetEvent(false);

            var task = asyncManager.CreateTask(() => taskCompleteEvent.Set());

            asyncManager.ScheduleTask(TimeSpan.FromMilliseconds(50), task);

            Assert.IsTrue(taskCompleteEvent.WaitOne(100));
        }

        [TestMethod]
        public void ScheduleTask_UseLongDelay_TaskWasnotExecuted()
        {
            ManualResetEvent taskCompleteEvent = new ManualResetEvent(false);

            var task = asyncManager.CreateTask(() => taskCompleteEvent.Set());

            asyncManager.ScheduleTask(TimeSpan.FromMilliseconds(100), task);

            Assert.IsFalse(taskCompleteEvent.WaitOne(50));
        }

    }
}
