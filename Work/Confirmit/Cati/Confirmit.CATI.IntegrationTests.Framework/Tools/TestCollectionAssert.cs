using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class TestCollectionAssert
    {
        class Pair<T>
        {
            public bool AlreadyReviewed = false;
            public T Value;
        }

        public static void AreEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual, Func<T, T, bool> comparer)
        {
            Assert.AreEqual(expected.Count(), actual.Count(), "Collection size differs: expected {0}, actual {1}", expected.Count(), actual.Count());

            var actualWithFlag = actual.Select(item => new Pair<T> { Value = item }).ToList();

            for (int i = 0; i < expected.Count(); i++)
            {
                var exp = expected.ElementAt(i);
                var actualPair = actualWithFlag.Find(pair => pair.AlreadyReviewed == false && comparer(exp, pair.Value));
                Assert.IsNotNull(actualPair, "Element with index {0} from expected collection is not found in actual collection", i);
                actualPair.AlreadyReviewed = true;
            }
        }
    }
}
