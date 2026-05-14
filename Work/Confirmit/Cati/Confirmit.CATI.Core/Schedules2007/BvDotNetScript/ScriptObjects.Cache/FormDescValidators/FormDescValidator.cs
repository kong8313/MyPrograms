using System;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators
{
    public class FormDescValidator : IFormDescValidator
    {
        public ValidationResult Validate(object validationData, string value)
        {
            if (validationData is DateValidationData)
                return Validate(validationData as DateValidationData, value);

            if (validationData is SingleValidationData)
                return Validate(validationData as SingleValidationData, value);

            if (validationData is NumericValidationData)
                return Validate(validationData as NumericValidationData, value);

            if (validationData is OpenValidationData)
                return Validate(validationData as OpenValidationData, value);
            
            throw new NotImplementedException();
        }

        public ValidationResult Validate(DateValidationData validationData, string inputValue)
        {
            DateTime _minDate = DateTime.MinValue;
            DateTime _maxDate = DateTime.MaxValue;

            DateTime value;

            if (!DateTime.TryParse(inputValue, out value))
                return ValidationResult.Error("Can't parse " + inputValue + " into DateTime value");

            if (value < _minDate || value > _maxDate)
                return ValidationResult.Error(string.Format("Input value must be between {0} and {1}", _minDate, _maxDate));

            return ValidationResult.Success();
        }

        public ValidationResult Validate(SingleValidationData validationData, string value)
        {
            if (!validationData.PreCodes.ContainsKey(value))
            {
                return ValidationResult.Error("Single form does not contains preCode: " + value);
            }

            return ValidationResult.Success();
        }

        public ValidationResult Validate(NumericValidationData validationData, string value)
        {
            const double ConfirmitDoubleNull = -9999999999.0;

            double _maxValue = Double.MaxValue;
            bool _maxValueAllow = true;
            double _minValue = Double.MinValue;
            bool _minValueAllow = true;

            if (validationData.UpperLimit != ConfirmitDoubleNull)
            {
                _maxValue = validationData.UpperLimit;
                if (validationData.UpperLimitDataType == UpperLimitDataType.Smaller)
                    _maxValueAllow = false;
            }
            if (validationData.LowerLimit != ConfirmitDoubleNull)
            {
                _minValue = validationData.LowerLimit;
                if (validationData.LowerLimitType == LowerLimitDataType.Greater)
                    _minValueAllow = false;
            }

            double resultValue;

            if (!double.TryParse(value, out resultValue))
                return ValidationResult.Error("Cannot parse input value: " + value);

            if (resultValue < _minValue)
                return ValidationResult.Error("Input value: " + value + " cannot be less than min value: " + _minValue);

            if (!_minValueAllow && resultValue == _minValue)
                return ValidationResult.Error("Input value: " + value + " cannot be equal to min value: " + _minValue);

            if (resultValue > _maxValue)
                return ValidationResult.Error("Input value: " + value + " cannot be greater than max value: " + _maxValue);

            if (!_maxValueAllow && resultValue == _maxValue)
                return ValidationResult.Error("Input value: " + value + " cannot be equal to max value: " + _maxValue);

            return ValidationResult.Success();
        }

        public ValidationResult Validate(OpenValidationData validationData, string value)
        {
            return ValidationResult.Success();
        }
    }
}