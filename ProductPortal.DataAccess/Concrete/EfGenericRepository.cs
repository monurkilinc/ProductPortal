using Microsoft.EntityFrameworkCore;
using ProductPortal.Core.Entities;
using ProductPortal.DataAccess.Abstract;
using ProductPortal.DataAccess.Contexts;

namespace ProductPortal.DataAccess.Concrete
{
    public class EfGenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ProductPortalContext _context;
        protected readonly DbSet<T> _dbSet;

        public EfGenericRepository(ProductPortalContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            entity.CreatedDate = DateTime.Now;
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity is not null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
