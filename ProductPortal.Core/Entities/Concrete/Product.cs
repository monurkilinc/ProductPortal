using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Entities.Concrete
{
    public class Product : BaseEntity
    {
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Ürün kodu zorunludur.")]
        public string Code { get; set; }

        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı sıfır veya daha büyük olmalıdır.")]
        public int Stock { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat sıfırdan büyük olmalıdır.")]
        public decimal Price { get; set; }

        public string? ImageURL { get; set; }
    }
}
