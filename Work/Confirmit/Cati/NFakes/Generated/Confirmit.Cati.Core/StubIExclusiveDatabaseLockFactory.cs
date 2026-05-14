using System;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;

namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation.Fakes
{
    public class StubIExclusiveDatabaseLockFactory : IExclusiveDatabaseLockFactory 
    {
        private IExclusiveDatabaseLockFactory _inner;

        public StubIExclusiveDatabaseLockFactory()
        {
            _inner = null;
        }

        public IExclusiveDatabaseLockFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate ExclusiveDatabaseLock CreateStringDelegate(string respourceName);
        public CreateStringDelegate CreateString;

        ExclusiveDatabaseLock IExclusiveDatabaseLockFactory.Create(string respourceName)
        {


            if (CreateString != null)
            {
                return CreateString(respourceName);
            } else if (_inner != null)
            {
                return ((IExclusiveDatabaseLockFactory)_inner).Create(respourceName);
            }

            return default(ExclusiveDatabaseLock);
        }

    }
}