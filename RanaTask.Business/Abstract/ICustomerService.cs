using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Business.Abstract
{
    public interface ICustomerService
    {
        Task<IDataResult<Customer>> AddAsync(Customer customer);
        Task<IDataResult<Customer>> UpdateAsync(Customer customer);
        Task<IResult> DeleteAsync(int id);

        Task<IDataResult<List<Customer>>> GetAllCustomersWithOrdersAsync();
        Task<IDataResult<Customer>> GetByIdAsync(int id);
        Task<Customer> GetCustomerByEmailAsync(string email);

    }
}
