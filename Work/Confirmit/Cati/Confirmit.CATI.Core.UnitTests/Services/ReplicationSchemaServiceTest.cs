using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Services
{
    [TestClass]
    public class ReplicationSchemaServiceTest
    {
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void IsColumnAvailableForSqlIndexing_ColumnWithNvarCharMaxType_ResultIsFalse()
        {
            var column = new ReplicationColumnInfo()
            {
                DataType = SqlDataType.NVarCharMax
            };
            var expected = ReplicationSchemaService.IsColumnAvailableForSqlIndexing(column);

            Assert.IsFalse(expected);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void IsColumnAvailableForSqlIndexing_ColumnWithIntType_ResultIsTrue()
        {
            var column = new ReplicationColumnInfo()
            {
                DataType = SqlDataType.Int
            };
            var expected = ReplicationSchemaService.IsColumnAvailableForSqlIndexing(column);

            Assert.IsTrue(expected);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void IsColumnAvailableForSqlIndexing_ColumnWithNChar100Type_ResultIsTrue()
        {
            var column = new ReplicationColumnInfo()
            {
                DataType = SqlDataType.NChar,
                MaxLength = 100
            };
            var expected = ReplicationSchemaService.IsColumnAvailableForSqlIndexing(column);

            Assert.IsTrue(expected);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void IsColumnAvailableForSqlIndexing_ColumnWithNChar300Type_ResultIsFalse()
        {
            var column = new ReplicationColumnInfo()
            {
                DataType = SqlDataType.NChar,
                MaxLength = 300
            };
            var expected = ReplicationSchemaService.IsColumnAvailableForSqlIndexing(column);

            Assert.IsTrue(expected);
        }
    }
}
