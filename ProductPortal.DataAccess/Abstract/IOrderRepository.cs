using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.DataAccess.Abstract
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByCustomerId(int customerId);
        Task<IEnumerable<Order>> GetAllAsync();
    }
}
