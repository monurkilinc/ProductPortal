using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Results.ErrorResult;
using ProductPortal.Core.Utilities.Results.SuccessResult;
using System.Security.Policy;

namespace ProductPortal.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IWebHostEnvironment _environment;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IWebHostEnvironment environment)
        {
            _productService = productService;
            _logger = logger;
            _environment = environment;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {

            var getAll =await _productService.GetAllAsync();
            if (getAll.Success)
            {
                return View(getAll.Data);
            }
            return View();
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Product product, IFormFile ImageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Model doğrulaması başarısızsa sayfayı yeniden döndür
                    return View(product);
                }

                // Eğer dosya varsa resmi kaydet-
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    product.ImageURL = await SaveImage(ImageFile);
                }

                // Ürünün oluşturulma tarihini ayarla
                product.CreatedDate = DateTime.Now;

                // Ürünü ekle
                var result = await _productService.AddAsync(product);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Ürün başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, result.Message);
                return View(product);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ürün oluşturulurken bir hata oluştu");
                return View(product);
            }
        }
        
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result.Success && result.Data != null)
            {
                return View(result.Data);
            }
            else
            {
                TempData["ErrorMessage"] = "Ürün bulunamadı.";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile ImageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, message = "Invalid model state", errors = errorMessages });
                }
                // Eğer yeni bir resim yüklenmişse, bu resmi kaydet ve ImageURL'i güncelle
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    product.ImageURL = await SaveImage(ImageFile);
                }
                else
                {
                    // Yeni bir resim yüklenmemişse, mevcut resim URL'sini koru
                    var existingProduct = await _productService.GetByIdAsync(product.Id);
                    if (existingProduct.Success && existingProduct.Data != null)
                    {
                        product.ImageURL = existingProduct.Data.ImageURL;
                    }
                }

                if (product.Id == 0) 
                {
                    product.CreatedDate = DateTime.Now;
                    product.IsActive = true; 
                }
                else
                {
                    product.UpdatedDate = DateTime.Now; 
                }

                var result = await _productService.UpdateAsync(product); if (result.Success)
                {
                    TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, result.Message);
                return View(product);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productService.DeleteAsync(id);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Ürün başarıyla silindi.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                TempData["ErrorMessage"] = "Ürün silinirken bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }
        [Authorize]
        [HttpGet]
        [Route("~/Product/GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var result = await _productService.GetAllAsync();
                return Json(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ürünler yüklenirken hata oluştu" });
            }
        }
        [Authorize]
        [HttpGet]
        [Route("~/Product/GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var result = await _productService.GetByIdAsync(id);

                if (result.Success && result.Data != null)
                {
                    return Ok(new { success = true, data = result.Data });
                }
                else
                {
                    return Ok(new { success = false, message = "Ürün bulunamadı" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ürün bilgileri alınamadı" });
            }
        }
        // Yüklenen resim dosyasını kaydeden metod
        private async Task<string> SaveImage(IFormFile ImageFile)
        {
            if (ImageFile == null || ImageFile.Length == 0)
                return null;

            var fileName = Path.GetRandomFileName() + Path.GetExtension(ImageFile.FileName);
            var filePath = Path.Combine(_environment.WebRootPath, "template", "assets", "images", "products", fileName);

            // Resim dosyası belirtilen yola kaydedilir
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }
            return $"/template/assets/images/products/{fileName}";
        }

    }
}

