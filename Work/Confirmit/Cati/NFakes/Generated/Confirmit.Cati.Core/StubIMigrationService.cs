using System;
using Confirmit.CATI.Core.Services.RecordsMigration;

namespace Confirmit.CATI.Core.Services.RecordsMigration.Fakes
{
    public class StubIMigrationService : IMigrationService 
    {
        private IMigrationService _inner;

        public StubIMigrationService()
        {
            _inner = null;
        }

        public IMigrationService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate ValueTuple<int, int, int, int> MigrateDeferredRecordsDelegate();
        public MigrateDeferredRecordsDelegate MigrateDeferredRecords;

        ValueTuple<int, int, int, int> IMigrationService.MigrateDeferredRecords()
        {


            if (MigrateDeferredRecords != null)
            {
                return MigrateDeferredRecords();
            } else if (_inner != null)
            {
                return ((IMigrationService)_inner).MigrateDeferredRecords();
            }

            return default(ValueTuple<int, int, int, int>);
        }

    }
}