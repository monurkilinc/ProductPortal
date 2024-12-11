using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Entities.Concrete
{
    public class Order : BaseEntity
    {
        public int CustomerId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? UpdatedDate { get; set; } 

        public string PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Beklemede";

        public Customer Customer { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }


    }

}
