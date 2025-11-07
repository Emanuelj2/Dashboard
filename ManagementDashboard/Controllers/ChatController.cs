using Microsoft.AspNetCore.Mvc;

namespace ManagementDashboard.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GroupChat()
        {
            return View();
        }

        public IActionResult PrivateChat()
        {
            return View();
        }
        public IActionResult PublicChat()
        {
            return View();
        }
    }
}
