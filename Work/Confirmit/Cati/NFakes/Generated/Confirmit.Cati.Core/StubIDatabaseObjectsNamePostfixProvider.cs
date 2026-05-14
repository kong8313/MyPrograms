using System;
using Confirmit.CATI.Core.AsynchronousTrigger.Database;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Database.Fakes
{
    public class StubIDatabaseObjectsNamePostfixProvider : IDatabaseObjectsNamePostfixProvider 
    {
        private IDatabaseObjectsNamePostfixProvider _inner;

        public StubIDatabaseObjectsNamePostfixProvider()
        {
            _inner = null;
        }

        public IDatabaseObjectsNamePostfixProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetPostfixDelegate();
        public GetPostfixDelegate GetPostfix;

        string IDatabaseObjectsNamePostfixProvider.GetPostfix()
        {


            if (GetPostfix != null)
            {
                return GetPostfix();
            } else if (_inner != null)
            {
                return ((IDatabaseObjectsNamePostfixProvider)_inner).GetPostfix();
            }

            return default(string);
        }

    }
}