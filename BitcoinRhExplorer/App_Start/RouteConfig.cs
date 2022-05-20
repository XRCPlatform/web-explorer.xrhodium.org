using System.Web.Mvc;
using System.Web.Routing;

namespace BitcoinRhExplorer.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "BlockByHeight",
                url: "{controller}/BlockByHeight/{height}",
                defaults: new { controller = "btr", action = "BlockByHeight" }
            );
            routes.MapRoute(
                name: "Block",
                url: "{controller}/Block/{hash}",
                defaults: new { controller = "btr", action = "Block" }
            );
            routes.MapRoute(
                name: "Tx",
                url: "{controller}/Tx/{hash}",
                defaults: new { controller = "btr", action = "Tx" }
            );
            routes.MapRoute(
                name: "Address",
                url: "{controller}/Address/{hash}",
                defaults: new { controller = "btr", action = "Address" }
            );

            routes.MapRoute(
                name: "BlockByHeightXrc",
                url: "{controller}/BlockByHeight/{height}",
                defaults: new { controller = "xrc", action = "BlockByHeight" }
            );
            routes.MapRoute(
                name: "BlockXrc",
                url: "{controller}/Block/{hash}",
                defaults: new { controller = "xrc", action = "Block" }
            );
            routes.MapRoute(
                name: "TxXrc",
                url: "{controller}/Tx/{hash}",
                defaults: new { controller = "xrc", action = "Tx" }
            );
            routes.MapRoute(
                name: "AddressXrc",
                url: "{controller}/Address/{hash}",
                defaults: new { controller = "xrc", action = "Address" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
