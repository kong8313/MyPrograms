using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Config
{
    public static class ConfigValidator
    {
        private static readonly Regex _rgxPhoneNumber = new Regex(@"^\+[1-9]\d{1,14}$", RegexOptions.Compiled);
        
        public static void ValidateConfig(DialerConfigurationParameters dialerParams)
        {
            var fields = dialerParams.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(field => field.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var value = (string)field.GetValue(dialerParams);
                ValidateIsNotEmpty(value, field.Name);
            }

            ValidateIsValidUrl(dialerParams.AwsPublicApiUrl, nameof(dialerParams.AwsPublicApiUrl));
            ValidateIsValidUrl(dialerParams.AwsCallStatusQueueUrl, nameof(dialerParams.AwsCallStatusQueueUrl));
            
            if (!Guid.TryParse(dialerParams.AwsConnectId, out _))
                throw new DialerConfigurationException($"{nameof(dialerParams.AwsConnectId)} is invalid");
            if (!Guid.TryParse(dialerParams.AwsContactFlowId, out _))
                throw new DialerConfigurationException($"{nameof(dialerParams.AwsContactFlowId)} is invalid");
        }

        public static void ValidateConfig(DialerSurveyParameters config)
        {
            var sourcePhoneNumber = config.GetValue(DialerParameterKnownNames.SourcePhoneNumber);
            ValidatePhoneNumber(sourcePhoneNumber);

            var callerId = config.GetValue(DialerParameterKnownNames.CallerID);
            if (string.IsNullOrWhiteSpace(callerId))
                throw new DialerConfigurationException($"CallerId is not set");
        }

        public static void ValidatePhoneNumber(string phoneNumber)
        {
            ValidateIsNotEmpty(phoneNumber, "Phone number");
            
            if (!_rgxPhoneNumber.IsMatch(phoneNumber))
                throw new DialerConfigurationException($"Phone number format is invalid");
        }

        public static void ValidateIsNotEmpty(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DialerConfigurationException($"{fieldName} cannot be empty");
        }
        
        public static void ValidateIsValidUrl(string value, string fieldName)
        {
            ValidateIsNotEmpty(value, fieldName);

            if (!Uri.TryCreate(value, UriKind.Absolute, out _))
                throw new DialerConfigurationException($"{fieldName} must be a valid URL");
        }
    }
}