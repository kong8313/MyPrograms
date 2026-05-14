using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIScheduleRepository : IScheduleRepository 
    {
        private IScheduleRepository _inner;

        public StubIScheduleRepository()
        {
            _inner = null;
        }

        public IScheduleRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvScheduleEntity GetByIdInt32Delegate(int scheduleId);
        public GetByIdInt32Delegate GetByIdInt32;

        BvScheduleEntity IScheduleRepository.GetById(int scheduleId)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(scheduleId);
            } else if (_inner != null)
            {
                return ((IScheduleRepository)_inner).GetById(scheduleId);
            }

            return default(BvScheduleEntity);
        }

        public delegate BvScheduleEntity GetByNameStringDelegate(string name);
        public GetByNameStringDelegate GetByNameString;

        BvScheduleEntity IScheduleRepository.GetByName(string name)
        {


            if (GetByNameString != null)
            {
                return GetByNameString(name);
            } else if (_inner != null)
            {
                return ((IScheduleRepository)_inner).GetByName(name);
            }

            return default(BvScheduleEntity);
        }

        public delegate int InsertWithSpecificIdBvScheduleEntityDelegate(BvScheduleEntity schedule);
        public InsertWithSpecificIdBvScheduleEntityDelegate InsertWithSpecificIdBvScheduleEntity;

        int IScheduleRepository.InsertWithSpecificId(BvScheduleEntity schedule)
        {


            if (InsertWithSpecificIdBvScheduleEntity != null)
            {
                return InsertWithSpecificIdBvScheduleEntity(schedule);
            } else if (_inner != null)
            {
                return ((IScheduleRepository)_inner).InsertWithSpecificId(schedule);
            }

            return default(int);
        }

    }
}