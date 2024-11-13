using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;
using System.Net.Mime;

namespace ProductPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)] // Ortak content type'lar
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] //Ortak response type'lar
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly ILogger<ProductsController> logger;

        public ProductsController(IProductService productService,ILogger<ProductsController> logger)
        {
            this.productService = productService;
            this.logger = logger;
        }
       
        //Urun ekler
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] Product product)
        {
            var result = await productService.AddAsync(product);
            if (result.Success)
            {
                var getById = nameof(GetById);
                var routeValues = new { id = result.Data.Id };
                var response = result;
                return CreatedAtAction(getById, routeValues, response);

            }
            return BadRequest(result);
        }

        //Urun gunceller
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("Girilen id degeri ile Urun Id degeri eslesmiyor!");
            }
            var result = await productService.UpdateAsync(product);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        //Urun siler
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await productService.DeleteAsync(id);
            if (result.Success)
            {
                return NoContent();
            }
            return BadRequest(result);
        }

        //Tum urunlerin listesini getirir
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            var result = await productService.GetAllAsync();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        //Id'ye gore urun getirir
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await productService.GetByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        //Urun koduna gore urun getirir
        [HttpGet("code/{code}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await productService.GetByCodeAsync(code);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
