using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceCounter = Confirmit.CATI.Common.PerformanceCounters.PerformanceCounter;

namespace Confirmit.CATI.Common.UnitTests
{
    [TestClass]
    public class PerformanceCounterTest
    {
        
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Initialize_CaunterIsnotRegistred_CountMethodsIsnotThrowException()
        {
            var counter = new PerformanceCounter("TestCounter", "help string from TestCounter",
                PerformanceCounterType.NumberOfItems32);

            counter.Initialize("NotExistsedCategory");

            counter.Increment();
            counter.Decrement();
            counter.IncrementBy(TimeSpan.FromSeconds(10));
            counter.IncrementBy(10);
            counter.Decrement();
        }
    }
}
