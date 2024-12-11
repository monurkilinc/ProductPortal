using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;
using StackExchange.Redis;

namespace ProductPortal.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public OrderController(IOrderService orderService, ICustomerService customerService, IProductService productService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _orderService.GetAllAsync();
            var orders = result.Data
                .OrderBy(o => o.Customer.Name)
                .ThenByDescending(o => o.CreatedDate)
                .ToList();
            return View(orders);
        }

        public async Task<IActionResult> CustomerOrders(int customerId)
        {
            var result = await _orderService.GetOrdersByCustomerIdAsync(customerId);
            return View(result.Data);
        }
        public async Task<IActionResult> CreateOrder()
        {
            ViewBag.Customers = (await _customerService.GetAllCustomersWithOrdersAsync()).Data;
            ViewBag.Products = (await _productService.GetAllAsync()).Data;
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(int customerId, int[] productIds, int[] quantities, DateTime estimatedDeliveryDate, string paymentStatus)
        {

            if (!ModelState.IsValid)
            {
                await LoadViewDataAsync();
                return View();
            }

            var customerResult = await _customerService.GetByIdAsync(customerId);
            if (customerResult is null || customerResult.Data is null)
            {
                ModelState.AddModelError("", "Müşteri bulunamadı.");
                await LoadViewDataAsync();
                return View();
            }

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            for (int i = 0; i < productIds.Length; i++)
            {
                var productResult = await _productService.GetByIdAsync(productIds[i]);
                if (productResult?.Data == null)
                {
                    ModelState.AddModelError("", $"Ürün bulunamadı: {productIds[i]}");
                    await LoadViewDataAsync();
                    return View();
                }

                var product = productResult.Data;
                var quantity = quantities[i];
                var itemTotal = product.Price * quantity;
                totalAmount += itemTotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    UnitPrice = product.Price
                });
            }

            var order = new Core.Entities.Concrete.Order
            {
                CustomerId = customerId,
                Status = "Beklemede",
                EstimatedDeliveryDate = estimatedDeliveryDate,
                PaymentStatus = paymentStatus,
                TotalAmount = totalAmount,
                OrderItems = orderItems
            };

            var orderResult = await _orderService.AddAsync(order);
            if (!orderResult.Success)
            {
                ModelState.AddModelError("", "Sipariş oluşturulurken bir hata oluştu.");
                await LoadViewDataAsync();
                return View();
            }

            TempData["SuccessMessage"] = "Sipariş başarıyla oluşturuldu!";
            return RedirectToAction("Index");
            
        }

        [HttpGet]
        public async Task<IActionResult> EditOrder(int id)
        {
            var result = await _orderService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["ErrorMessage"] = "Sipariş bulunamadı.";
                return RedirectToAction("Index");
            }
            var order = result.Data;
            if (order.OrderItems == null)
            {
                order.OrderItems = new List<OrderItem>();
            }
            ViewBag.Products = (await _productService.GetAllAsync()).Data;
            if (order.Customer == null || ViewBag.Products == null)
            {
                TempData["ErrorMessage"] = "Sipariş detayları yüklenirken bir hata oluştu.";
                return RedirectToAction("Index");
            }
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrder(int id, int[] productIds, int[] quantities, string status, string paymentStatus, DateTime? estimatedDeliveryDate)
        {
            Core.Entities.Concrete.Order order = null; 
            try
            {
                order = new Core.Entities.Concrete.Order
                {
                    Id = id,
                    Status = status,
                    PaymentStatus = paymentStatus,
                    EstimatedDeliveryDate = estimatedDeliveryDate,
                    OrderItems = new List<OrderItem>()
                };

                for (int i = 0; i < productIds.Length; i++)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = productIds[i],
                        Quantity = quantities[i]
                    });
                }

                var result = await _orderService.UpdateAsync(order);
                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Message;
                    ViewBag.Products = (await _productService.GetAllAsync()).Data;
                    return View(order);
                }

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Bir hata oluştu: " + ex.Message;
                if (order != null)
                {
                    ViewBag.Products = (await _productService.GetAllAsync()).Data;
                    return View(order);
                }
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        public async Task LoadViewDataAsync()
        {
            ViewBag.Customers = (await _customerService.GetAllCustomersWithOrdersAsync()).Data;
            ViewBag.Products = (await _productService.GetAllAsync()).Data;
        }
    }
}

