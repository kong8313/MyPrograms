using System;

namespace Confirmit.CATI.Supervisor.ActivityViews
{
    public interface ICookieDataAccess
    {
        void SetValue(string cookieName, string cookieValue, DateTime expires);
        string GetValue(string cookieName);
    }
}