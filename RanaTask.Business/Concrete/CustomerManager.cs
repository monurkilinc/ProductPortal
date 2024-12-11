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

        public async Task<IDataResult<Customer>> AddAsync(Customer entity)
        {
            try
            {
                var customer = await _customerRepository.AddAsync(entity);
                return new SuccessDataResult<Customer>(_logger, _httpContextAccessor, customer, "Müşteri başarıyla eklendi");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Customer>(_logger, _httpContextAccessor, "Müşteri eklenirken hata oluştu");
            }
        }

        public async Task<Core.Utilities.Results.IResult> DeleteAsync(int id)
        {
            try
            {
                await _customerRepository.DeleteAsync(id);
                return new SuccessResult(_logger, _httpContextAccessor, "Müşteri silindi");
            }
            catch (Exception ex)
            {
                return new ErrorResult(_logger, _httpContextAccessor, "Müşteri silinemedi");
            }
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
                return new ErrorDataResult<List<Customer>>(_logger, _httpContextAccessor, "Müşteriler listelenemedi");
            }
        }

        public async Task<IDataResult<Customer>> GetByIdAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                return new SuccessDataResult<Customer>(_logger, _httpContextAccessor, customer);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Customer>(_logger, _httpContextAccessor, "Müşteri bulunamadı");
            }
        }

        public Task<Customer> GetCustomerByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<IDataResult<Customer>> UpdateAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(customer.Id);
                if (existingCustomer == null)
                    return new ErrorDataResult<Customer>(_logger, _httpContextAccessor, "Müşteri bulunamadı");

                var customers = await _customerRepository.GetAllAsync();
                if (existingCustomer.Email != customer.Email &&
                    customers.Any(x => x.Email == customer.Email))
                    return new ErrorDataResult<Customer>(_logger, _httpContextAccessor, "Email kullanımda");

                existingCustomer.Name = customer.Name;
                existingCustomer.Email = customer.Email;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.UpdatedDate = DateTime.Now;

                var updatedCustomer = await _customerRepository.UpdateAsync(existingCustomer);
                return new SuccessDataResult<Customer>(_logger, _httpContextAccessor, updatedCustomer, "Müşteri güncellendi");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Customer>(_logger, _httpContextAccessor, "Güncelleme başarısız");
            }
        }
        

       
    }
}
