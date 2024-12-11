using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Core.Utilities.Results;
using ProductPortal.DataAccess.Abstract;
using IResult = ProductPortal.Core.Utilities.Results.IResult;

namespace ProductPortal.Business.Concrete
{
    public class ProductManager : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly ICacheService cacheService;
        private readonly ILogger<Result> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ProductManager(IProductRepository productRepository, ICacheService cacheService, ILogger<Result> logger, IHttpContextAccessor httpContextAccessor)
        {

            this.productRepository = productRepository;
            this.cacheService = cacheService;
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IDataResult<Product>> AddAsync(Product product)
        {
            try
            {
                if (await productRepository.GetProductByCodeAsync(product.Code) != null)
                {
                    return new ErrorDataResult<Product>(
                        logger,
                        httpContextAccessor,
                        "Bu ürün kodu başka bir ürün tarafından kullanılıyor!");
                }

                if (product.Price <= 0)
                {
                    return new ErrorDataResult<Product>(
                        logger,
                        httpContextAccessor,
                        "Ürün fiyatı 0'dan büyük olmalıdır!");
                }

                product.CreatedDate = DateTime.Now;
                product.IsActive = true;

                var createdProduct = await productRepository.AddAsync(product);

                return new SuccessDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    createdProduct,
                    "Ürün başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    "Ürün oluşturulurken bir hata oluştu!");
            }
        }
        public async Task<IResult> DeleteAsync(int id)
        {
            try
            {
                var existingProduct = await productRepository.GetByIdAsync(id);
                if (existingProduct is null)
                {
                    return new ErrorDataResult<Product>(
                        logger,
                        httpContextAccessor,
                        "Silinecek urun bulunamadi");
                }

                await productRepository.DeleteAsync(id);

                return new SuccessDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    existingProduct,
                    "Urun basariyla silindi",
                    200);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    "Urun silinirken bir hata olustu!");
            }
        }
        public async Task<IDataResult<Product>> UpdateAsync(Product product)
        {
            try
            {
                var existingProduct = await productRepository.GetByIdAsync(product.Id);
                if (existingProduct == null)
                {
                    return new ErrorDataResult<Product>(
                        logger,
                        httpContextAccessor,
                        "Guncellenecek urun bulunamadi!");
                }
                if (existingProduct.Code != product.Code &&
                    await productRepository.GetProductByCodeAsync(product.Code) != null)
                {
                    return new ErrorDataResult<Product>(
                        logger,
                        httpContextAccessor,
                        "Bu urun kodu baska bir urun tarafindan kullaniliyor!");
                }

                if (product.Price <= 0)
                {
                    return new ErrorDataResult<Product>(
                        logger,
                        httpContextAccessor,
                        "Urun fiyati 0'dan buyuk olmalidir!");
                }

                product.CreatedDate = existingProduct.CreatedDate;
                product.IsActive = existingProduct.IsActive;

                existingProduct.Name = product.Name;
                existingProduct.Code = product.Code;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.ImageURL = product.ImageURL;
                existingProduct.UpdatedDate = DateTime.Now;

                var updatedProduct = await productRepository.UpdateAsync(existingProduct);

                return new SuccessDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    updatedProduct,
                    "Urun basariyla guncellendi");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    "Urun guncellenirken bir hata olustu!");
            }
        }
        public async Task<IDataResult<List<Product>>> GetAllAsync()
        {
            try
            {
                string cacheKey = "products_all";

                var cachedProducts = cacheService.Get<List<Product>>(cacheKey);
                if (cachedProducts != null)
                {
                    return new SuccessDataResult<List<Product>>(
                        logger,
                        httpContextAccessor,
                        cachedProducts,
                        "Urunler cache'den getirildi");
                }

                var products = (await productRepository.GetAllAsync()).ToList();

                cacheService.Set(cacheKey, products, TimeSpan.FromMinutes(5));

                return new SuccessDataResult<List<Product>>(
                    logger,
                    httpContextAccessor,
                    products,
                    "Urunler basariyla listelendi");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<Product>>(
                    logger,
                    httpContextAccessor,
                    "Urunler listelenirken bir hata olustu!");
            }
        }
        public async Task<IDataResult<Product>> GetByCodeAsync(string code)
        {
            try
            {
                var getProduct = await productRepository.GetProductByCodeAsync(code);
                if (getProduct == null)
                {
                    return new ErrorDataResult<Product>(
                        logger,
                        httpContextAccessor,
                        "Code degerine gore getirilicek urun bulunamadi!");
                }

                return new SuccessDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    getProduct,
                    "Urun basariyla getirildi");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    "Urun getirilirken bir hata olustu!");
            }
        }
        public async Task<IDataResult<Product>> GetByIdAsync(int id)
        {
            try
            {
                var getProductsById = await productRepository.GetByIdAsync(id);
                if (getProductsById is null)
                {
                    return new ErrorDataResult<Product>(
                 logger,
                 httpContextAccessor,
                 null,
                 "Ürün bulunamadı!");
                }

                return new SuccessDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    getProductsById,
                    "Urunler basariyla Id degerine gore getirildi"
                    );
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    null,
                    "Ürün getirilirken bir hata oluştu");
            }
        }
    }
}
