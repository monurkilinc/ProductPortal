using ProductPortal.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Entities.Aggregates
{
    public class Product : BaseEntity,IAggregateRoot
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

        //Domain Logic metodlari(DDD prensipleri ve CQRS Pattern icin eklendi)
        public void UpdateStock(int quantity)
        {
            if (quantity < 0)
            {
                throw new ArgumentOutOfRangeException("Stock negatif olamaz!!!");
                Stock = quantity;
            }
        }

        public void UpdatePrice(decimal price)
        {
            if (price < 0)
            {
                throw new ArgumentOutOfRangeException("Fiyat 0'dan buyuk olmali!!!");
            }
            Price=price;
        }

    }
}
