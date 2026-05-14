using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;

namespace Confirmit.CATI.Backend.TimezoneManager
{
    public class TimezoneUpdateManager
    {
        private readonly ITimezoneRepository _timezoneRepository;
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;
        private readonly ICompanyInfo _companyInfo;
        
        public TimezoneUpdateManager(
            ITimezoneRepository timezoneRepository, IDatabaseLockTimeouts databaseLockTimeouts, ICompanyInfo companyInfo)
        {
            _timezoneRepository = timezoneRepository;
            _databaseLockTimeouts = databaseLockTimeouts;
            _companyInfo = companyInfo;
        }

        public void UpdateTimezones()
        {
            try
            {
                var lockFactory = new ExclusiveDatabaseLockFactory(
                    "TimezoneManager", _databaseLockTimeouts.TimezoneUpdateLockTimeoutInMs);

                using (var dbLock = lockFactory.Create(DatabaseLockTimeoutsAndRecourceNames.TimezoneManagerResourceName))
                {
                    if (dbLock.TryEnterLock())
                    {
                        var timezoneUpdateEvent = new TimezoneUpdateEvent(new List<BvTimezoneEntity>(), new List<BvTimezoneEntity>());

                        var timezoneProvider = new TimeZoneDataProvider();

                        if (_companyInfo.CompanyId != 0)
                        {
                            UpdateMasterTimezonesFromDefaultDatabase();
                        }

                        var systemTimezones = timezoneProvider.GetSystemTimeZones();

                        var allTimezones = _timezoneRepository.GetMasterList();
                        var activeTimezones = _timezoneRepository.GetActiveList();

                        var timezoneAnalyzer = new TimeZoneAnalyzer();

                        timezoneAnalyzer.GenerateIdForSystemTimezones(systemTimezones, allTimezones);

                        var newTimezones = timezoneAnalyzer.GetNewTimezones(systemTimezones, allTimezones);
                        var updatedTimezones = timezoneAnalyzer.GetUpdatedTimezones(systemTimezones, allTimezones);

                        if (newTimezones.Count != 0 || updatedTimezones.Count != 0)
                        {
                            timezoneUpdateEvent.Details.UpdatedTimezones = updatedTimezones;
                            timezoneUpdateEvent.Details.NewTimezones = newTimezones;

                            foreach (var timezone in newTimezones)
                            {
                                _timezoneRepository.InsertMasterEntity(timezone);
                            }

                            foreach (var timezone in updatedTimezones)
                            {
                                var isActiveTimeZone = activeTimezones.Any(x => x.StandardName == timezone.StandardName);

                                _timezoneRepository.UpdateMasterEntity(timezone, isActiveTimeZone);
                            }

                            timezoneUpdateEvent.Finish();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);
            }

        }

        private void UpdateMasterTimezonesFromDefaultDatabase()
        {
            var timezones = _timezoneRepository.GetMasterList();
            var timezonesFromDefaultDb = _timezoneRepository.GetMasterListFromDefaultDatabase();

            foreach (var timezoneFromDefaultDb in timezonesFromDefaultDb)
            {
                if (timezones.All(x => x.StandardName != timezoneFromDefaultDb.StandardName))
                {
                    _timezoneRepository.InsertMasterEntity(timezoneFromDefaultDb);
                }
            }
        }
    }
}