using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Entities.Concrete
{
    public class SupportTicket : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Status { get; set; } // Open, InProgress, Resolved, Closed
        public string Priority { get; set; } // Low, Medium, High
        public List<SupportMessage> Messages { get; set; }
    }
}
