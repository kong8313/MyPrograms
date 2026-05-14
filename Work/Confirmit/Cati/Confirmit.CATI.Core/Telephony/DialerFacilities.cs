using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using ConfirmitDialerInterface;
using DialerCommon;

namespace Confirmit.CATI.Core.Telephony
{
    class DialerFacilities : IDialerFacilities
    {
        private readonly ITelephony _telephony;
        private readonly IDialerCollection _dialerCollection;
        private readonly IDialerFeaturesRepository _dialerFeaturesRepository;

        public DialerFacilities(ITelephony telephony, IDialerCollection dialerCollection,
            IDialerFeaturesRepository dialerFeaturesRepository)
        {
            _telephony = telephony;
            _dialerCollection = dialerCollection;
            _dialerFeaturesRepository = dialerFeaturesRepository;
        }

        public string GetDialerVersion(int dialerId)
        {
            var dialer = _dialerCollection.GetDialerById(dialerId);
            return dialer.Version;
        }

        public DialerAvailableExtendedFunctionality GetAvailableExtendedFunctionality(int dialerId)
        {
            var evt = new GetAvailableExtendedFunctionalityEvent(dialerId);

            var result = new DialerAvailableExtendedFunctionality { IsLogGetterSupported = false };

            var currentVersion = GetVersion(dialerId);

            if (currentVersion >= new Version(3, 6, 9))
                result.IsLogGetterSupported = true;

            evt.Finish();

            return result;
        }

        private Version GetVersion(int dialerId)
        {
            var verArr = _telephony.GetDialerVersion(dialerId).Split('.');
            if (verArr.Length < 3) return null;

            if (!int.TryParse(verArr[0], out var major)) return null;
            if (!int.TryParse(verArr[1], out var minor)) return null;
            if (!int.TryParse(verArr[2], out var rev)) return new Version(major, minor);

            return new Version(major, minor, rev);
        }

        public IEnumerable<LogFileInfo> GetLogFiles(int dialerId)
        {
            var evt = new GetLogFilesEvent(dialerId);
            var result = _telephony.GetLogFiles(dialerId);
            evt.Details.IsSuccessful = true;
            evt.Details.Count = result.Count();
            evt.Finish();

            return result;
        }

        public byte[] GetLogFileBodyZipped(int dialerId, string fileName)
        {
            var evt = new GetLogFileBodyZippedEvent(dialerId, fileName);
            byte[] result = null;
            result = _telephony.GetLogFileBodyZipped(dialerId, fileName);
            evt.Details.IsSuccessful = true;
            evt.Details.Length = result.Length;
            evt.Finish();

            return result;
        }

        public DialerFeatures GetDialerSupportedFeatures(int dialerId)
        {
            var evt = new GetDialerSupportedFeaturesEvent(dialerId);
            var dialer = _dialerCollection.GetDialerById(dialerId);
            var dialerFeatures = dialer.SupportedFeatures ?? new DialerFeatures();

            var overridenFeatures = _dialerFeaturesRepository.GetAll(dialerId);

            var dfProperties = typeof(DialerFeatures).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in dfProperties)
            {
                var value = overridenFeatures.FirstOrDefault(x => x.Name == property.Name);
                if (value != null)
                    property.SetValue(dialerFeatures, value.Value);
            }

            evt.Finish();

            return dialerFeatures;
        }

        public IEnumerable<DialerOverridenFeature> GetOverridenDialerSupportedFeatures(int dialerId)
        {
            var evt = new GetOverridenDialerSupportedFeaturesEvent(dialerId);
            var dialer = _dialerCollection.GetDialerById(dialerId);
            var dialerFeatures = dialer.SupportedFeatures ?? new DialerFeatures();

            var overridenFeatures = _dialerFeaturesRepository.GetAll(dialerId);
            var result = new List<DialerOverridenFeature>();

            var dfProperties = typeof(DialerFeatures).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in dfProperties)
            {
                if (property.GetValue(dialerFeatures) is bool value)
                    result.Add(new DialerOverridenFeature(property.Name, value,
                        overridenFeatures.FirstOrDefault(x => x.Name == property.Name)?.Value));
                else if (property.GetValue(dialerFeatures) == null)
                    result.Add(new DialerOverridenFeature(property.Name, null,
                        overridenFeatures.FirstOrDefault(x => x.Name == property.Name)?.Value));
                else
                    throw new Exception($"Non supported Dialer feature type: feature - {property.Name}, type - {property.PropertyType}");
            }

            evt.Finish();

            return result;
        }

        public void UpdateOverridenDialerSupportedFeature(int dialerId, string featureName, bool? overridenFeatureValue)
        {
            var evt = new UpdateOverridenDialerSupportedFeatureEvent(dialerId);
            if (overridenFeatureValue.HasValue)
                _dialerFeaturesRepository.UpdateOrInsert(new BvDialerFeaturesEntity
                {
                    DialerId = dialerId,
                    Name = featureName,
                    Value = overridenFeatureValue.Value
                });
            else
                _dialerFeaturesRepository.Delete(dialerId, featureName);

            evt.Finish();
        }
    }

}