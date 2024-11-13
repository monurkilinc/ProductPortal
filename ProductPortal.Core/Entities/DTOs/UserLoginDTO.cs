using System;
using System.ComponentModel.DataAnnotations;

namespace ProductPortal.Core.Entities.DTOs
{
    public class UserLoginDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
