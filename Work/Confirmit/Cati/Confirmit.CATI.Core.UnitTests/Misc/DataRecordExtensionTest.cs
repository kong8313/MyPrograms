using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Misc.Extensions;
using Microsoft.SqlServer.Server;

namespace Confirmit.CATI.Core.UnitTests.Misc
{
    [TestClass]
    public class DataRecordExtensionTest
    {
        [TestMethod, Owner(@"FIRM\OlegZ")]
        public void GetValueOrDefault_BooleanValue_ReturnIntegerValue()
        {
            const string boolCol = "boolCol";
            var record = new SqlDataRecord(new[] { new SqlMetaData(boolCol, SqlDbType.Bit) });
            record.SetBoolean(0, true);

            var target = record.GetValueOrDefault(boolCol, 0);
            Assert.AreEqual(target.GetType(), typeof(int));
            Assert.AreEqual(target, 1);
        }
    }
}
