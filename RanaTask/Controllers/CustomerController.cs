using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;

namespace ProductPortal.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _customerService.GetAllCustomersWithOrdersAsync();
            return View(result.Data);
        }
        public IActionResult Create()
        {
            return View(new Customer());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.AddAsync(customer);
                if (result.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(customer);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Customer customer)
        {
            var result = await _customerService.UpdateAsync(customer);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var result = await _customerService.DeleteAsync(id);
            return Json(new { success = result.Success });
        }

    }
}
