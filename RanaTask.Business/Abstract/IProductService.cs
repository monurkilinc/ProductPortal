using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Interfaces;
using IResult = ProductPortal.Core.Utilities.Results.IResult;

namespace ProductPortal.Business.Abstract
{
    public interface IProductService
    {
        Task<IDataResult<List<Product>>> GetAllAsync();
        Task<IDataResult<Product>> GetByIdAsync(int id);
        Task<IDataResult<Product>> GetByCodeAsync(string code);
        Task<IDataResult<Product>> AddAsync(Product product);
        Task<IDataResult<Product>> UpdateAsync(Product product);
        Task<IResult> DeleteAsync(int id);
    }
}
