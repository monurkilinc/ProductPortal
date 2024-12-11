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

        public Task<Customer> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithOrders()
        {
            return await _context.Customers
                .Include(c => c.Orders)
            .ThenInclude(o => o.OrderItems)
            .AsNoTracking()
            .ToListAsync();
        }
    }
}
