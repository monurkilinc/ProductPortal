using ProductPortal.Core.Entities.Concrete;

namespace ProductPortal.DataAccess.Abstract
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByPriceRange(decimal minPrice, decimal maxPrice);
        Task<Product> GetProductByCodeAsync(string code);
    }
}
