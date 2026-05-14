using System;
using Confirmit.CATI.Core.Services.Interfaces.Database;

namespace Confirmit.CATI.Core.Services.Interfaces.Database.Fakes
{
    public class StubIDatabaseNameService : IDatabaseNameService 
    {
        private IDatabaseNameService _inner;

        public StubIDatabaseNameService()
        {
            _inner = null;
        }

        public IDatabaseNameService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetLinkToConfirmlogDatabaseDelegate();
        public GetLinkToConfirmlogDatabaseDelegate GetLinkToConfirmlogDatabase;

        string IDatabaseNameService.GetLinkToConfirmlogDatabase()
        {


            if (GetLinkToConfirmlogDatabase != null)
            {
                return GetLinkToConfirmlogDatabase();
            } else if (_inner != null)
            {
                return ((IDatabaseNameService)_inner).GetLinkToConfirmlogDatabase();
            }

            return default(string);
        }

    }
}