namespace Confirmit.CATI.Core.Services.RecordsMigration
{
    public interface IMigrationService
    {
        (int totalRecordsCount, int migratedRecordsCount, int failedRecordsCount, int alreadyMigratedRecordsCount) MigrateDeferredRecords();
    }
}