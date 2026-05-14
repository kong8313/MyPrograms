using System;
using Confirmit.CATI.Common.Logging;

namespace Confirmit.CATI.Common.Logging.Fakes
{
    public class StubIEventDetails : IEventDetails 
    {
        private IEventDetails _inner;

        public StubIEventDetails()
        {
            _inner = null;
        }

        public IEventDetails Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AddTimingStringDelegate(string timingName);
        public AddTimingStringDelegate AddTimingString;

        void IEventDetails.AddTiming(string timingName)
        {

            if (AddTimingString != null)
            {
                AddTimingString(timingName);
            } else if (_inner != null)
            {
                ((IEventDetails)_inner).AddTiming(timingName);
            }
        }

        public delegate void AddTimingStringInt32Delegate(string timingName, int minimumTimingToIgnore);
        public AddTimingStringInt32Delegate AddTimingStringInt32;

        void IEventDetails.AddTiming(string timingName, int minimumTimingToIgnore)
        {

            if (AddTimingStringInt32 != null)
            {
                AddTimingStringInt32(timingName, minimumTimingToIgnore);
            } else if (_inner != null)
            {
                ((IEventDetails)_inner).AddTiming(timingName, minimumTimingToIgnore);
            }
        }

        public delegate void AddTimingStringArrayOfObjectDelegate(string format, Object[] args);
        public AddTimingStringArrayOfObjectDelegate AddTimingStringArrayOfObject;

        void IEventDetails.AddTiming(string format, Object[] args)
        {

            if (AddTimingStringArrayOfObject != null)
            {
                AddTimingStringArrayOfObject(format, args);
            } else if (_inner != null)
            {
                ((IEventDetails)_inner).AddTiming(format, args);
            }
        }

        public delegate void AddMessageStringArrayOfObjectDelegate(string format, Object[] args);
        public AddMessageStringArrayOfObjectDelegate AddMessageStringArrayOfObject;

        void IEventDetails.AddMessage(string format, Object[] args)
        {

            if (AddMessageStringArrayOfObject != null)
            {
                AddMessageStringArrayOfObject(format, args);
            } else if (_inner != null)
            {
                ((IEventDetails)_inner).AddMessage(format, args);
            }
        }

    }
}