using System;
using Confirmit.CATI.Core.Services.Database.Interfaces;

namespace Confirmit.CATI.Core.Services.Database.Interfaces.Fakes
{
    public class StubIDatabaseIdentifierService : IDatabaseIdentifierService 
    {
        private IDatabaseIdentifierService _inner;

        public StubIDatabaseIdentifierService()
        {
            _inner = null;
        }

        public IDatabaseIdentifierService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetEscapedIdentifierStringDelegate(string identifier);
        public GetEscapedIdentifierStringDelegate GetEscapedIdentifierString;

        string IDatabaseIdentifierService.GetEscapedIdentifier(string identifier)
        {


            if (GetEscapedIdentifierString != null)
            {
                return GetEscapedIdentifierString(identifier);
            } else if (_inner != null)
            {
                return ((IDatabaseIdentifierService)_inner).GetEscapedIdentifier(identifier);
            }

            return default(string);
        }

    }
}