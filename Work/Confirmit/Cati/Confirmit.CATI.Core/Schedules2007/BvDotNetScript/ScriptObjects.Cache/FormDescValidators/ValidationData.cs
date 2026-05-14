using System.Collections.Generic;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators
{
    public abstract class ValidationData
    {
    }

    public class OpenValidationData : ValidationData
    {
    }

    public class DateValidationData : ValidationData
    {
    }

    public class NumericValidationData : ValidationData
    {
        public UpperLimitDataType UpperLimitDataType { get; private set; }
        public LowerLimitDataType LowerLimitType { get; private set; }
        public double UpperLimit { get; private set; }
        public double LowerLimit { get; private set; }
        

        public NumericValidationData(UpperLimitDataType upperLimitType, double upperLimit, LowerLimitDataType lowerLimitType, double lowerLimit)
        {
            UpperLimitDataType = upperLimitType;
            LowerLimitType = lowerLimitType;
            UpperLimit = upperLimit;
            LowerLimit = lowerLimit;
        }
    }

    public class SingleValidationData : ValidationData
    {
        public readonly Dictionary<string, string> PreCodes = new Dictionary<string, string>();

        public SingleValidationData(Dictionary<string, string> preCodes)
        {
            PreCodes = preCodes;
        }
    }
}