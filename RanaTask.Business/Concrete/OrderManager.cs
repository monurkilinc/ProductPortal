using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Core.Utilities.Results;
using ProductPortal.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Business.Concrete
{
    public class OrderManager : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<Result> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderManager(IOrderRepository orderRepository,ILogger<Result> logger,IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<IDataResult<Order>> AddAsync(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<Core.Utilities.Results.IResult> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IDataResult<List<Order>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IDataResult<Order>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
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

        public Task<IDataResult<Order>> UpdateAsync(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
