using System;
using Confirmit.CATI.Common;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Persons;

namespace Confirmit.CATI.Supervisor.Core.Persons.Fakes
{
    public class StubISetDialType : ISetDialType 
    {
        private ISetDialType _inner;

        public StubISetDialType()
        {
            _inner = null;
        }

        public ISetDialType Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SetDialTypeIEnumerableOfInt32Delegate(DialType dialType, IEnumerable<int> personIds);
        public SetDialTypeIEnumerableOfInt32Delegate SetDialTypeIEnumerableOfInt32;

        void ISetDialType.Set(DialType dialType, IEnumerable<int> personIds)
        {

            if (SetDialTypeIEnumerableOfInt32 != null)
            {
                SetDialTypeIEnumerableOfInt32(dialType, personIds);
            } else if (_inner != null)
            {
                ((ISetDialType)_inner).Set(dialType, personIds);
            }
        }

    }
}