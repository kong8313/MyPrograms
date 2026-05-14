using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Services.RecordsMigration
{
    public class MigrationService : IMigrationService
    {
        private readonly IDeferredMonitoringRepository _deferredMonitoringRepository;
        
        public MigrationService(IDeferredMonitoringRepository deferredMonitoringRepository)
        {
            _deferredMonitoringRepository = deferredMonitoringRepository;
        }
        
        public (int totalRecordsCount, int migratedRecordsCount, int failedRecordsCount, int alreadyMigratedRecordsCount) MigrateDeferredRecords()
        {
            Trace.TraceInformation("Start migration of saved deferred records.");
            
            int migratedRecordsCount = 0;
            int failedRecordsCount = 0;
            int alreadyMigratedRecordsCount = 0;
            
            var savedRecords = _deferredMonitoringRepository.GetAllSavedRecords();
            int totalRecordsCount = savedRecords.Count;
            
            foreach (var record in savedRecords)
            {
                var eventsFile = record.EventsFile;
                List<MonitoringEvent> stateEvents;

                try
                {
                    new JsonSerializationStateEventInfoDepacker(eventsFile).GetAllEvents();
                    alreadyMigratedRecordsCount++;
                    Trace.TraceInformation($"Deferred record with ID {record.ID} is already migrated.");
                    continue;
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation($"Couldn't deserialize record with ID {record.ID} by json converter. Error: " + ex.Message);
                }

                try
                {
                    stateEvents = new BinaryFormatterStateEventInfoDepacker(eventsFile).GetAllEvents();
                }
                catch (Exception ex)
                {
                    failedRecordsCount++;
                    Trace.TraceError($"Couldn't deserialize record with ID {record.ID} by binary converter. Error: " + ex);
                    continue;
                }
                
                try
                {
                    record.EventsFile = new StateEventInfoPacker(stateEvents).SerializeAllEvents();
                }
                catch (Exception ex)
                {
                    failedRecordsCount++;
                    Trace.TraceError($"An error occurred during json serialization of the deferred record with ID {record.ID}: {ex}");
                    continue;
                }
                
                _deferredMonitoringRepository.UpdateRecord(record);
                migratedRecordsCount++;
                Trace.TraceInformation($"Deferred record with ID {record.ID} was migrated successfully.");
            }
            
            Trace.TraceInformation($"End migration of saved deferred records. Total records: {totalRecordsCount}, migrated records: {migratedRecordsCount}, failed records: {failedRecordsCount}, already migrated records: {alreadyMigratedRecordsCount}");
            
            return (totalRecordsCount, migratedRecordsCount, failedRecordsCount, alreadyMigratedRecordsCount);
        }
    }
}