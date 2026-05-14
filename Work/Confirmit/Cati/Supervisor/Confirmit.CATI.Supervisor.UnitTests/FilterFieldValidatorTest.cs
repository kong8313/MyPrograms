using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Classes.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class FilterFieldValidatorTest
    {
        private FilterFieldValidator _validator;

        private BvFilterFieldsEntity _predefinedField;

        [TestInitialize]
        public void TestInitialize()
        {
            _validator = new FilterFieldValidator();
            _predefinedField = new BvFilterFieldsEntity
            {
                Table = (int)TableTypes.Interview,
                Column = "column",
                FilterSID = 0,
                IsNeedCast = false,
                ID = 0,
                Type = (int)VariableTypes.PredefinedValue,
                Sign = (int)FilterOperator.Equal,
                Value = "fsd"
            };
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(UserMessageException))]
        public void Validate_IntegerFieldWithNonNumericValue_ExceptionIsThrown()
        {
            var field = new BvFilterFieldsEntity
                            {
                                Table = (int) TableTypes.Interview,
                                Column = "column",
                                FilterSID = 0,
                                IsNeedCast = false,
                                ID = 0,
                                Type = (int) VariableTypes.Integer,
                                Sign = (int) FilterOperator.Bigger,
                                Value = "fsd"
                            };

            _validator.Validate(field);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(UserMessageException))]
        public void Validate_SubfilterFieldWithNonNumericValue_ExceptionIsThrown()
        {
            var field = new BvFilterFieldsEntity
            {
                Table = (int)TableTypes.Interview,
                Column = "column",
                FilterSID = 0,
                IsNeedCast = false,
                ID = 0,
                Type = (int)VariableTypes.Subfilter,
                Sign = (int)FilterOperator.Equal,
                Value = "fsd"
            };

            _validator.Validate(field);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_PredefinedFieldWithIncorrectOperator_ExceptionIsThrown()
        {
            var unsupportedOperators =
                Enum.GetValues(typeof (FilterOperator)).Cast<FilterOperator>()
                    .Where(filterOp => filterOp != FilterOperator.Equal && filterOp != FilterOperator.NotEqual);

            foreach (var op in unsupportedOperators)
            {
                try
                {
                    _predefinedField.Sign = (int) op;
                    _validator.Validate(_predefinedField);
                }
                catch (UserMessageException)
                {
                    continue;
                }

                Assert.Fail("Operator {0} shouldn't be supported by Predefined list field type.", op);
            }
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_PredefinedFieldWithCorrectOperator_Success()
        {
            var supportedOperators = new[] {FilterOperator.Equal, FilterOperator.NotEqual};

            foreach (var op in supportedOperators)
            {
                try
                {
                    _predefinedField.Sign = (int)op;
                    _validator.Validate(_predefinedField);
                }
                catch (UserMessageException)
                {
                    Assert.Fail("Operator {0} should be supported by Predefined list field type.", op);
                }
            }
        }
    }
}
