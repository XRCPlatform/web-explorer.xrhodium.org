using BitcoinRhExplorer.Server;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BitcoinRhExplorer.App_Start;
using Forloop.HtmlHelpers;

namespace BitcoinRhExplorer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            BitcoinRhExplorerServer.Current.Initialize(null,
                BitcoinRhExplorerServer.EntityFrameworkLevel2CacheDefinitionTypes.CacheNothingOnlyDefined);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AuthConfig.RegisterAuth();
            AreaRegistration.RegisterAllAreas();
            ScriptContext.ScriptPathResolver = Scripts.Render;
        }
        protected void Session_Start(object sender, EventArgs e)
        {
            Session["initTime"] = DateTime.UtcNow;    
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.IsLocal.Equals(false))
            {
                if (Request.IsSecureConnection.Equals(false))
                {
                    Response.Redirect("https://explorer.xrhodium.org" + Request.RawUrl);
                }
            }
        }
    }
}