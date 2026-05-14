using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using  FilterSvc=Confirmit.CATI.Core.Services.FilterServiceImplementation;

namespace Confirmit.CATI.Backend.UnitTests.FilterService
{
    [TestClass]
    public class GetWhereClauseForReplicatedTableTest
    {
        [TestMethod]
        public void TwoAndedEqualityConditions()
        {
            var filters = new Dictionary<string, int>  {{ "q1", 1}, {"q2", 2}};

            var whereClause = FilterSvc.FilterService.GetWhereClauseForReplTable(filters);

            Assert.AreEqual("(CFinterview.[q1]=1) AND (CFinterview.[q2]=2)", whereClause);
        }
    }
}
