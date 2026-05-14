using System;
using Confirmit.CATI.Core.Misc.CP;

namespace Confirmit.CATI.Core.Misc.CP.Fakes
{
    public class StubISupervisorNameProvider : ISupervisorNameProvider 
    {
        private ISupervisorNameProvider _inner;

        public StubISupervisorNameProvider()
        {
            _inner = null;
        }

        public ISupervisorNameProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _Name;
        public Func<string> NameGet;
        public Action<string> NameSetString;

        string ISupervisorNameProvider.Name
        {
            get
            {
                if (NameGet != null)
                {
                    return NameGet();
                } else if (_inner != null)
                {
                    return ((ISupervisorNameProvider)_inner).Name;
                }

                if (NameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Name;
                }

                return default(string);
            }

        }

    }
}