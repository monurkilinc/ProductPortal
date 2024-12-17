using ProductPortal.Core.Entities.Aggregates;

namespace ProductPortal.DataAccess.Abstract
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByPriceRange(decimal minPrice, decimal maxPrice);
        Task<Product> GetProductByCodeAsync(string code);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByCodeAsync(string code);
    }
}
    