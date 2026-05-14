using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Backend.WebApiServices.Controllers;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Backend.WebApiServices.Services;
using Confirmit.CATI.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Backend.UnitTests.WebApiServices
{
    [TestClass]
    public class BlackListHelperTests
    {
        [TestMethod]
        public void BlackListHelper_IsTelephoneBlackListItemValid_CorrectBlacklistItem()
        {
            var item = new TelephoneBlacklistItem
            {
                TelephoneNumber = "12345",
                Type = BlacklistPatternType.StartWith
            };

            Assert.IsTrue(BlackListHelper.IsTelephoneBlackListItemValid(item));
        }

        [TestMethod]
        public void BlackListHelper_IsTelephoneBlackListItemValid_WrongTelephoneNumber()
        {
            var item = new TelephoneBlacklistItem
            {
                TelephoneNumber = "12345*",
                Type = BlacklistPatternType.StartWith
            };

            Assert.IsFalse(BlackListHelper.IsTelephoneBlackListItemValid(item));
        }

        [TestMethod]
        public void BlackListHelper_IsTelephoneBlackListItemValid_WrongBlackListPatternType()
        {
            var item = new TelephoneBlacklistItem
            {
                TelephoneNumber = "123",
                Type = (BlacklistPatternType)9
            };

            Assert.IsFalse(BlackListHelper.IsTelephoneBlackListItemValid(item));
        }

        [TestMethod]
        public void BlackListHelper_IsTelephoneBlackListItemValid_WrongTelephoneNumberAndBlackListPatternType()
        {
            var item = new TelephoneBlacklistItem
            {
                TelephoneNumber = "12345a",
                Type = (BlacklistPatternType)9
            };

            Assert.IsFalse(BlackListHelper.IsTelephoneBlackListItemValid(item));
        }
    }
}
