using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;

namespace ProductPortal.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _orderService.GetAllAsync();
            return View(result.Data);
        }

        public async Task<IActionResult> CustomerOrders(int customerId)
        {
            var result = await _orderService.GetOrdersByCustomerIdAsync(customerId);
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                var result = await _orderService.AddAsync(order);
                if (result.Success)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", result.Message);
            }
            return View(order);
        }
    }

}
