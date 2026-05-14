using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.Controls.Grid;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class SearchParametersProviderTest
    {
        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void GetLikeOrEqualOrEmptyOperator_ValueWithOnlyOpeningQuotes_ShouldReturnLikeOperator()
        {
            var provider = new SearchParametersProvider();
            string retVal;

            var result = provider.GetLikeOrEqualOrEmptyOperator("\"", out retVal);

            Assert.AreEqual(SearchOperator.Like, result);
            Assert.AreEqual("\"", retVal);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void GetLikeOrEqualOrEmptyOperator_ValueWithEmptyQuotes_ShouldReturnIsNullOrEmptyOperator()
        {
            var provider = new SearchParametersProvider();
            string retVal;

            var result = provider.GetLikeOrEqualOrEmptyOperator("\"\"", out retVal);

            Assert.AreEqual(SearchOperator.IsNullOrEmpty, result);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void GetLikeOrEqualOrEmptyOperator_NotEmptyValueInQuotes_ShouldReturnEqualOperator()
        {
            var provider = new SearchParametersProvider();
            string retVal;

            var result = provider.GetLikeOrEqualOrEmptyOperator("\"NotEmpty\"", out retVal);

            Assert.AreEqual(SearchOperator.Equal, result);
            Assert.AreEqual("NotEmpty", retVal);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void GetLikeOrEqualOrEmptyOperator_NotEmptyValueWithOpenQuotes_ShouldReturnLikeOperator()
        {
            var provider = new SearchParametersProvider();
            string retVal;

            var result = provider.GetLikeOrEqualOrEmptyOperator("\"NotEmpty", out retVal);

            Assert.AreEqual(SearchOperator.Like, result);
            Assert.AreEqual("\"NotEmpty", retVal);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void GetLikeOrEqualOrEmptyOperator_NotEmptyValueWithClosingQuotes_ShouldReturnLikeOperator()
        {
            var provider = new SearchParametersProvider();
            string retVal;

            var result = provider.GetLikeOrEqualOrEmptyOperator("NotEmpty\"", out retVal);

            Assert.AreEqual(SearchOperator.Like, result);
            Assert.AreEqual("NotEmpty\"", retVal);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void GetLikeOrEqualOrEmptyOperator_NotEmptyValueWithNoQuotes_ShouldReturnLikeOperator()
        {
            var provider = new SearchParametersProvider();
            string retVal;

            var result = provider.GetLikeOrEqualOrEmptyOperator("NotEmpty", out retVal);

            Assert.AreEqual(SearchOperator.Like, result);
            Assert.AreEqual("NotEmpty", retVal);
        }

    }
}
