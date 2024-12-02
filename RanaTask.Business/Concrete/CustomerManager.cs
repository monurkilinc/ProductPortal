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
    public class CustomerManager:ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<Result> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerManager(ICustomerRepository customerRepository, ILogger<Result> logger, IHttpContextAccessor httpContextAccessor)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<IDataResult<Customer>> AddAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        public Task<Core.Utilities.Results.IResult> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IDataResult<List<Customer>>> GetAllCustomersWithOrdersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetCustomersWithOrders();
                return new SuccessDataResult<List<Customer>>(_logger, _httpContextAccessor, customers.ToList());
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<Customer>>(_logger, _httpContextAccessor, ex.Message);
            }
        }

        public Task<IDataResult<Customer>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IDataResult<Customer>> UpdateAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
