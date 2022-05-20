using System;
using System.Configuration;
using System.Web.Mvc;

namespace BitcoinRhExplorer.App_Start
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }

    public class CustomRequireHttpsFilter : RequireHttpsAttribute
    {
        protected override void HandleNonHttpsRequest(AuthorizationContext filterContext)
        {
            var forceHttps = ConfigurationManager.AppSettings["Explorer_forceHTTPS"];

            if (filterContext.HttpContext.Request.IsLocal.Equals(false) 
                && (forceHttps != null) 
                && (forceHttps.ToString().ToLower() == "true"))
            {

                //if (!string.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)
                //    && !string.Equals(filterContext.HttpContext.Request.HttpMethod, "HEAD", StringComparison.OrdinalIgnoreCase))
                //{
                //    //base.HandleNonHttpsRequest(filterContext);
                //}

                string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectResult(url, true);
            }
        }
    }

}