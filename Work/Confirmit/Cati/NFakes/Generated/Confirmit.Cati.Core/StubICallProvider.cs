using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubICallProvider : ICallProvider 
    {
        private ICallProvider _inner;

        public StubICallProvider()
        {
            _inner = null;
        }

        public ICallProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvCallEntity GetCallAndNoLockInt32Int32Delegate(int surveySid, int interviewId);
        public GetCallAndNoLockInt32Int32Delegate GetCallAndNoLockInt32Int32;

        BvCallEntity ICallProvider.GetCallAndNoLock(int surveySid, int interviewId)
        {


            if (GetCallAndNoLockInt32Int32 != null)
            {
                return GetCallAndNoLockInt32Int32(surveySid, interviewId);
            } else if (_inner != null)
            {
                return ((ICallProvider)_inner).GetCallAndNoLock(surveySid, interviewId);
            }

            return default(BvCallEntity);
        }

        public delegate BvCallEntity GetCallAndNoLockInt32Int32Int32BooleanDelegate(int surveySid, int interviewId, int batchId, bool isSampleUpdateMode);
        public GetCallAndNoLockInt32Int32Int32BooleanDelegate GetCallAndNoLockInt32Int32Int32Boolean;

        BvCallEntity ICallProvider.GetCallAndNoLock(int surveySid, int interviewId, int batchId, bool isSampleUpdateMode)
        {


            if (GetCallAndNoLockInt32Int32Int32Boolean != null)
            {
                return GetCallAndNoLockInt32Int32Int32Boolean(surveySid, interviewId, batchId, isSampleUpdateMode);
            } else if (_inner != null)
            {
                return ((ICallProvider)_inner).GetCallAndNoLock(surveySid, interviewId, batchId, isSampleUpdateMode);
            }

            return default(BvCallEntity);
        }

    }
}