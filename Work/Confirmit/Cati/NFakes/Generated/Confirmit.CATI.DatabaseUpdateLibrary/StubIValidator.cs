using System;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes
{
    public class StubIValidator : IValidator 
    {
        private IValidator _inner;

        public StubIValidator()
        {
            _inner = null;
        }

        public IValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CheckDatabasesArrayOfStringDelegate(string[] productionDatabases);
        public CheckDatabasesArrayOfStringDelegate CheckDatabasesArrayOfString;

        void IValidator.CheckDatabases(string[] productionDatabases)
        {

            if (CheckDatabasesArrayOfString != null)
            {
                CheckDatabasesArrayOfString(productionDatabases);
            } else if (_inner != null)
            {
                ((IValidator)_inner).CheckDatabases(productionDatabases);
            }
        }

        public delegate void CheckUpdateScriptsDelegate();
        public CheckUpdateScriptsDelegate CheckUpdateScripts;

        void IValidator.CheckUpdateScripts()
        {

            if (CheckUpdateScripts != null)
            {
                CheckUpdateScripts();
            } else if (_inner != null)
            {
                ((IValidator)_inner).CheckUpdateScripts();
            }
        }

    }
}