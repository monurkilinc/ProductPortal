using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Results;
using System.Security.Policy;

namespace ProductPortal.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly ILogger<ProductController> _logger;
        private readonly IWebHostEnvironment _environment;

        public ProductController(IProductService productService, IUserService userService, ILogger<ProductController> logger, IWebHostEnvironment environment)
        {
            _productService = productService;
            _userService = userService;
            _logger = logger;
            _environment = environment;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {

            var username = User.Identity.Name;
            var currentUser = await _userService.GetUserByNameAsync(username);
            ViewBag.CurrentUser = currentUser.Data;

            var getAll = await _productService.GetAllAsync();
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Product product, IFormFile ImageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    
                    return View(product);
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    product.ImageURL = await SaveImage(ImageFile);
                }

                product.CreatedDate = DateTime.Now;
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
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    product.ImageURL = await SaveImage(ImageFile);
                }
                else
                {
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
               
                TempData["ErrorMessage"] = "Ürün silinirken bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }
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

