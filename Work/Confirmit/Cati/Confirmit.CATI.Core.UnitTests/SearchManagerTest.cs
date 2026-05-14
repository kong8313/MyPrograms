using System;
using System.Linq;
using Confirmit.CATI.Core.Paging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests
{
    [TestClass]
    public class SearchManagerTest : BaseTest
    {
        private static readonly SearchParameterCollection _parameterCollection = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "TransientState",
                    ColumnType = SearchColumnType.Number,
                    Operator = SearchOperator.Equal,
                    Value = 39
                },
                new SearchParameter
                {
                    ColumnName = "Date",
                    ColumnType = SearchColumnType.DateTime,
                    Operator = SearchOperator.Equal,
                    Value = new DateTime(2026, 1, 1)
                },
                new SearchParameter
                {
                    ColumnName = "Float value",
                    ColumnType = SearchColumnType.Decimal,
                    Operator = SearchOperator.Equal,
                    Value = 5.0
                },
                new SearchParameter
                {
                    ColumnName = "Timestamp",
                    ColumnType = SearchColumnType.TimeSpan,
                    Operator = SearchOperator.Equal,
                    Value = new TimeSpan(1, 1, 1)
                },
                new SearchParameter
                {
                    ColumnName = "String",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Equal,
                    Value = "test"
                },
                new SearchParameter
                {
                    ColumnName = "Predefined data",
                    ColumnType = SearchColumnType.PredefinedDatePeriod,
                    Operator = SearchOperator.Equal,
                    Value = SearchPredefinedDate.ThisWeek
                }
            };

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void SerializeAndEncode_SearchParameterWithAllSupportedTypes_ResultDoesNotContainApostrophe()
        {
            var encodedString = SearchManager.SerializeAndEncode(_parameterCollection);

            Assert.IsFalse(encodedString.Contains('\''));
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void SerializeAndEncode_SearchParameterWithAllSupportedTypes_SuccessfullySerializedAndDeserialized()
        {
            var encodedString = SearchManager.SerializeAndEncode(_parameterCollection);

            var decodedResult = SearchManager.DeserializeWithDecode(encodedString);

            Assert.AreEqual(39, (int)decodedResult[0].Value);
            Assert.AreEqual(new DateTime(2026, 1, 1), (DateTime)decodedResult[1].Value);
            Assert.AreEqual(5.0, (double)decodedResult[2].Value);
            Assert.AreEqual(new TimeSpan(1, 1, 1), (TimeSpan)decodedResult[3].Value);
            Assert.AreEqual("test", (string)decodedResult[4].Value);
            Assert.AreEqual(SearchPredefinedDate.ThisWeek, (SearchPredefinedDate)decodedResult[5].Value);
        }
    }
}
