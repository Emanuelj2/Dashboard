using ManagementDashboard.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ManagementDashboard.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }


        //GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        //Post: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.User user)
        {
            if (ModelState.IsValid)
            {
                try
                {   
                    await _userService.AddUserAsync(user);
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the user.");
                }
            }
            return View(user);
        }
    }
}
