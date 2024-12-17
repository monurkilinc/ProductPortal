using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Business.Commands.Product;
using ProductPortal.Business.Queries.Product;
using ProductPortal.Business.Queries.User;
using ProductPortal.Core.Entities.Aggregates;
using ProductPortal.Core.Utilities.Results;
using System.Security.Policy;

namespace ProductPortal.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _environment;

        public ProductController(IMediator mediator, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _environment = environment;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {

            var username = User.Identity.Name;
            var currentUser = await _mediator.Send(new GetUserByNameQuery(username));
            ViewBag.CurrentUser = currentUser.Data;

            var getAllQuerry = new GetAllProductsQuery();
            var getAll = await _mediator.Send(getAllQuerry);

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
        public async Task<IActionResult> Create([FromForm] CreateProductCommand command, IFormFile ImageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {

                    return View(command);
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    command.ImageURL = await SaveImage(ImageFile);
                }

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Ürün başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, result.Message);
                return View(command);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ürün oluşturulurken bir hata oluştu");
                return View(command);
            }
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);

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
        public async Task<IActionResult> Edit(UpdateProductCommand command, IFormFile ImageFile)
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
                    command.ImageURL = await SaveImage(ImageFile);
                }
                else
                {
                    var getProductQuery = new GetProductByIdQuery(command.Id);
                    var existingProduct = await _mediator.Send(getProductQuery);
                    if (existingProduct.Success && existingProduct.Data != null)
                    {
                        command.ImageURL = existingProduct.Data.ImageURL;
                    }
                }
                var result = await _mediator.Send(command);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, result.Message);
                return View(command);
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
                var command = new DeleteProductCommand(id);
                var result = await _mediator.Send(command);

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

