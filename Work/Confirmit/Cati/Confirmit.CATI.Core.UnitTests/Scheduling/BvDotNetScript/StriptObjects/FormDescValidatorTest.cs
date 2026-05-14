using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Scheduling.BvDotNetScript.StriptObjects
{
    [TestClass]
    public class FormDescValidatorTest : BaseTest
    {
        [TestMethod(), Owner(@"FIRM\OlegM")]
        public void dateValidator_should_validate_correct()
        {
            var validator = new FormDescValidator();
            string[] inputCorrectValues = {"2008-05-01T07:34:42-5:00", 
                              "2008-05-01 7:34:42Z", 
                              "Thu, 01 May 2008 07:34:42 GMT",
                                   "03/01/2009 05:42:00 -5:00",
                                   "2016-10-10"};

            string[] inputIncorrectValues = {"0000-1-1 00:00:00", "test", 
                              "10000-01-01"};

            var validatorObj = new DateValidationData();

            foreach (var value in inputCorrectValues)
            {
                AssertCorrectValue(value, validatorObj, validator);
            }

            foreach (var value in inputIncorrectValues)
            {
                AssertIncorrectValue(value, validatorObj, validator);
            }
        }

        [TestMethod(), Owner(@"FIRM\OlegM")]
        public void numericValidator_should_validate_correct()
        {
            var validator = new FormDescValidator();

            if (!CultureInfo.CurrentCulture.NumberFormat.IsReadOnly)
            {
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
                CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator = "";
            }

            List<string> inputBorderValues = new List<string> { "-2", "10" };
            List<string> inputInboundValues = new List<string> { "3", "9.5", "-1.55555555" };
            List<string> inputOutboundValues = new List<string> { "-3", "19", "4566.445" };
            List<string> inputIncorrectValues = new List<string> { "30qw", "-63;33", "3,1233" };

           // check open interval
            var validatorObj = new NumericValidationData(UpperLimitDataType.Smaller, 10d, LowerLimitDataType.Greater, -2d);
            inputBorderValues.ForEach(value => AssertIncorrectValue(value, validatorObj, validator));
            inputInboundValues.ForEach(value => AssertCorrectValue(value, validatorObj, validator));
            inputOutboundValues.ForEach(value => AssertIncorrectValue(value, validatorObj, validator));
            inputIncorrectValues.ForEach(value => AssertIncorrectValue(value, validatorObj, validator));

            // check closed interval
            validatorObj = new NumericValidationData(UpperLimitDataType.SmallerOrEqual, 10d, LowerLimitDataType.GreaterOrEqual, -2d);
            inputBorderValues.ForEach(value => AssertCorrectValue(value, validatorObj, validator));
            inputInboundValues.ForEach(value => AssertCorrectValue(value, validatorObj, validator));
            inputOutboundValues.ForEach(value => AssertIncorrectValue(value, validatorObj, validator));
            inputIncorrectValues.ForEach(value => AssertIncorrectValue(value, validatorObj, validator));
        }

        [TestMethod(), Owner(@"FIRM\OlegM")]
        public void singleValidator_should_validate_correct()
        {
             var validator = new FormDescValidator();
             AssertCorrectValue("a1", new SingleValidationData(new Dictionary<string, string>{ {"a1", null}, {"a2", null} }), validator);
             AssertIncorrectValue("a3", new SingleValidationData(new Dictionary<string, string> { { "a1", null }, { "a2", null } }), validator);
        }

        private void AssertIncorrectValue(string checkValue, ValidationData validatorData, IFormDescValidator validator)
        {
            var validationResult = validator.Validate(validatorData, checkValue);
            Assert.IsFalse(validationResult.IsSuccess);
            Assert.IsFalse(string.IsNullOrEmpty(validationResult.ErrorMessage));
        }

        private void AssertCorrectValue(string checkValue, ValidationData validatorData, IFormDescValidator validator)
        {
            var validationResult = validator.Validate(validatorData, checkValue);
            
            if (!validationResult.IsSuccess)
            {
                Trace.TraceError(validationResult.ErrorMessage);
            }

            Assert.IsTrue(validationResult.IsSuccess);
            Assert.IsTrue(string.IsNullOrEmpty(validationResult.ErrorMessage));
        }
    }
}