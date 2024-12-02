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
    public class SupportTicketManager : ISupportTicketService
    {
        private readonly ISupportTicketRepository _ticketRepository;
        private readonly ILogger<Result> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SupportTicketManager(ISupportTicketRepository ticketRepository, ILogger<Result> logger, IHttpContextAccessor httpContextAccessor)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<IDataResult<SupportTicket>> AddAsync(SupportTicket ticket)
        {
            throw new NotImplementedException();
        }

        public Task<Core.Utilities.Results.IResult> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IDataResult<List<SupportTicket>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IDataResult<SupportTicket>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IDataResult<List<SupportTicket>>> GetTicketsByCustomerIdAsync(int customerId)
        {
            try
            {
                var tickets = await _ticketRepository.GetTicketsByCustomerId(customerId);
                return new SuccessDataResult<List<SupportTicket>>(_logger, _httpContextAccessor, tickets.ToList());
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<SupportTicket>>(_logger, _httpContextAccessor, ex.Message);
            }
        }

        public Task<IDataResult<SupportTicket>> UpdateAsync(SupportTicket ticket)
        {
            throw new NotImplementedException();
        }
    }
}