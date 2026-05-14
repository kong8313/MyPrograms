using System;
using Confirmit.CATI.Supervisor.ActivityViews;

namespace Confirmit.CATI.Supervisor.ActivityViews.Fakes
{
    public class StubICookieDataAccess : ICookieDataAccess 
    {
        private ICookieDataAccess _inner;

        public StubICookieDataAccess()
        {
            _inner = null;
        }

        public ICookieDataAccess Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SetValueStringStringDateTimeDelegate(string cookieName, string cookieValue, DateTime expires);
        public SetValueStringStringDateTimeDelegate SetValueStringStringDateTime;

        void ICookieDataAccess.SetValue(string cookieName, string cookieValue, DateTime expires)
        {

            if (SetValueStringStringDateTime != null)
            {
                SetValueStringStringDateTime(cookieName, cookieValue, expires);
            } else if (_inner != null)
            {
                ((ICookieDataAccess)_inner).SetValue(cookieName, cookieValue, expires);
            }
        }

        public delegate string GetValueStringDelegate(string cookieName);
        public GetValueStringDelegate GetValueString;

        string ICookieDataAccess.GetValue(string cookieName)
        {


            if (GetValueString != null)
            {
                return GetValueString(cookieName);
            } else if (_inner != null)
            {
                return ((ICookieDataAccess)_inner).GetValue(cookieName);
            }

            return default(string);
        }

    }
}