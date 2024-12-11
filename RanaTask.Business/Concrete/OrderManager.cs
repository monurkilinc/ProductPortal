using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Core.Utilities.Results;
using ProductPortal.DataAccess.Abstract;

namespace ProductPortal.Business.Concrete
{
    public class OrderManager : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly ILogger<Result> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderManager(IOrderRepository orderRepository, IProductService productService, ILogger<Result> logger,IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IDataResult<Order>> AddAsync(Order order)
        {
            try
            {
                var addedOrder = await _orderRepository.AddAsync(order);
                return new SuccessDataResult<Order>(_logger, _httpContextAccessor, addedOrder, "Sipariş başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Order>(_logger, _httpContextAccessor, "Sipariş oluşturulamadı: " + ex.Message);
            }
        }

        public async Task<Core.Utilities.Results.IResult> DeleteOrder(int id)
        {
            try
            {
                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder is null)
                {
                    return new ErrorDataResult<Order>(
                        _logger,
                        _httpContextAccessor,
                        "Silinecek sipariş bulunamadı");
                }

                await _orderRepository.DeleteAsync(id);

                return new SuccessDataResult<Order>(
                    _logger,
                    _httpContextAccessor,
                    existingOrder,
                    "Sipariş başarıyla silindi",
                    200);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Order>(
                    _logger,
                    _httpContextAccessor,
                    "Sipariş silinirken bir hata oluştu!");
            }
        }

        public async Task<IDataResult<List<Order>>> GetAllAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                return new SuccessDataResult<List<Order>>(_logger,_httpContextAccessor, orders.ToList(), "Sipariş başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<Order>>(_logger, _httpContextAccessor, "Sipariş getirilirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<IDataResult<Order>> GetByIdAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    return new ErrorDataResult<Order>(_logger, _httpContextAccessor, "Sipariş bulunamadı");
                }
                return new SuccessDataResult<Order>(_logger, _httpContextAccessor, order, "Sipariş başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Order>(_logger, _httpContextAccessor, "Sipariş getirilirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<IDataResult<List<Order>>> GetOrdersByCustomerIdAsync(int customerId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByCustomerId(customerId);
                return new SuccessDataResult<List<Order>>(_logger, _httpContextAccessor, orders.ToList());
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<Order>>(_logger, _httpContextAccessor, ex.Message);
            }
        }

        public async Task<IDataResult<Order>> UpdateAsync(Order order)
        {
            try
            {
                if (order.OrderItems == null || !order.OrderItems.Any())
                    return new ErrorDataResult<Order>(_logger, _httpContextAccessor, "Sipariş ürün içermelidir.");

                decimal totalAmount = 0;

                foreach (var item in order.OrderItems)
                {
                    var productResult = await _productService.GetByIdAsync(item.ProductId);
                    if (productResult?.Data == null) continue;
                    totalAmount += productResult.Data.Price * item.Quantity;
                    item.UnitPrice = productResult.Data.Price;
                }
                order.TotalAmount = totalAmount;

                var updatedOrder = await _orderRepository.UpdateAsync(order);
                return new SuccessDataResult<Order>(_logger, _httpContextAccessor, updatedOrder, "Sipariş güncellendi");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Order>(_logger, _httpContextAccessor, ex.Message);
            }
        }
       
    }
}
