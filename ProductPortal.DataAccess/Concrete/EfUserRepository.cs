using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ProductPortalContext _context;
        private readonly ILogger<EfUserRepository> _logger;
        public EfUserRepository(ProductPortalContext context, ILogger<EfUserRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }
        public async Task DeleteAsync(User user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                _logger.LogInformation($"Kullanıcı silme işlemi başlatıldı. ID: {user.Id}");

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Kullanıcı başarıyla silindi. ID: {user.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Kullanıcı silinirken hata oluştu. ID: {user.Id}");
                throw;
            }
        }

        public async Task<User> GetByEmail(string email)
        {
           return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        }

        public Task<List<User>> GetByRoleAsync(string role)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByUserName(string userName)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Username == userName);
        }

        public Task<int> GetCountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalCountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUsersByRoleAsync(string role)
        {
            throw new NotImplementedException();
        }
    }
}
