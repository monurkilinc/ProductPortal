using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Entities.Concrete
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
    }
}
