using System;
using Confirmit.CATI.Core.ActivityLogging;
using System.Collections.Generic;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;

namespace Confirmit.CATI.Core.ActivityLogging.Fakes
{
    public class StubIActivityEventsLoader : IActivityEventsLoader 
    {
        private IActivityEventsLoader _inner;

        public StubIActivityEventsLoader()
        {
            _inner = null;
        }

        public IActivityEventsLoader Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<Type> GetInterviewerActivityEventsDelegate();
        public GetInterviewerActivityEventsDelegate GetInterviewerActivityEvents;

        List<Type> IActivityEventsLoader.GetInterviewerActivityEvents()
        {


            if (GetInterviewerActivityEvents != null)
            {
                return GetInterviewerActivityEvents();
            } else if (_inner != null)
            {
                return ((IActivityEventsLoader)_inner).GetInterviewerActivityEvents();
            }

            return default(List<Type>);
        }

        public delegate List<Type> GetManagementActivityEventsDelegate();
        public GetManagementActivityEventsDelegate GetManagementActivityEvents;

        List<Type> IActivityEventsLoader.GetManagementActivityEvents()
        {


            if (GetManagementActivityEvents != null)
            {
                return GetManagementActivityEvents();
            } else if (_inner != null)
            {
                return ((IActivityEventsLoader)_inner).GetManagementActivityEvents();
            }

            return default(List<Type>);
        }

        public delegate EventDetails GetInterviewerActivityEventDetailsTypeDelegate(Type eventType);
        public GetInterviewerActivityEventDetailsTypeDelegate GetInterviewerActivityEventDetailsType;

        EventDetails IActivityEventsLoader.GetInterviewerActivityEventDetails(Type eventType)
        {


            if (GetInterviewerActivityEventDetailsType != null)
            {
                return GetInterviewerActivityEventDetailsType(eventType);
            } else if (_inner != null)
            {
                return ((IActivityEventsLoader)_inner).GetInterviewerActivityEventDetails(eventType);
            }

            return default(EventDetails);
        }

        public delegate EventDetails GetManagementActivityEventDetailsTypeDelegate(Type eventType);
        public GetManagementActivityEventDetailsTypeDelegate GetManagementActivityEventDetailsType;

        EventDetails IActivityEventsLoader.GetManagementActivityEventDetails(Type eventType)
        {


            if (GetManagementActivityEventDetailsType != null)
            {
                return GetManagementActivityEventDetailsType(eventType);
            } else if (_inner != null)
            {
                return ((IActivityEventsLoader)_inner).GetManagementActivityEventDetails(eventType);
            }

            return default(EventDetails);
        }

    }
}