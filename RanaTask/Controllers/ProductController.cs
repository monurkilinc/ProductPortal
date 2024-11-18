using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.Concrete;

namespace ProductPortal.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [Authorize]
        public IActionResult Index()
        {

            var getAll = _productService.GetAllAsync();
            if(getAll.Result.Success)
            {
                return View(getAll.Result.Data);
            }
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product); // ModelState geçersizse, kullanıcıyı hata mesajları ile tekrar forma yönlendirir
            }

            var result = await _productService.UpdateAsync(product);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Ürün başarıyla güncellendi!";
                return RedirectToAction("~/Product/Index"); // Başarılı olursa, ürün listesine geri döner
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(product); // Hata durumunda, ürünü tekrar düzenleme sayfasına döner
        }

    }
}
