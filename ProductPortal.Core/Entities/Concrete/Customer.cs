using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Entities.Concrete
{
    public class Customer : BaseEntity
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon zorunludur")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Geçersiz telefon formatı")]
        [StringLength(20)]
        public string Phone { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
