using ProductPortal.Core.Entities.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Utilities.Security
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(User user);
        Task<AccessToken> CreateAccessTokenAsync(User user);
    }
}
