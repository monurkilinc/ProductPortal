﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Interfaces;
using ProductPortal.Core.Utilities.Results;
using ProductPortal.Core.Utilities.Results.ErrorResult;
using ProductPortal.Core.Utilities.Results.SuccessResult;
using ProductPortal.DataAccess.Abstract;
using IResult = ProductPortal.Core.Utilities.Interfaces.IResult;

namespace ProductPortal.Business.Concrete
{
    public class ProductManager : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly ILogger<Result> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ProductManager(IProductRepository productRepository, ILogger<Result> logger, IHttpContextAccessor httpContextAccessor)
        {
            this.productRepository = productRepository;
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IDataResult<Product>> AddAsync(Product product)
        {
            try
            {
                var existingProduct = await productRepository.GetProductByCodeAsync(product.Code);
                if (existingProduct is not null)
                {
                    return new ErrorDataResult<Product>(
                        logger,
                        httpContextAccessor,
                        "Bu ürün kodu kullaniliyor"
                        );


                }
                if (product.Price <= 0)
                {
                    return new ErrorDataResult<Product>(
                        logger,
                        httpContextAccessor,
                        "Urun fiyati 0'dan buyuk olmalidir");
                }

                var addedProduct = await productRepository.AddAsync(product);
                return new SuccessDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    addedProduct,
                    "Urun basariyla eklendi",
                    200);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AddAsync metodunda hata olustu. Product: {@Product}", product);
                return new ErrorDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    "Urun eklenirken bir hata olustu!");
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
                logger.LogError(ex, "DeleteAsync metodunda hata olustu. ProductId: {ProductId}", id);
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

                // Validasyonlar
                if (product.Price <= 0)
                {
                    return new ErrorDataResult<Product>(
                        logger,
                        httpContextAccessor,
                        "Urun fiyati 0'dan buyuk olmalidir!");
                }

                // Mevcut ürünün değişmeyecek bilgilerini koru
                product.CreatedDate = existingProduct.CreatedDate;

                // Güncelleme yap
                var updatedProduct = await productRepository.UpdateAsync(product);

                return new SuccessDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    updatedProduct,
                    "Urun basariyla guncellendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateAsync metodunda hata olustu. Product: {@Product}", product);
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
                var getProducts = (await productRepository.GetAllAsync()).ToList();
                return new SuccessDataResult<List<Product>>(
                    logger,
                    httpContextAccessor,
                    getProducts,
                    "Urunler basariyla listelendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllAsync metodunda hata olustu!");
                return new ErrorDataResult<List<Product>>(
                    logger,
                    httpContextAccessor,
                    "Urunler listelenirken bir hata olustu!"
                    );
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
                logger.LogError(ex, "GetByCodeAsync metodunda hata olustu. Code: {Code}", code);
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
                if(getProductsById is null)
                {
                    return new ErrorDataResult<Product>(
                        logger,
                        httpContextAccessor,
                        getProductsById,
                        "Urunler bulunamadi!");
                }
                return new SuccessDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    getProductsById,
                    "Urunler basariyla Id degerine gore getirildi"
                    );
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "GetByIdAsync metodunda hata olustu. ProductId: {ProductId}", id);
                return new ErrorDataResult<Product>(
                    logger,
                    httpContextAccessor,
                    "Urunler Id degerine gore getirilirken bir hata olustur");
            }
        }

    }
}