using Microsoft.EntityFrameworkCore;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.DataAccess.Abstract;
using ProductPortal.DataAccess.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.DataAccess.Concrete
{
    public class EfUserRepository : EfGenericRepository<User>, IUserRepository
    {
        public EfUserRepository(ProductPortalContext context) : base(context)
        {
        }

        public async Task<User> GetByEmail(string email)
        {
           return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> GetByUserName(string userName)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Username == userName);
        }
    }
}
