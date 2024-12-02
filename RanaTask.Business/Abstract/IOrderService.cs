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
    public interface IOrderService
    {
        Task<IDataResult<Order>> AddAsync(Order order);
        Task<IDataResult<List<Order>>> GetAllAsync();
        Task<IDataResult<List<Order>>> GetOrdersByCustomerIdAsync(int customerId);
        Task<IDataResult<Order>> GetByIdAsync(int id);
        Task<IDataResult<Order>> UpdateAsync(Order order);
        Task<IResult> DeleteAsync(int id);
    }
}
