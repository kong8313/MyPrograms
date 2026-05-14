using System;

namespace Confirmit.CATI.Common.Logging
{
    public class EventDetailsScope : IDisposable
    {
        [ThreadStatic]
        private static EventDetailsScope current;

        private readonly EventDetailsScope parent;

        private readonly IEventDetails _eventDetails;

        public EventDetailsScope(IEventDetails eventDetails)
        {
            if (eventDetails == null)
            {
                throw new ArgumentNullException("eventDetails");
            }

            this.parent = current;
            this._eventDetails = eventDetails;
            current = this;
        }

        public void Dispose()
        {
            current = this.parent;
        }

        public static IEventDetails Current
        {
            get
            {
                return current != null ?  current._eventDetails : new DummyEventDetails();
            }
        }
    }
}
