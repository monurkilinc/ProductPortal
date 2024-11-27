using Microsoft.AspNetCore.Http;
using ProductPortal.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Entities.DTOs
{
    public class UserCreateDTO
    {
        [Required]
        [MinLength(5)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(5)]
        public string Password { get; set; }

        public string Department { get; set; }
        public bool IsAdmin { get; set; }
        public bool Role { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
