using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.ActivityLogging.SiteSettings
{
    public abstract class UpdateSiteSettingsEventBase : ManagementActivityEvent<UpdateSiteSettingsParameters>
    {
        private IDictionary<string, object> _siteSettings;

        public bool HasChanges { get; set; }

        protected UpdateSiteSettingsEventBase(ManagementEvent eventType): base(ManagementEventCategory.SystemSettings, eventType)
        {
        }

        protected abstract Dictionary<string, object> GetSiteSettingsAsDictionary(ISystemSettings systemSettings);

        public void RememberSettings(ISystemSettings systemSettings)
        {
            _siteSettings = GetSiteSettingsAsDictionary(systemSettings);
        }

        public void CollectChangedSettings(ISystemSettings systemSettings)
        {
            var currentSettings = GetSiteSettingsAsDictionary(systemSettings);
            if (_siteSettings == null)
            {
                return;
            }

            var changes = new List<string>();
            foreach (var oldValuePair in _siteSettings)
            {
                var newValuePair = currentSettings.Single(k => k.Key.Equals(oldValuePair.Key));
                NoteIfChanged(oldValuePair.Value, newValuePair.Value, oldValuePair.Key, changes);
            }

            Details = new UpdateSiteSettingsParameters
            {
                ChangedSettings = string.Format(
                    "Following system settings were changed:\r\n{0}",
                    string.Join(Environment.NewLine, changes.ToArray())
                    )
            };

            HasChanges = changes.Count > 0;
        }

        protected void NoteIfChanged(object oldValue, object newValue, string name, ICollection<string> changes)
        {
            if (oldValue == null && newValue == null)
            {
                return;
            }

            if (oldValue == null || !oldValue.Equals(newValue))
            {
                changes.Add(string.Format("'{0}' from '{1}' to '{2}'", name, oldValue, newValue));
            }
        }

    }
}
