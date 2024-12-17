using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Aggregates;
using ProductPortal.Core.Entities.DTOs;

namespace ProductPortal.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IUserService _userService;
        public AdminController(ILogger<AdminController> logger, IUserService userService)
        {
            _userService = userService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            try
            {
                var username = User.Identity.Name;
                var currentUser = await _userService.GetUserByNameAsync(username);
                ViewBag.CurrentUser = currentUser.Data;

                var result = await _userService.GetAllUsersAsync();
                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(new List<User>());
                }
                return View(result.Data ?? new List<User>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kullanıcılar listelenirken hata oluştu";
                return View(new List<User>());
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateDTO createDto)
        {
            if (!ModelState.IsValid)
                return View(createDto);

            var result = await _userService.CreateUserAsync(createDto);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(createDto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            var updateDto = new UserUpdateDTO
            {
                Id = result.Data.Id,
                Username = result.Data.Username,
                Password = result.Data.Password,
                Email = result.Data.Email,
                Department = result.Data.Department,
                Role = result.Data.IsAdmin,
                IsAdmin = result.Data.IsAdmin,
                IsActive = result.Data.IsActive
            };
            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserUpdateDTO updateDto)
        {
            if (!ModelState.IsValid)
                return View(updateDto);

            var result = await _userService.UpdateUserAsync(updateDto);
            if (!result.Success)
            {
                return View(updateDto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                  
                    return RedirectToAction(nameof(Index));
                }
                var result = await _userService.DeleteUserAsync(id);

                if (result.Success)
                {
                    _logger.LogInformation($"Kullanıcı başarıyla silindi. ID: {id}");
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    _logger.LogWarning($"Kullanıcı silinemedi. ID: {id}, Hata: {result.Message}");
                    TempData["ErrorMessage"] = result.Message;
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Kullanıcı silme işlemi sırasında hata oluştu. ID: {id}");
                TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _userService.ToggleUserStatusAsync(id);
            if (!result.Success)
                TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

    }
}
