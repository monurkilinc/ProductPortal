using Microsoft.EntityFrameworkCore;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.DataAccess.Abstract;
using ProductPortal.DataAccess.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.DataAccess.Concrete
{
    public class EfCustomerRepository : EfGenericRepository<Customer>, ICustomerRepository
    {
        public EfCustomerRepository(ProductPortalContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithOrders()
        {
            return await _context.Customers
                .Include(x=>x.Orders)
                .ThenInclude(x=>x.OrderItems)
                .ToListAsync();
        }
    }
}
