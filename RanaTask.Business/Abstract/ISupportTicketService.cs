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
    public interface ISupportTicketService
    {
        Task<IDataResult<SupportTicket>> AddAsync(SupportTicket ticket);
        Task<IDataResult<List<SupportTicket>>> GetAllAsync();
        Task<IDataResult<List<SupportTicket>>> GetTicketsByCustomerIdAsync(int customerId);
        Task<IDataResult<SupportTicket>> GetByIdAsync(int id);
        Task<IDataResult<SupportTicket>> UpdateAsync(SupportTicket ticket);
        Task<IResult> DeleteAsync(int id);
    }
}
