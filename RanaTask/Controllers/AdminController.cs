using Microsoft.AspNetCore.Mvc;

namespace ProductPortal.Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
