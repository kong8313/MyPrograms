using System;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes
{
    public class StubIDatabaseUpdateEngine : IDatabaseUpdateEngine 
    {
        private IDatabaseUpdateEngine _inner;

        public StubIDatabaseUpdateEngine()
        {
            _inner = null;
        }

        public IDatabaseUpdateEngine Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SaveUpdateScriptEventsDelegate();
        public SaveUpdateScriptEventsDelegate SaveUpdateScriptEvents;

        void IDatabaseUpdateEngine.SaveUpdateScriptEvents()
        {

            if (SaveUpdateScriptEvents != null)
            {
                SaveUpdateScriptEvents();
            } else if (_inner != null)
            {
                ((IDatabaseUpdateEngine)_inner).SaveUpdateScriptEvents();
            }
        }

        public delegate void ApplyUpdatesStringStringBooleanDelegate(string dbUpateUtilityVersion, string activeUser, bool commitTransaction);
        public ApplyUpdatesStringStringBooleanDelegate ApplyUpdatesStringStringBoolean;

        void IDatabaseUpdateEngine.ApplyUpdates(string dbUpateUtilityVersion, string activeUser, bool commitTransaction)
        {

            if (ApplyUpdatesStringStringBoolean != null)
            {
                ApplyUpdatesStringStringBoolean(dbUpateUtilityVersion, activeUser, commitTransaction);
            } else if (_inner != null)
            {
                ((IDatabaseUpdateEngine)_inner).ApplyUpdates(dbUpateUtilityVersion, activeUser, commitTransaction);
            }
        }

        public delegate void StopExecutionDelegate();
        public StopExecutionDelegate StopExecution;

        void IDatabaseUpdateEngine.StopExecution()
        {

            if (StopExecution != null)
            {
                StopExecution();
            } else if (_inner != null)
            {
                ((IDatabaseUpdateEngine)_inner).StopExecution();
            }
        }

        private string[] _DatabasesForUpgrade;
        public Func<string[]> DatabasesForUpgradeGet;
        public Action<string[]> DatabasesForUpgradeSetArrayOfString;

        string[] IDatabaseUpdateEngine.DatabasesForUpgrade
        {
            get
            {
                if (DatabasesForUpgradeGet != null)
                {
                    return DatabasesForUpgradeGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseUpdateEngine)_inner).DatabasesForUpgrade;
                }

                if (DatabasesForUpgradeSetArrayOfString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DatabasesForUpgrade;
                }

                return default(string[]);
            }

        }

    }
}