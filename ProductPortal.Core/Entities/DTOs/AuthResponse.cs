using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Entities.DTOs
{
    public class AuthResponse
    {
        public AccessToken AccessToken { get; set; }
        public User User { get; set; }
    }
}
