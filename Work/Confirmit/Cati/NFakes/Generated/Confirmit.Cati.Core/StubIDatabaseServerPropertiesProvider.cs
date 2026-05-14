using System;
using Confirmit.CATI.Core.Services.Database.Interfaces;

namespace Confirmit.CATI.Core.Services.Database.Interfaces.Fakes
{
    public class StubIDatabaseServerPropertiesProvider : IDatabaseServerPropertiesProvider 
    {
        private IDatabaseServerPropertiesProvider _inner;

        public StubIDatabaseServerPropertiesProvider()
        {
            _inner = null;
        }

        public IDatabaseServerPropertiesProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate EngineEdition GetEngineEditionDelegate();
        public GetEngineEditionDelegate GetEngineEdition;

        EngineEdition IDatabaseServerPropertiesProvider.GetEngineEdition()
        {


            if (GetEngineEdition != null)
            {
                return GetEngineEdition();
            } else if (_inner != null)
            {
                return ((IDatabaseServerPropertiesProvider)_inner).GetEngineEdition();
            }

            return default(EngineEdition);
        }

        public delegate Version GetProductVersionDelegate();
        public GetProductVersionDelegate GetProductVersion;

        Version IDatabaseServerPropertiesProvider.GetProductVersion()
        {


            if (GetProductVersion != null)
            {
                return GetProductVersion();
            } else if (_inner != null)
            {
                return ((IDatabaseServerPropertiesProvider)_inner).GetProductVersion();
            }

            return default(Version);
        }

    }
}