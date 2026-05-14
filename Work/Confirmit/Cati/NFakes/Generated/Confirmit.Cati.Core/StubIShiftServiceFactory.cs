using System;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIShiftServiceFactory : IShiftServiceFactory 
    {
        private IShiftServiceFactory _inner;

        public StubIShiftServiceFactory()
        {
            _inner = null;
        }

        public IShiftServiceFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IShiftService GetInt32Delegate(int scheduleId);
        public GetInt32Delegate GetInt32;

        IShiftService IShiftServiceFactory.Get(int scheduleId)
        {


            if (GetInt32 != null)
            {
                return GetInt32(scheduleId);
            } else if (_inner != null)
            {
                return ((IShiftServiceFactory)_inner).Get(scheduleId);
            }

            return default(IShiftService);
        }

        public delegate void DropScheduleCacheDelegate();
        public DropScheduleCacheDelegate DropScheduleCache;

        void IShiftServiceFactory.DropScheduleCache()
        {

            if (DropScheduleCache != null)
            {
                DropScheduleCache();
            } else if (_inner != null)
            {
                ((IShiftServiceFactory)_inner).DropScheduleCache();
            }
        }

    }
}