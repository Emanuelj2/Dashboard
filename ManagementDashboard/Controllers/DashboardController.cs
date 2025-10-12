using Microsoft.AspNetCore.Mvc;

namespace ManagementDashboard.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
