using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductPortal.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            try
            {
                var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

                _logger.LogInformation($"Admin panel accessed by {username} with role {userRole}");

                ViewBag.UserRole = userRole;
                ViewBag.Username = username;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing admin panel");
                return RedirectToAction("Login", "Auth");
            }
        }
    }
}
