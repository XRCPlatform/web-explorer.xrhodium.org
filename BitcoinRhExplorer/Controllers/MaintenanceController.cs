using System.Web.Mvc;

namespace BitcoinRhExplorer.Controllers
{
    public class MaintenanceController : BaseController
    {
        public ActionResult Index()
        {
            var viewModel = ViewModel<CrudViewModel>();

            return View("Index", null, viewModel);
        }
    }
}