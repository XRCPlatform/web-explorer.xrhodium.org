using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BitcoinRhExplorer.Controllers
{
    public class ApiWikiController : BaseController
    {
        // GET: ApiWiki
        public ActionResult Index()
        {
            return View();
        }
    }
}