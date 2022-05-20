using System;
using System.Web;

namespace BitcoinRhExplorer.Components
{
    public static class LayoutHelper
    {
        public static bool IsParralaxViewed()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["_parralax"];
            if (cookie == null)
            {
                cookie = new HttpCookie("_parralax");
                cookie.Value = "true";
                cookie.Expires = DateTime.Now.AddDays(1);
                HttpContext.Current.Response.Cookies.Add(cookie);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}