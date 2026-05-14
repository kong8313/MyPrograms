using System;
using System.Web;

namespace Confirmit.CATI.Supervisor.ActivityViews
{
    public class CookieDataAccess : ICookieDataAccess
    {
        public void SetValue(string cookieName, string cookieValue, DateTime expires)
        {
            var response = HttpContext.Current.Response;
            var cookie = response.Cookies.Get(cookieName);
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName, cookieValue) { Expires = expires };
                response.Cookies.Add(cookie);
            }
            else
            {
                cookie.Expires = expires;
                cookie.Value = cookieValue;
                response.Cookies.Set(cookie);
            }
        }

        public string GetValue(string cookieName)
        {
            var cookie = HttpContext.Current.Request.Cookies.Get(cookieName);
            if (cookie == null)
                return null;

            return cookie.Value;
        }
    }
}