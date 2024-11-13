using Microsoft.EntityFrameworkCore;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.DataAccess.Abstract;
using ProductPortal.DataAccess.Contexts;

namespace ProductPortal.DataAccess.Concrete
{
    public class EfProductRepository : EfGenericRepository<Product>, IProductRepository
    {
        public EfProductRepository(ProductPortalContext context) : base(context)
        {
        }

        public async Task<Product> GetProductByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Where(x => x.Price >= minPrice && x.Price <= maxPrice)
                .ToListAsync();
        }
    }
}
