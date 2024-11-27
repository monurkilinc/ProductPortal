using Microsoft.AspNetCore.Http;
using ProductPortal.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Entities.DTOs
{
    public class UserUpdateDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string? Email { get; set; }
        public string? Department { get; set; }
        public bool? IsAdmin { get; set; }
        public bool Role {  get; set; } 
        public bool IsActive { get; set; }
        public IFormFile? ImageFile { get; set; }
        public List<PermissionType>? Permissions { get; set; }

        
       
    }
}
