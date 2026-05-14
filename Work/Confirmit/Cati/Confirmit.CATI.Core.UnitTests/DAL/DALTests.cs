using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.UnitTests.DAL
{
    [TestClass]
    public class ShiftServiceTests
    {
        #region Initialize and Cleanup methods

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        public TestContext TestContext { get; set; }

        #endregion

        [TestMethod, Owner( @"FIRM\AlexeyN")]
        public void DALGeneratedTableEntity_SetNullValueToNullableString_Success()
        {
            var entity = new BvDialersEntity();
            entity.ConfigurationParameters = null;
        }

        [TestMethod, Owner( @"FIRM\AlexeyN" ), ExpectedException( typeof( ArgumentNullException ) )]
        public void DALGeneratedTableEntity_SetNullValueToNotNullableString_ExceptionThrown()
        {
            var entity = new BvDialersEntity();
            entity.Name = null;
        }
    }
}