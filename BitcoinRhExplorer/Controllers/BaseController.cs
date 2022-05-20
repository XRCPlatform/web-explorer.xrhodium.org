using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BitcoinRhExplorer.Components;
using BitcoinRhExplorer.Entities.Users;
using BitcoinRhExplorer.Library;
using BitcoinRhExplorer.Resources;
using BitcoinRhExplorer.Server;

namespace BitcoinRhExplorer.Controllers
{
    public abstract class BaseController : Controller
    {
        public const string LOGINERROR = "LoginError";
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            HttpContext.SetOverriddenBrowser(BrowserOverride.Desktop);
            string cultureName = null;

            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null) {
                cultureName = cultureCookie.Value;
            }
            else
            {
                cultureName = 
                    Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null;
            }

            //validujeme a případně vracíme defaultní (ajinu)
            cultureName = CultureHelper.GetImplementedCulture(cultureName);

            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }

        protected bool PerformAction(Action action, string token = null)
        {
            try
            {
                action();
                return true;
            }
            catch (BitcoinRhExplorerException ex)
            {
                BitcoinRhExplorerServer.Current.Errors.AddWithToken(ex.Exceptions.First(), token);
                return false;
            }
            catch (Exception ex)
            {
                BitcoinRhExplorerServer.Current.Errors.AddWithToken(ex.Message + ex.StackTrace, token);
                Trace.TraceError(ex.Message + ex.StackTrace);
                return false;
            }
        }

        protected TViewModel ViewModel<TViewModel>()
            where TViewModel : new()
        {
            var viewModel = new TViewModel();
            return viewModel;
        }
    }

    public class CrudViewModel
    {
        public string Title { get; set; }
        public bool ShowParralax { get; set; }
    }

    public class CrudViewModel<TEntity> : CrudViewModel where TEntity : new()
    {
        public TEntity Entity { get; set; }
        public IEnumerable<TEntity> Entities { get; set; }
        public string EntityTypeName
        {
            get { return typeof(TEntity).Name; }
        }

        public CrudViewModel()
        {
            Entity = new TEntity();
        }
    }

    public static class View
    {
        public static string Title
        {
            get { return ConfigurationManager.AppSettings["Explorer_Title"]; }
        }

        public static string MenuLinks
        {
            get { return ConfigurationManager.AppSettings["Explorer_MenuLinks"]; }
        }

        public static IView Current
        {
            get { return WebPageContext.Current.Page as IView; }
        }

        public static WebViewPage CurrentPage
        {
            get { return WebPageContext.Current.Page as WebViewPage; }
        }

        public static HtmlHelper<object> Html
        {
            get { return CurrentPage.Html; }
        }

        public static HttpRequestBase Request
        {
            get { return CurrentPage.Request; }
        }

        public static bool IsScmsAdmin
        {
            get
            {
                if (Request.IsAuthenticated)
                {
                    return ((UserIdentity)CurrentPage.User.Identity).IsInRole(UserRole.SuperAdmin.ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public static UrlHelper Url
        {
            get { return CurrentPage.Url; }
        }

        public static IEnumerable<string> Cultures
        {
            get { return CultureHelper.GetCultures(); }
        }

        public static CultureInfo CurrentCulture
        {
            get { return CultureHelper.GetCurrentCulture(); }
        }

        public static UserIdentity User
        {
            get { return (UserIdentity)CurrentPage.User.Identity; }
        }

        public static string GetDateTimePattern
        {
            get { 
                if (CurrentCulture.Name.Contains(Enum.GetName(typeof(CultureHelper.CulturesAbbreviation), CultureHelper.CulturesAbbreviation.cs)))
                {
                    return "d.M.yyyy H:mm:ss";
                }
                else
                {
                    return "M/d/yyyy H:mm:ss";
                }
            }
        }

        public static string GetDatePattern
        {
            get
            {
                if (CurrentCulture.Name.Contains(Enum.GetName(typeof(CultureHelper.CulturesAbbreviation), CultureHelper.CulturesAbbreviation.cs)))
                {
                    return "d.M.yyyy";
                }
                else
                {
                    return "M/d/yyyy";
                }
            }
        }

        public static string GetJSDateTimePattern
        {
            get
            {
                if (CurrentCulture.Name.Contains(Enum.GetName(typeof(CultureHelper.CulturesAbbreviation), CultureHelper.CulturesAbbreviation.cs)))
                {
                    return "d.m.yyyy h:ii:ss";
                }
                else
                {
                    return "M/d/yyyy h:ii:ss";
                }
            }
        }

        public static string GetJSDatePattern
        {
            get
            {
                if (CurrentCulture.Name.Contains(Enum.GetName(typeof(CultureHelper.CulturesAbbreviation),  CultureHelper.CulturesAbbreviation.cs))) {
                    return "d.m.yyyy";
                } else {
                    return "m/d/yyyy";
                }
            }
        }

        public static string GetJSShortDatePattern
        {
            get
            {
                if (CurrentCulture.Name.Contains(Enum.GetName(typeof(CultureHelper.CulturesAbbreviation), CultureHelper.CulturesAbbreviation.cs)))
                {
                    return "d.m.yy";
                }
                else
                {
                    return "m/d/yy";
                }
            }
        }
    }
}