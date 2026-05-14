using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Classes.Filters;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class FilterFieldsFactoryTest
    {
        class VaidatorWithException : IFilterFieldValidator
        {
            public void Validate(BvFilterFieldsEntity field)
            {
                throw new UserMessageException();
            }
        }

        class EmptyValidator : IFilterFieldValidator
        {
            public void Validate(BvFilterFieldsEntity field) { }
        }

        class EmptyTimezoneConverter : ITimezoneConverter
        {
            public DateTime ConvertToUtc(int tzId, DateTime localTime)
            {
                return localTime;
            }
        }

        class EmptyCachedLocalTimezoneManager : ICachedLocalTimezoneManager
        {
            public int GetLocalTimezoneId()
            {
                throw new NotImplementedException();
            }

            public BvTimezoneEntity GetLocalTimezone()
            {
                throw new NotImplementedException();
            }

            public void ChangeLocal(int timezoneId)
            {
                throw new NotImplementedException();
            }

            public DateTime GetCurrentLocalTime()
            {
                throw new NotImplementedException();
            }

            public DateTime ConvertToLocalTime(DateTime utc)
            {
                throw new NotImplementedException();
            }

            public DateTime ConvertToUtc(DateTime localTime)
            {
                return localTime;
            }
        }

        private FilterFieldsFactory _factoryWithException;
        private FilterFieldsFactory _factory;
        private string _validFieldsXml;

        private BvFilterFieldsEntity _expectedFirstField;
        private BvFilterFieldsEntity _expectedSecondField;

        [TestInitialize]
        public void TestInitialize()
        {
            _factoryWithException = new FilterFieldsFactory(new VaidatorWithException(), new EmptyCachedLocalTimezoneManager());
            _factory = new FilterFieldsFactory(new EmptyValidator(), new EmptyCachedLocalTimezoneManager());
            _validFieldsXml =
                "<vars><var><TableType>2</TableType><Column>CallState</Column><VarType>5</VarType><Sign>3</Sign><Value>2</Value><Disable>0</Disable><IsBackground>False</IsBackground></var><var><TableType>1</TableType><Column>ID</Column><VarType>1</VarType><Sign>2</Sign><Value>10</Value><Disable>0</Disable><IsBackground>false</IsBackground></var></vars>";
            _expectedFirstField = new BvFilterFieldsEntity
                              {
                                  Table = (int) TableTypes.Call,
                                  Column = "CallState",
                                  Type = (int) VariableTypes.PredefinedValue,
                                  Sign = (int) FilterOperator.Equal,
                                  Value = "2",
                                  IsNeedCast = false
                              };
            _expectedSecondField = new BvFilterFieldsEntity
                              {
                                  Table = (int) TableTypes.Interview,
                                  Column = "ID",
                                  Type = (int) VariableTypes.Integer,
                                  Sign = (int) FilterOperator.Bigger,
                                  Value = "10",
                                  IsNeedCast = false
                              };
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(UserMessageException))]
        public void Create_FieldValidationFailsWithException_ExceptionIsPushedUpOutsideFactory()
        {
            _factoryWithException.Create(_validFieldsXml);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Create_ValidFieldsXml_Success()
        {
            var result = _factory.Create(_validFieldsXml);

            Assert.AreEqual(2, result.Count(), "Wrong number of fields");

            AssertField(_expectedFirstField, result.ElementAt(0));
            AssertField(_expectedSecondField, result.ElementAt(1));
        }

        private static void AssertField(BvFilterFieldsEntity expected, BvFilterFieldsEntity actual)
        {
            Assert.AreEqual(expected.Table, actual.Table, "Wrong table type: expected {0}, actual {1}", (TableTypes)expected.Table, (TableTypes)actual.Table);
            Assert.AreEqual(expected.Column, actual.Column, "Wrong column type");
            Assert.AreEqual(expected.Type, actual.Type, "Wrong variable type: expected {0}, actual {1}", (VariableTypes)expected.Type, (VariableTypes)actual.Type);
            Assert.AreEqual(expected.Sign, actual.Sign, "Wrong operator: expected {0}, actual {1}", (FilterOperator)expected.Sign, (FilterOperator)actual.Sign);
            Assert.AreEqual(expected.Value, actual.Value, "Wrong value");
            Assert.AreEqual(expected.IsNeedCast, actual.IsNeedCast, "Wrong IsNeedCast flag");
        }
    }
}
