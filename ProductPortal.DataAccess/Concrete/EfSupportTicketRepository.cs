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
    public class EfSupportTicketRepository : EfGenericRepository<SupportTicket>, ISupportTicketRepository
    {
        public EfSupportTicketRepository(ProductPortalContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SupportTicket>> GetTicketsByCustomerId(int customerId)
        {
            return await _context.SupportTickets
                .Include(t => t.Messages)
                .Where(t => t.CustomerId == customerId)
                .ToListAsync();
        }
    }
}
