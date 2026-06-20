using Microsoft.AspNetCore.Mvc;

namespace DIEMS.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}