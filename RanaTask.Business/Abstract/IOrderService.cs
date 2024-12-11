using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Core.Utilities.Results;

namespace ProductPortal.Business.Abstract
{
    public interface IOrderService
    {
        Task<IDataResult<Order>> AddAsync(Order order);
        Task<IDataResult<Order>> UpdateAsync(Order order);
        Task<IResult> DeleteOrder(int id);

        Task<IDataResult<Order>> GetByIdAsync(int id);
        Task<IDataResult<List<Order>>> GetOrdersByCustomerIdAsync(int customerId);
        Task<IDataResult<List<Order>>> GetAllAsync();



    }
}
