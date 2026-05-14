using System;
using Confirmit.CATI.Core.Services.Database.Interfaces;

namespace Confirmit.CATI.Core.Services.Database.Interfaces.Fakes
{
    public class StubIDatabaseStatisticService : IDatabaseStatisticService 
    {
        private IDatabaseStatisticService _inner;

        public StubIDatabaseStatisticService()
        {
            _inner = null;
        }

        public IDatabaseStatisticService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void UpdateStatisticStringDelegate(string tableName);
        public UpdateStatisticStringDelegate UpdateStatisticString;

        void IDatabaseStatisticService.UpdateStatistic(string tableName)
        {

            if (UpdateStatisticString != null)
            {
                UpdateStatisticString(tableName);
            } else if (_inner != null)
            {
                ((IDatabaseStatisticService)_inner).UpdateStatistic(tableName);
            }
        }

    }
}