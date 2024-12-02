using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Entities.Concrete
{
    public class SupportMessage : BaseEntity
    {
        public int TicketId { get; set; }
        public SupportTicket Ticket { get; set; }
        public string Message { get; set; }
        public bool IsCustomerMessage { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
