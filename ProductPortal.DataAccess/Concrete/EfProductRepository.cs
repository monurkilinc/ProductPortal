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
        public override async Task<Product> GetByIdAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var query = _context.Products;
                var sql = query.ToQueryString();
                Console.WriteLine($"Generated SQL: {sql}");

                return await query
                    .AsNoTracking()
                    .Select(p => new Product
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Code = p.Code,
                        Description = p.Description,
                        Stock = p.Stock,
                        Price = p.Price,
                        ImageURL = p.ImageURL,
                        CreatedDate = p.CreatedDate,
                        UpdatedDate = p.UpdatedDate,
                        IsActive = p.IsActive
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                throw new Exception($"Veritabanından ürünler alınırken bir hata oluştu. Detay: {ex.Message}, Inner: {innerMessage}", ex);
            }
        }
    }
}
