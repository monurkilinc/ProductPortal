using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Entities.DTOs;

namespace ProductPortal.Web.Controllers.API
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsApiController> _logger;

        public ProductsApiController(IProductService productService, ILogger<ProductsApiController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productService.DeleteAsync(id);
                if (result.Success)
                    return Ok(result.Message);
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün silinirken hata oluştu");
                return StatusCode(500, "Ürün silinemedi");
            }
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Product>>>> GetAll()
        {
            var result = await _productService.GetAllAsync();
            return Ok(new ApiResponse<List<Product>>
            {
                Success = result.Success,
                Data = result.Data,
                Message = result.Message,
                StatusCode = result.Success ? 200 : 400
            });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var result = await _productService.GetByIdAsync(id);
                if (result.Success)
                    return Ok(result.Data);
                return NotFound(result.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ürün bilgileri alınamadı");
            }
        }

    }
}
