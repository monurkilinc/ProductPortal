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

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _customerService.GetAllCustomersWithOrdersAsync();
            return View(result.Data);
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
    }
}
